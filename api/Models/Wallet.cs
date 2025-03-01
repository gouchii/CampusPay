namespace api.Models;

public class Wallet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Balance { get; set; } = 0.0m;

    public User User { get; set; }
}