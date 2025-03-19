using api.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace api.DTOs.Wallet;

public class WalletDto
{
    public DateTime CreatedAt { get; init; }

    [JsonConverter(typeof(StringEnumConverter))]
    public WalletType Type { get; set; }

    public decimal Balance { get; set; }
}