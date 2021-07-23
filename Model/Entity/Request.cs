using System;
using System.Collections.Generic;

namespace TimeOffTracker.Model
{
    public partial class Request
    {
        public Request()
        {
            UserSignatures = new HashSet<UserSignature>();
        }

        public int Id { get; set; }
        public int RequestTypeId { get; set; }
        public string Reason { get; set; }
        public string ProjectRoleComment { get; set; }
        public int ProjectRoleTypeId { get; set; }
        public int UserId { get; set; }
        public int StateDetailId { get; set; }
        public DateTime DateTimeFrom { get; set; }
        public DateTime DateTimeTo { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<UserSignature> UserSignatures { get; set; }
    }
}
