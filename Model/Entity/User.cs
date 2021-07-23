using System;
using System.Collections.Generic;

namespace TimeOffTracker.Model
{
    public partial class User
    {
        public User()
        {
            Requests = new HashSet<Request>();
            UserSignatures = new HashSet<UserSignature>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<UserSignature> UserSignatures { get; set; }
    }
}
