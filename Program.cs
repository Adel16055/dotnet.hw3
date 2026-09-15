UserService userService = new UserService();

while (true){
    Console.WriteLine();
    Console.WriteLine("1. Создать пользователя");
    Console.WriteLine("2. Авторизация");
    Console.WriteLine("3. Редактировать пользователя");
    Console.WriteLine("4. Удалить пользователя");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите действие: ");

    string? command = Console.ReadLine();

    if (command == "1") {
        Console.Write("Введите логин: ");
        string login = Console.ReadLine() ?? "";

        Console.Write("Введите имя: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Введите пароль: ");
        string password = Console.ReadLine() ?? "";

        userService.CreateUser(login, name, password);
    }
    else if (command == "2") {
        Console.Write("Введите логин: ");
        string login = Console.ReadLine() ?? "";

        Console.Write("Введите пароль: ");
        string password = Console.ReadLine() ?? "";

        userService.Authorize(login, password);
    }
    else if (command == "3")
    {
        Console.Write("Введите текущий логин: ");
        string login = Console.ReadLine() ?? "";

        Console.Write("Введите новый логин: ");
        string newLogin = Console.ReadLine() ?? "";

        Console.Write("Введите новое имя: ");
        string newName = Console.ReadLine() ?? "";

        Console.Write("Введите новый пароль: ");
        string newPassword = Console.ReadLine() ?? "";

        userService.EditUser(
            login,
            newLogin,
            newName,
            newPassword
        );
    }
    else if (command == "4")
    {
        Console.Write("Введите логин: ");
        string login = Console.ReadLine() ?? "";

        userService.DeleteUser(login);
    }
    else if (command == "0")
    {
        break;
    }
    else
    {
        Console.WriteLine("Неизвестная команда");
    }
}