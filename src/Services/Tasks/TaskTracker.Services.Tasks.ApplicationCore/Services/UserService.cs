using Microsoft.AspNetCore.Identity;
using TaskTracker.Services.Shared.Events.Users;
using TaskTracker.Services.Shared.Results;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Auth;
using TaskTracker.Services.Tasks.ApplicationCore.Errors;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Services;

public class UserService(
    IJwtProvider jwtProvider, 
    UserManager<ApplicationUser> userManager,
    IKafkaProducer<UserRegisteredEvent> kafkaProducer): IUserService
{
    public async Task<ResultT<AuthenticationResult>> RegisterAsync(
        string email, 
        string password, 
        CancellationToken cancellationToken)
    {
        ApplicationUser applicationUser = new()
        { 
            UserName = email,
            Email = email
        };
        
        var identityResult = await userManager.CreateAsync(user: applicationUser, password: password);
        
        if (!identityResult.Succeeded)
        {
            var description = identityResult.Errors.First().Description;
            return UserErrors.CreateFailure(description);
        }

        await kafkaProducer.ProduceAsync(
            new UserRegisteredEvent(UserId:  applicationUser.Id, Email: email), 
            cancellationToken);
        
        var jwtToken = jwtProvider.GenerateJwtToken(applicationUser);
        return new AuthenticationResult(jwtToken);
    }

    public async Task<ResultT<AuthenticationResult>> LoginAsync(
        string email, 
        string password, 
        CancellationToken cancellationToken)
    {
        var applicationUser = await userManager.FindByEmailAsync(email);
        
        if (applicationUser is null)
        {
            return UserErrors.NotFound(email);
        }
        
        var isPasswordValid = await userManager.CheckPasswordAsync(user: applicationUser, password);
        if (!isPasswordValid)
        {
            return UserErrors.InvalidCredentials;
        }
        
        var jwtToken = jwtProvider.GenerateJwtToken(applicationUser);
        return new AuthenticationResult(jwtToken);
    }
}