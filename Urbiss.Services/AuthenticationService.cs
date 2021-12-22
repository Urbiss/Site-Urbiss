using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JWTSettingsDto _jwtSettings;
        private readonly AppSettingsDto _appSettings;
        private readonly ISendMailService _mailService;

        public AuthenticationService(UserManager<User> userManager,
            IOptions<JWTSettingsDto> jwtSettings,
            IOptions<AppSettingsDto> appSettings,
            SignInManager<User> signInManager, ISendMailService mailService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _appSettings = appSettings.Value;
            _signInManager = signInManager;
            _mailService = mailService;
        }

        public async Task<TokenResponseDto> GetTokenAsync(TokenRequestDto request, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new ApiException("Usuário ou senha inválidos!");
            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if ((_signInManager.Options.SignIn.RequireConfirmedAccount) && (!user.EmailConfirmed))
                throw new ApiException($"O e-mail {request.Email} não foi confirmado!");
            if (!user.IsActive)
                throw new ApiException($"A conta {request.Email} não está ativa. Entre em contato por meio do endereço contato@urbiss.com.br");
            if (!result.Succeeded)
                throw new ApiException("Usuário ou senha inválidos!");
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
            var response = new TokenResponseDto
            {
                Id = user.Id.ToString(),
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime(),
                ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime(),
                Email = user.Email,
                UserName = user.UserName
            };
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            var refreshToken = GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;
            return response;
        }

        //TODO: implementar refreshtoken
        //https://jasonwatmore.com/post/2020/07/06/aspnet-core-3-boilerplate-api-with-email-sign-up-verification-authentication-forgot-password#revoke-token-request-cs

        private async Task<JwtSecurityToken> GenerateJWToken(User user, string ipAddress)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString()),
                new Claim("full_name", user.FullName),
                new Claim("ip", ipAddress)
            }
            .Union(userClaims)
            .Union(roleClaims);
            return JWTGeneration(claims);
        }

        private JwtSecurityToken JWTGeneration(IEnumerable<Claim> claims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private static string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private RefreshTokenDto GenerateRefreshToken(string ipAddress)
        {
            return new RefreshTokenDto
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private const string REGISTER_CONFIRM_MAIL_TEMPLATE = @"
            <font size=""2"" face=""Verdana"">
            <table width=""700"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                    <td>
                        <div align=""center""><img src=""http://urbiss.com.br/wp-content/uploads/2021/03/logo-urbis-2.png"" width=""333"" height=""90"" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>            
                <tr>
                    <td style=""text-align:center"">Seja muito bem vindo à plataforma URBISS. <p>Para confirmar o seu cadastro, clique no link abaixo:</p>
                    </td>
                </tr>            
                <tr>
                    <td>&nbsp;</td>
                </tr>            
                <tr>
                    <td style=""text-align:center""><a href='{0}'>Confirmar cadastro</a>
                    </td
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>            
                <tr>
                    <td><p align=""center"">Mensagem automática enviada pela Urbiss (não responda) <br /><td/>
                <tr/>
            </table>
        ";

        public async Task<int> RegisterAsync(RegisterRequestDto request)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.Email);
            if (userWithSameUserName != null)
            {
                throw new ApiException($"Usuário '{request.Email}' já existe.");
            }
            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                UserName = request.Email,
                IsActive = true
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                var verificationUri = await GetVerificationEmailUrl(user);
                _mailService.Send(new MailRequestDto()
                {
                    To = user.Email,
                    Body = string.Format(REGISTER_CONFIRM_MAIL_TEMPLATE, verificationUri),
                    Subject = "Confirmação do registro"
                });
                return user.Id;
            }
            else
            {
                throw new ApiException($"{result.Errors}");
            }
        }

        private async Task<string> GetVerificationEmailUrl(User user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var endpointUri = new Uri(string.Concat($"{_appSettings.Url}/", $"user/{user.Id}/confirmemail/{code}"));
            return endpointUri.ToString();
        }

        public async Task<string> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return user.Email;
            else
                throw new ApiException($"Ocorreu um erro ao confirmar o e-mail {user.Email}.");
        }

        private const string FORGOT_PASSWORD_MAIL_TEMPLATE = @"
            <font size=""2"" face=""Verdana"">
            <table width=""700"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                    <td>
                        <div align=""center""><img src=""http://urbiss.com.br/wp-content/uploads/2021/03/logo-urbis-2.png"" width=""333"" height=""90"" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>            
                <tr>
                    <td style=""text-align:center"">Foi solicitado uma redefinição de senha. <p>Para redefini-la, clique no link abaixo:</p>
                    </td>
                </tr>            
                <tr>
                    <td>&nbsp;</td>
                </tr>            
                <tr>
                    <td style=""text-align:center""><a href='{0}'>Redefinir senha</a>
                    </td
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>            
                <tr>
                    <td><p align=""center"">Mensagem automática enviada pela Urbiss (não responda) <br /><td/>
                <tr/>
            </table>
        ";

        public async Task ForgotPassword(ForgotPasswordRequestDto model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);

            //Evita tentativas de inúmeros e-mails
            // always return ok response to prevent email enumeration
            if (account == null) return;

            var code = await _userManager.GeneratePasswordResetTokenAsync(account);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var endpointUri = new Uri(string.Concat($"{_appSettings.Url}/", $"user/{account.Email}/forgotpassword/{code}"));

            var emailRequest = new MailRequestDto()
            {
                Body = string.Format(FORGOT_PASSWORD_MAIL_TEMPLATE, HttpUtility.HtmlEncode(endpointUri.ToString())),
                To = model.Email,
                Subject = "Redefinir senha",
            };
            _mailService.Send(emailRequest);
        }

        public async Task ResetPassword(ResetPasswordRequestDto model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null) throw new ApiException($"O e-mail {model.Email} não pertence a nenhuma conta.");
            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(account, token, model.Password);
            if (!result.Succeeded)
                throw new ApiException($"Ocorreu um erro durante a redefinição da senha!");
        }
    }
}
