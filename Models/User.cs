namespace WindowsFormsApp.Models
{
    public enum UserRole
    {
        Admin,
        Cashier
    }

    public abstract class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; protected set; }
    }

    public class AdminUser : User
    {
        public AdminUser()
        {
            Role = UserRole.Admin;
        }
    }

    public class CashierUser : User
    {
        public CashierUser()
        {
            Role = UserRole.Cashier;
        }
    }
}


