using System.Security.Cryptography;
using System.Text;

public class UserService
{
    private readonly List<User> users = new List<User>();

    public void CreateUser(string login, string name, string password)
    {
        User? user = FindUser(login);

        if (user != null)
        {
            Console.WriteLine("Пользователь уже существует");
            return;
        }

        string passwordHash = HashPassword(password);

        User newUser = new User(login, name, passwordHash);
        users.Add(newUser);

        Console.WriteLine("Пользователь создан");
    }

    public void Authorize(string login, string password)
    {
        User? user = FindUser(login);

        if (user == null)
        {
            Console.WriteLine("Неверный логин или пароль");
            return;
        }

        string passwordHash = HashPassword(password);

        if (user.PasswordHash != passwordHash)
        {
            Console.WriteLine("Неверный логин или пароль");
            return;
        }

        Console.WriteLine("Авторизация успешна");
        Console.WriteLine($"Здравствуйте, {user.Name}");
    }

    public void EditUser(
        string login,
        string newLogin,
        string newName,
        string newPassword)
    {
        User? user = FindUser(login);

        if (user == null)
        {
            Console.WriteLine("Пользователь не найден");
            return;
        }

        user.Login = newLogin;
        user.Name = newName;
        user.PasswordHash = HashPassword(newPassword);

        Console.WriteLine("Пользователь изменён");
    }

    public void DeleteUser(string login)
    {
        User? user = FindUser(login);

        if (user == null)
        {
            Console.WriteLine("Пользователь не найден");
            return;
        }

        users.Remove(user);

        Console.WriteLine("Пользователь удалён");
    }

    private User? FindUser(string login)
    {
        foreach (User user in users)
        {
            if (user.Login == login)
            {
                return user;
            }
        }

        return null;
    }

    private string HashPassword(string password)
    {
        using SHA256 sha256 = SHA256.Create();

        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = sha256.ComputeHash(passwordBytes);

        return Convert.ToHexString(hashBytes);
    }
}
