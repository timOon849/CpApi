using Microsoft.AspNetCore.Mvc;
using CpApi.Requests;
using CpApi.Model;

namespace CpApi.Interfaces
{
    public interface IUsersLoginsService
    {
        Task<IActionResult> GetAllUsersAsync();
        Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUserAndLogin newUser);
        Task<IActionResult> Authorization(string email, string pass);
        Task<IActionResult> DeleteUser(int Id);
        Task<IActionResult> EditUser(Users user, string email, string pass);
        Task<IActionResult> GetUsers();

    }
}
