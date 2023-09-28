using ErrorOr;

using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Entities;
using BuberDinner.Domain.Common.Errors;
using BuberDinner.Application.Services.Authentication.Common;

namespace BuberDinner.Application.Services.Authentication.Queries;

public class AuthenticationQueryService : IAuthenticationQueryService
{

    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticationQueryService(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;

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