public class User {
    public string Login {
      get; 
      set; 
    }
    public string Name {
      get; 
      set; 
    }
    public string PasswordHash {
      get; 
      set; 
    }

    public User(string login, string name, string passwordHash)
    {
        Login = login;
        Name = name;
        PasswordHash = passwordHash;
    }
}