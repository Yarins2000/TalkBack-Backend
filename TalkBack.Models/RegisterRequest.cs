namespace TalkBack.Models
{
    /// <summary>
    /// A class represents a register request.
    /// </summary>
    public class RegisterRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
