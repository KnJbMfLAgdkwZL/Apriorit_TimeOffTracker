using System;
using System.Collections.Generic;

namespace TimeOffTracker.Model
{
    public partial class UserSignature
    {
        public int Id { get; set; }
        public int NInQueue { get; set; }
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public bool Approved { get; set; }
        public bool Deleted { get; set; }

        public virtual Request Request { get; set; }
        public virtual User User { get; set; }
    }
}
