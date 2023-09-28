using ErrorOr;

using BuberDinner.Application.Common.Errors;
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Entities;
using BuberDinner.Domain.Common.Errors;

namespace BuberDinner.Application.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{

    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;

    }
    public ErrorOr<AuthenticationResult> Register(string firstName, string lastName, string email, string password)
    {
        // 1. Validate the user doesn't exist
        if (_userRepository.GetUserByEmail(email) is not null)
            return Errors.User.DuplicateEmail;

        //2. Create user (generate unique ID, and hash password) & Persist to DB
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        };

        _userRepository.Add(user);
        //create a JWT token
        var token = _jwtTokenGenerator.GenerateToken(user);


        return new AuthenticationResult(
            user,
            token
        );
    }

    public ErrorOr<AuthenticationResult> Login(string email, string password)
    {
        //1. Validate the user exists

        if (_userRepository.GetUserByEmail(email) is not User user)
            return Errors.Authentication.InvalidCredentials;

        //2. Validate the password is correct
        if (user.Password != password)
            return Errors.Authentication.InvalidCredentials;


        //3. Create JWT Token
        var token = _jwtTokenGenerator.GenerateToken(user);

        //4. Return AuthenticationResult
        return new AuthenticationResult(
            user,
            token
        );
    }

}