using System.ComponentModel.DataAnnotations;

namespace Homework.Contracts;

public sealed class SaveUserRequest
{
    [Required(ErrorMessage = "Введите логин."), RegularExpression(@"^[a-zA-Z0-9_]{3,50}$", ErrorMessage = "Логин должен содержать 3-50 символов. Разрешены латинские буквы, цифры и знак подчёркивания.")]
    public string Login { get; set; } = "";
    [Required(ErrorMessage = "Введите имя."), StringLength(100, MinimumLength = 1, ErrorMessage = "Имя должно содержать 1-100 символов.")]
    public string Name { get; set; } = "";
    [Required(ErrorMessage = "Введите пароль."), StringLength(128, MinimumLength = 8, ErrorMessage = "Пароль должен содержать 8-128 символов.")]
    public string Password { get; set; } = "";
}

public sealed class LoginRequest
{
    [Required(ErrorMessage = "Введите логин."), StringLength(50, ErrorMessage = "Логин не должен превышать 50 символов.")] public string Login { get; set; } = "";
    [Required(ErrorMessage = "Введите пароль."), StringLength(128, ErrorMessage = "Пароль не должен превышать 128 символов.")] public string Password { get; set; } = "";
}

public sealed record UserResponse(Guid Id, string Login, string Name, DateTime CreatedDate, DateTime UpdatedDate);

public sealed class UserFilter : IValidatableObject
{
    public DateTimeOffset? CreatedFrom { get; set; }
    public DateTimeOffset? CreatedTo { get; set; }
    public DateTimeOffset? UpdatedFrom { get; set; }
    public DateTimeOffset? UpdatedTo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (CreatedFrom > CreatedTo)
            yield return new ValidationResult("Неверный период создания.", [nameof(CreatedFrom)]);
        if (UpdatedFrom > UpdatedTo)
            yield return new ValidationResult("Неверный период изменения.", [nameof(UpdatedFrom)]);
    }
}
