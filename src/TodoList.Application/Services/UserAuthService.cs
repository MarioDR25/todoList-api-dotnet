using TodoList.Application.DTOs;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

public class UserAuthService(IUserRepository userRepository, IAuthService authService, int expirationHours) : IUserAuthService
{
    private readonly IUserRepository  _userRepository = userRepository;
    private readonly IAuthService _authService = authService;
    private readonly int _expirationHours = expirationHours;

    public async Task<AuthResponseDto> RegisterAsync(CreateUserDto createDto)
    {
        if (await _userRepository.EmailExistsAsync(createDto.Email))
            throw new InvalidOperationException($"The email '{createDto.Email}' is already registered.");

        if(await _userRepository.UserExistsAsync(createDto.Username))
             throw new InvalidOperationException($"The username '{createDto.Username}' is already in use.");


        var passwordHash = _authService.HashPassword(createDto.Password);

        var user = new User {
            Name = createDto.Name.ToLower().Trim(),
            Username = createDto.Username.ToLower().Trim(),
            Email = createDto.Email.ToLower().Trim(),
            PasswordHash = passwordHash
        };

        var created = await _userRepository.AddUserAsync(user);

        var token = _authService.GenerateToken(created.Id, created.Username, created.Email);

        return BuildAuthResponse(created, token);
    }



    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username.ToLower().Trim());

        if (user is null || !_authService.VerifyPassword(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        var token = _authService.GenerateToken(user.Id, user.Username, user.Email);

        return BuildAuthResponse(user, token);
    }



    private AuthResponseDto BuildAuthResponse(User user, string token) => new()
    {
        Token = token,
        Name = user.Name,
        Username = user.Username,
        Email = user.Email,
        UserId = user.Id,
        ExpiresAt = DateTime.UtcNow.AddHours(_expirationHours)
    };
}


