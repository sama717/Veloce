namespace Veloco.Models;

public class PhoneChangeToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string NewPhoneNumber { get; set; }
    public string TokenHash { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public User User { get; set; }
}