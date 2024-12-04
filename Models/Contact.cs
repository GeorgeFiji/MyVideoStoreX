namespace MyVideostore.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }

        public required string Message { get; set; }
        public required string PhoneNumber { get; set; }

            
    }
}
