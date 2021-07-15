namespace TimeOffTracker.Model.DTO
{
    public partial class RequestTypeDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Comments { get; set; }
        public bool Deleted { get; set; }
    }
}