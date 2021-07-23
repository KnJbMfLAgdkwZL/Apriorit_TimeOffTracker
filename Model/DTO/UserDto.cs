using TimeOffTracker.Model.Enum;

namespace TimeOffTracker.Model.DTO
{
    public partial class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Password { get; set; }
        public UserRoles RoleId { get; set; }
        public bool Deleted { get; set; }
    }
}