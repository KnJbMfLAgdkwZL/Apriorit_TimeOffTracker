using System;
using System.Collections.Generic;

namespace TimeOffTracker.Model
{
    public partial class StateDetail
    {
        public StateDetail()
        {
            Requests = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public string Comments { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<Request> Requests { get; set; }
    }
}
