using WindowsFormsApp.Models;

namespace WindowsFormsApp.Factory
{
    /// <summary>
    /// Factory Pattern:
    /// Creates concrete User objects based on role.
    /// </summary>
    public static class UserFactory
    {
        public static User CreateUser(UserRole role, int id, string fullName)
        {
            User user = role switch
            {
                UserRole.Admin => new AdminUser(),
                UserRole.Cashier => new CashierUser(),
                _ => new CashierUser()
            };

            user.Id = id;
            user.FullName = fullName;
            return user;
        }
    }
}


