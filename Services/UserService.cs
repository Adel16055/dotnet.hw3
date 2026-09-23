using Homework.Contracts;
using Homework.Data;
using Homework.Models;
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace Homework.Services;

public sealed class DuplicateLoginException() : Exception("Пользователь с таким логином уже существует.");

public sealed class UserService(UserRepository repository, IPasswordHasher<User> hasher)
{
    public async Task<UserResponse> CreateAsync(SaveUserRequest request)
    {
        var now = UtcNow();
        var user = new User { Login = request.Login, Name = request.Name.Trim(), CreatedDate = now, UpdatedDate = now };
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        try { await repository.InsertAsync(user); }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        { throw new DuplicateLoginException(); }
        return ToResponse(user);
    }

    public async Task<UserResponse?> AuthorizeAsync(LoginRequest request)
    {
        var user = await repository.FindAsync(request.Login);
        if (user is null || hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed) return null;
        return ToResponse(user);
    }

    public async Task<UserResponse?> EditAsync(string login, SaveUserRequest request)
    {
        var user = await repository.FindAsync(login);
        if (user is null) return null;
        user.Login = request.Login;
        user.Name = request.Name.Trim();
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        user.UpdatedDate = UtcNow();
        try { if (!await repository.UpdateAsync(user)) return null; }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        { throw new DuplicateLoginException(); }
        return ToResponse(user);
    }

    public Task<bool> DeleteAsync(string login) => repository.DeleteAsync(login);
    public async Task<IEnumerable<UserResponse>> GetAllAsync(UserFilter filter) =>
        (await repository.GetAllAsync(filter)).Select(ToResponse);
    private static DateTime UtcNow()
    {
        var now = DateTime.UtcNow;
        return new DateTime(now.Ticks - now.Ticks % 10, DateTimeKind.Utc);
    }

    private static UserResponse ToResponse(User user) =>
        new(user.Id, user.Login, user.Name, user.CreatedDate, user.UpdatedDate);
}
