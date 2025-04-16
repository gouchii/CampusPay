using api.Features.Wallet;
using api.Shared.Wallet.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace api.Shared.DTOs.Wallet;

public class WalletDto
{
    public DateTime CreatedAt { get; init; }

    [JsonConverter(typeof(StringEnumConverter))]
    public WalletType Type { get; set; }

    public decimal Balance { get; set; }
}