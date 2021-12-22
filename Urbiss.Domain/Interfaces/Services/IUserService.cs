using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Interfaces
{
    public interface IUserService
    {
        int CurrentUserId { get; }
        string CurrentUserName { get; }
        Task<bool> Exists(long id);
        Task<User> FindById(long id);
        Task<string[]> GetRoles(long id);        
        Task<bool> IsInRole(long id, string role);
        Task<bool> IsAdmin(long id);
        Task<bool> IsSurveyor(long id);

        Task<UserDto> GetUser(int id);

        Task SaveUser(UserDto user);

        Task SaveRoles(UserRolesDto user);
    }
}
