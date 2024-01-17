using FluentValidation;

namespace AHeat.Web.Shared;

public class UserDto
{
    public UserDto() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) { }

    public UserDto(string id, string userName, string email, string firstName, string lastName)
    {
        Id = id;
        UserName = userName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public List<string> Roles { get; set; } = new();
}

public class UserDtoFluentValidator : AbstractValidator<UserDto>
{
    public UserDtoFluentValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().Length(4, 50);
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName)
            .NotEmpty();
        RuleFor(x => x.LastName)
            .NotEmpty();
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<UserDto>.CreateWithOptions((UserDto)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}
