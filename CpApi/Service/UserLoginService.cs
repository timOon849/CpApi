using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CpApi.DataBaseContext;
using CpApi.Interfaces;
using CpApi.Model;
using CpApi.Requests;

namespace CpApi.Service
{
    public class UserLoginService : IUsersLoginsService
    {
        private readonly ContextDb _context;

        public UserLoginService(ContextDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetAllUsersAsync()
        {
            var logins = await _context.Logins.ToListAsync();
            var users = await _context.Users.ToListAsync();

            return new OkObjectResult(new
            {
                data = new { users = users },
                status = true
            });
        }

        public async Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUserAndLogin newUser)
        {
            var user = new Users()
            {
                Name = newUser.Name,
                AboutMe = newUser.AboutMe,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var login = new Logins()
            {
                User_id = user.id_User,
                Email = newUser.Email,
                Password = newUser.Password,
            };

            await _context.Logins.AddAsync(login);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true
            });
        }

        public async Task<IActionResult> Authorization(string email, string pass)
        {
            var login = await _context.Logins.FirstOrDefaultAsync(a => a.Email == email && a.Password == pass);
            if (login is null)
            {
                return new NotFoundObjectResult(new { status = false, MessageContent = "Такого пользователя нет" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(a => a.id_User == login.User_id);
            return new OkObjectResult(new { status = true, user });
        }

        public async Task<IActionResult> DeleteUser(int Id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.id_User == Id);
            var login = await _context.Logins.FirstOrDefaultAsync(a => a.User_id == Id);

            if (user is null)
            {
                return new NotFoundObjectResult(new { status = false, MessageContent = "Пользователь не найден" });
            }

            try 
            {
                _context.Remove(login);
                _context.Remove(user);
            }
            catch { return new NotFoundObjectResult(new { status = false, MessageContent = "Логин пользователя не найден" }); }
            
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true });
        }

        public async Task<IActionResult> EditUser(Users user, string email, string pass)
        {
            var edituser = await _context.Users.FirstOrDefaultAsync(a => a.id_User == user.id_User);
            if (edituser is null)
            {
                return new NotFoundObjectResult(new { status = false, MessageContent = "Пользователь не найден" });
            }
            var login = await _context.Logins.FirstOrDefaultAsync(a => a.User_id == user.id_User);

            edituser.Name = user.Name;
            edituser.AboutMe = user.AboutMe;
            edituser.Admin = user.Admin;
            login.User_id = user.id_User;
            login.Email = email;
            login.Password = pass;

            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, edituser, login });

        }

        public async Task<IActionResult> GetUsers()
        {
            var usersWithLogins = from user in _context.Users
                                  join login in _context.Logins on user.id_User equals login.User_id
                                  select new
                                  {
                                      user.id_User,
                                      user.Name,
                                      user.AboutMe,
                                      login.Email,
                                      login.Password
                                  };

            return new OkObjectResult(usersWithLogins.ToList());
        }
    }
}
