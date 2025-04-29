using DanceStudioFinder.Models;

namespace DanceStudioFinder.Services
{
    public interface IAdminService
    {
        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="midel"></param>
        /// <returns></returns>
        Task<bool> RegisterAdmin(Admin admin, string password);

        /// <summary>
        /// Получение пользователя по email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<Admin?> GetAdminByEmail(string email);


        /// <summary>
        /// Проверка пароля
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> ValidateAdmin(LoginViewModel model);
    }
}
