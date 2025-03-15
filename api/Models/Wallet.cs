using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace api.Models;

public class Wallet
{
    [Key]
    public int Id { get; init; }

    [MaxLength(450)]
    public string UserId { get; init; } = string.Empty;

    [Precision(18, 4)]
    public decimal Balance { get; set; }

    public User? User { get; set; }
}