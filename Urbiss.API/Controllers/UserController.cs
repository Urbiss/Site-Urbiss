using AspNetCoreHero.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;

namespace Urbiss.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : UrbissControllerBase<UserController>
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserService _userService;

        public UserController(IAuthenticationService authService, IUserService userService)
        {
            this._authService = authService;
            this._userService = userService;
        }
        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        [HttpGet("getlogged")]
        public async Task<Result<UserDto>> GetLoggedUser()
        {
            var user = await this._userService.GetUser(_userService.CurrentUserId);
            return Result<UserDto>.Success(user, "Usuário logado");
        }

        [HttpPost("savelogged")]
        public async Task<IResult> SaveLoggedUser([FromBody] UserDto user)
        {
            await _userService.SaveUser(user);
            if (await _userService.IsAdmin(_userService.CurrentUserId))
            {
                var userRolesDto = new UserRolesDto()
                {
                    Id = user.Id,
                    Roles = user.Roles
                };
                await _userService.SaveRoles(userRolesDto);
            }
            return Result.Success();
        }

        [HttpPost("saveroles")]
        [Authorize(Roles = "Admin")]
        public async Task<IResult> SaveRoles([FromBody] UserRolesDto user)
        {
            await _userService.SaveRoles(user);
            return Result.Success();
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<Result<TokenResponseDto>> Authenticate(TokenRequestDto tokenRequest)
        {
            var ipAddress = GenerateIPAddress();
            var token = await _authService.GetTokenAsync(tokenRequest, ipAddress);
            return Result<TokenResponseDto>.Success(token, "Usuário autenticado com sucesso!");
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<Result<int>> Register(RegisterRequestDto request)
        {
            var idUser = await _authService.RegisterAsync(request);
            return Result<int>.Success(idUser, $"Usuário registrado com sucesso com o id {idUser}. Um e-mail de confirmação foi enviado!");
        }

        [HttpGet("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<Result<string>> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            var email = await _authService.ConfirmEmailAsync(userId, code);
            return Result<string>.Success(email, $"O e-mail {email} foi confirmado com sucesso!");
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IResult> ForgotPassword(ForgotPasswordRequestDto model)
        {
            await _authService.ForgotPassword(model);
            return Result.Success();
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IResult> ResetPassword(ResetPasswordRequestDto model)
        {
            await _authService.ResetPassword(model);
            return Result.Success();
        }
    }
}
