using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

public class Wallet
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    [Precision(18, 4)]
    public decimal Balance { get; set; } = 0.0m;

    public User? User { get; set; }
}