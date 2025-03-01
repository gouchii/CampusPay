using api.DTOs.User;
using api.Models;

namespace api.Mappers;

public static class UserMappers
{
    public static UserDto ToUserDto(this User userModel)
    {
        return new UserDto
        {
            Name = userModel.Name,
            EmailAddress = userModel.EmailAddress,
            Wallets = userModel.Wallets.Select(w => w.ToWalletDto()).ToList()
        };
    }
    public static User ToUserFromCreateDto(this CreateUserRequestDto userDto)
    {
        return new User
        {
            Name = userDto.Name,
            EmailAddress = userDto.EmailAddress
        };
    }

    public static User ToUserFromUpdateDto(this UpdateUserRequestDto userDto)
    {
        return new User
        {
            Name = userDto.Name,
            EmailAddress = userDto.EmailAddress
        };
    }
}