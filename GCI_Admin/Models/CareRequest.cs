namespace GCI_Admin.Models
{
    public class CareRequest
    {
        public int Id { get; set; }

        public int? MemberId { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string RequestType { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public bool IsAnonymous { get; set; }

        public bool IsPicked { get; set; }

        public int? PickedByPastorId { get; set; }

        public DateTime? PickedAt { get; set; }

        public string Response { get; set; }

        public DateTime? RespondedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
