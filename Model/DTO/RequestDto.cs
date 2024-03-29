﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TimeOffTracker.Model.Enum;

namespace TimeOffTracker.Model.DTO
{
    public partial class RequestDto
    {
        public int Id { get; set; }
        public RequestTypes RequestTypeId { get; set; }
        public string Reason { get; set; }
        public string ProjectRoleComment { get; set; }
        public ProjectRoleTypes ProjectRoleTypeId { get; set; }
        public int UserId { get; set; }
        public StateDetails StateDetailId { get; set; }
        public DateTime DateTimeFrom { get; set; }
        public DateTime DateTimeTo { get; set; }
        
        public UserDto User { get; set; }
        public List<UserSignatureDto> UserSignature { get; set; }
    }
}