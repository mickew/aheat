using FluentValidation;

namespace AHeat.Web.Shared;

public class ChangePasswordDto
{     
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}


public class ChangePasswordDtoFluentValidator: AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoFluentValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty().Length(6, 50)
            .Must(HasLowerCase).WithMessage("Must have one lower case char.")
            .Must(HasUpperCase).WithMessage("Must have one upper case char.")
            .Must(HasDigit).WithMessage("Must have one digit.")
            .Must(HasSpecialChar).WithMessage("Must have one special char like \"!@#$%^&*?_~-£().,\".");
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().Equal(x => x.NewPassword).WithMessage("\"Confirm Password\" måste vara lika med \"New Password\"");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ChangePasswordDto>.CreateWithOptions((ChangePasswordDto)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };

    private bool HasLowerCase(string password)
    {
        return password.Any(char.IsLower);
    }

    private bool HasUpperCase(string password) 
    { 
        return password.Any(char.IsUpper); 
    }

    private bool HasDigit(string password)
    {
        return password.Any(char.IsDigit); 
    }

    private bool HasSpecialChar(string password)
    {
        return password.IndexOfAny("!@#$%^&*?_~-£().,".ToCharArray()) != -1;
    }
}
