using RazorEngine.Templating;

namespace TaskTracker.Services.Shared.Emails;

public class RazorEmailTemplateService(
    IRazorEngineService razorEngineService) : IEmailTemplateService
{
    public async Task<string?> GetEmailBodyAsync(EmailTemplate emailTemplate, object? model)
    {
        var template = await emailTemplate.GetRazorTemplateAsync();
        var templateKey = emailTemplate.ToString();

        if (razorEngineService.IsTemplateCached(templateKey, modelType: null))
        {
            return razorEngineService.Run(
                templateKey, modelType: null, model: model);
        }
        
        var result = razorEngineService.RunCompile(
            template, 
            name: emailTemplate.ToString(), 
            modelType: null, 
            model: model);
        
        return result;
    }
}