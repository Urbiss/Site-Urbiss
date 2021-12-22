namespace Urbiss.Domain.Dtos
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string[] Roles { get; set; }
    }

    public class UserRolesDto
    {
        public long Id { get; set; }
        public string[] Roles { get; set; }
    }
}
