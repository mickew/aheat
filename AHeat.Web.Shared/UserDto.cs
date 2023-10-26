using FluentValidation;

namespace AHeat.Web.Shared;

public class UserDto
{
    public UserDto() : this(string.Empty, string.Empty, string.Empty) { }

    public UserDto(string id, string userName, string email)
    {
        Id = id;
        UserName = userName;
        Email = email;
    }

    public string Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public List<string> Roles { get; set; } = new();
}

public class UserDtoFluentValidator : AbstractValidator<UserDto>
{
    public UserDtoFluentValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().Length(8, 50);
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<UserDto>.CreateWithOptions((UserDto)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}
