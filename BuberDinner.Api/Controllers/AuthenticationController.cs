using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using BuberDinner.Contracts.Authentication;
using BuberDinner.Domain.Common.Errors;
using BuberDinner.Application.Services.Authentication.Common;
using BuberDinner.Application.Services.Authentication.Commands;
using BuberDinner.Application.Services.Authentication.Queries;


namespace BuberDinner.Api.Controllers;


[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly IAuthenticationCommandService _authenticationCommandService;
    private readonly IAuthenticationQueryService _authenticationQueryService;


    public AuthenticationController(IAuthenticationCommandService authenticationCommandService,
        IAuthenticationQueryService authenticationQueryService)
    {
        _authenticationQueryService = authenticationQueryService;
        _authenticationCommandService = authenticationCommandService;
    }


    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        var (FirstName, LastName, Email, Password) = request;
        ErrorOr<AuthenticationResult> authResult = _authenticationCommandService.Register(
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
        ErrorOr<AuthenticationResult> authResult = _authenticationQueryService.Login(Email, Password);

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
