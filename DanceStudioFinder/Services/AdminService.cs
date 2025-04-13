using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Services
{
    public class AdminService: IAdminService
    {
        private readonly ApplicationDbContext _context;  // Замените YourDbContext на ваш DbContext
        private readonly IPasswordHasher<Admin> _passwordHasher;

        public AdminService(ApplicationDbContext context, IPasswordHasher<Admin> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }


        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="model">Модель для валидации регистрирующегося пользователя</param>
        /// <returns></returns>
        public async Task<bool> RegisterAdmin(Admin admin, string password)
        {            
            if (await _context.Admins.AnyAsync(a => a.Email == admin.Email))  //если пользователь с регистрируемой почтой уже существует
            {
                return false; //нельзя зарегистрировать
            }
            //хэширование введённого пароля
            admin.Password = _passwordHasher.HashPassword(admin, password);

            _context.Admins.Add(admin);         //сохранение администратора в БД
            await _context.SaveChangesAsync();  //сохранить изменения
            return true;                        //успешное завершение регистрации
        }

        public async Task<Admin?> GetAdminByEmail(string email)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
        }

        /*public bool IsAdminConfigured()
        {
            return _context.Admins.Any(); // Проверка на наличие хотя бы одного администратора
        }*/

        public async Task<bool> ValidateAdmin(LoginViewModel model)
        {
            // 1. Получение администратора по почте
            var admin = await GetAdminByEmail(model.LoginEmail);

            // 2. Проверка существования администратора
            if (admin == null)
            {
                return false; // Администратор не найден
            }

            // 3. Проверка пароля
            var result = _passwordHasher.VerifyHashedPassword(admin, admin.Password, model.LoginPassword);

            // 4. Возврат результата проверки
            return result == PasswordVerificationResult.Success;
        }
    }
}
