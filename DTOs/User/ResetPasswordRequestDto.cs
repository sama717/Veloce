namespace Veloco.DTOs.User;

public class ResetPasswordRequestDto
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}