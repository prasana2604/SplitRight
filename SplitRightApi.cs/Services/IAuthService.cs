using SplitRight.API.Models.Entities;
using SplitRight.API.Models;
using SplitRightApi.cs.Models;
namespace SplitRight.API.Services;


public interface IAuthService
{
    Task<String> RegisterAsync(RegisterDto register);

    Task<String> LoginAsync(LoginDto login);
}
