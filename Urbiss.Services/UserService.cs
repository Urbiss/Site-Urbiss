using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this._context = httpContextAccessor;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public int CurrentUserId { get => Convert.ToInt32(_context.HttpContext.User.FindFirstValue("uid")); }

        public string CurrentUserName { get => _context.HttpContext.User.FindFirstValue(ClaimTypes.Email); }

        public async Task<bool> Exists(long id)
        {
            return (await FindById(id) != null);
        }

        public async Task<User> FindById(long id)
        {
            return (await _userManager.FindByIdAsync(id.ToString()));
        }

        public async Task<string[]> GetRoles(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var roles = (await _userManager.GetRolesAsync(user)).ToArray();
            return roles;
        }

        public async Task<bool> IsAdmin(long id)
        {
            return await IsInRole(id, "Admin");
        }

        public async Task<bool> IsInRole(long id, string role)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var isInRole = await _userManager.IsInRoleAsync(user, role);
            return isInRole;
        }

        public async Task<bool> IsSurveyor(long id)
        {
            return await IsInRole(id, "Surveyor");
        }

        public async Task<UserDto> GetUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ApiException($"Não existe usuário com o id {id}!");
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles.ToArray()
            };
        }

        public async Task SaveUser(UserDto user)
        {
            var userRepo = await _userManager.FindByIdAsync(user.Id.ToString());
            userRepo.FullName = user.FullName;
            //Não permito a alteração do e-mail
            //userRepo.Email = user.Email;
            await _userManager.UpdateAsync(userRepo);
        }

        public async Task SaveRoles(UserRolesDto user)
        {
            IQueryable<User> queryUser = _userManager.Users.Include(ur => ur.UserRoles).ThenInclude(ur => ur.Role);
            var roles = _roleManager.Roles;
            var userRepo = await queryUser.Where(u => u.Id == user.Id).FirstOrDefaultAsync();
            userRepo.UserRoles.Clear();
            UserRole[] newRoles = Array.ConvertAll<string, UserRole>(user.Roles, r => 
                new UserRole() 
                { 
                    UserId = Convert.ToInt32(user.Id), 
                    RoleId = roles.Where(role => role.Name.Equals(r)).FirstOrDefault().Id 
                });
            userRepo.UserRoles.AddRange(newRoles);
            await _userManager.UpdateAsync(userRepo);
        }
    }
}
