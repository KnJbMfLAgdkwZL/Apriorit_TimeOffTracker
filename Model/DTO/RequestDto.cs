using System;
using System.Collections.Generic;

namespace TimeOffTracker.Model.DTO
{
    public partial class RequestDto
    {
        public int Id { get; set; }
        public int RequestTypeId { get; set; }
        public string Reason { get; set; }
        public string ProjectRoleComment { get; set; }
        public int ProjectRoleTypeId { get; set; }
        public int UserId { get; set; }
        public int StateDetailId { get; set; }
        public DateTime DateTimeFrom { get; set; }
        public DateTime DateTimeTo { get; set; }
        public virtual List<UserSignatureDto> UserSignatureDto { get; set; }
    }
}