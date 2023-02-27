namespace TalkBack.Models
{
    public class RegisterRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
