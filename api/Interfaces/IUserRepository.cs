using api.Models;

namespace api.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User> CreateAsync(User userModel);
    Task<User?> DeleteAsync(string id);
    Task<User?> UpdateAsync(string id, User userModel);
}