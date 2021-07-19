using System.Collections.Generic;

namespace TimeOffTracker.Model
{
    public partial class UserRole
    {
        public UserRole()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public string Comments { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}