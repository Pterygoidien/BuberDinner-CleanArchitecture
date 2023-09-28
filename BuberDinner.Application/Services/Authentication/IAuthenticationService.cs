using ErrorOr;

namespace BuberDinner.Application.Services.Authentication;

public interface IAuthenticationService
{
    ErrorOr<AuthenticationResult> Login(string email, string password);

    ErrorOr<AuthenticationResult> Register(string firstname, string lastName, string email, string password);
}