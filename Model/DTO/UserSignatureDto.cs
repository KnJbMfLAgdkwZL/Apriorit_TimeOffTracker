namespace TimeOffTracker.Model.DTO
{
    public partial class UserSignatureDto
    {
        public int Id { get; set; }
        public int NInQueue { get; set; }
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public bool Approved { get; set; }
        public bool Deleted { get; set; }
        public string Reason { get; set; }

        public UserDto User { get; set; }
    }
}