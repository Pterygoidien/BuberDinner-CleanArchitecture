using ErrorOr;
using Microsoft.AspNetCore.Mvc;

using BuberDinner.Application.Common.Errors;
using BuberDinner.Application.Services.Authentication;
using BuberDinner.Contracts.Authentication;
using BuberDinner.Domain.Common.Errors;

namespace BuberDinner.Api.Controllers;


[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        var (FirstName, LastName, Email, Password) = request;
        ErrorOr<AuthenticationResult> authResult = _authenticationService.Register(
            FirstName,
            LastName,
            Email,
            Password
        );

        return authResult.Match<IActionResult>(
            authResult => Ok(MapAuthResult(authResult)),
            errors => Problem(errors)
        );


    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var (Email, Password) = request;
        ErrorOr<AuthenticationResult> authResult = _authenticationService.Login(Email, Password);

        if (authResult.IsError && authResult.FirstError == Errors.Authentication.InvalidCredentials)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: authResult.FirstError.Description
            );
        }

        return authResult.Match<IActionResult>(
           authResult => Ok(MapAuthResult(authResult)),
           errors => Problem(errors)
       );

    }

    private static AuthenticationResponse MapAuthResult(AuthenticationResult authResult)
    {
        return new AuthenticationResponse(
        authResult.User.Id,
        authResult.User.FirstName,
        authResult.User.LastName,
        authResult.User.Email,
        authResult.Token
        );
    }
}
