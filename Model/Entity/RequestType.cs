using System.Collections.Generic;

namespace TimeOffTracker.Model
{
    public partial class RequestType
    {
        public RequestType()
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