namespace TalkBack.Models
{
    /// <summary>
    /// A record represents a change password request.
    /// </summary>
    public record ChangePasswordRequest(string Username, string Password, string ConfirmPassword);
}
