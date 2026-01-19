using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskTracker.Services.Shared.Data.Entities;
using TaskTracker.Services.Shared.Emails;
using TaskTracker.Services.Shared.Kafka;
using TaskTracker.Services.Shared.Models;
using TaskTracker.Services.Shared.Results;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Auth;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs;
using TaskTracker.Services.Tasks.ApplicationCore.Errors;

namespace TaskTracker.Services.Tasks.ApplicationCore.Services;

public class UserService(
    ILogger<UserService> logger,
    IJwtProvider jwtProvider, 
    UserManager<ApplicationUser> userManager,
    IKafkaProducer<EmailNotificationEvent> kafkaProducer,
    IEmailTemplateService emailTemplateService): IUserService
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

        logger.LogInformation(
            "Учетная запись пользователя создана в базе данных. UserId: {UserId}", 
            applicationUser.Id);
        
        try
        {
            var greetingMessage = await emailTemplateService.GetEmailBodyAsync(
                emailTemplate: EmailTemplate.Greeting,
                model: null);

            var emailNotificationEvent = EmailNotificationEvent.Create(
                email: email, 
                subject: EmailTemplate.Greeting.GetSubjectFromResource()!,
                body: greetingMessage!);
            
            await kafkaProducer.ProduceAsync(
                key: Guid.NewGuid().ToString(),
                message: emailNotificationEvent, 
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                exception: ex, 
                message: "Не удалось сформировать приветственное электронное письмо для пользователя {Email}",
                email);
        }
        
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