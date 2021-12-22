using AspNetCoreHero.Results;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;

namespace Urbiss.Domain.Interfaces
{
    public interface IAuthenticationService
    {
        Task<TokenResponseDto> GetTokenAsync(TokenRequestDto request, string ipAddress);

        Task<int> RegisterAsync(RegisterRequestDto request);

        Task<string> ConfirmEmailAsync(string userId, string code);

        Task ForgotPassword(ForgotPasswordRequestDto model);

        Task ResetPassword(ResetPasswordRequestDto model);
    }
}
