using System.Resources;
using TaskTracker.Services.Shared.Emails.Resources;

namespace TaskTracker.Services.Shared.Emails;

public static class EmailTemplateExtensions
{
    private const string BaseNamespace = "TaskTracker.Services.Shared.Emails.Resources.Templates";
    
    public static async Task<string?> GetRazorTemplateAsync(this EmailTemplate emailTemplate)
    {
        var assembly = typeof(EmailTemplateExtensions).Assembly;
        var resourceStream = assembly.GetManifestResourceStream(
            name: $"{BaseNamespace}.{emailTemplate}.cshtml");

        if (resourceStream == null)
        {
            throw new FileNotFoundException(
                $"Resource not found. {resourceStream}");
        }
        
        using var reader = new StreamReader(resourceStream);
        var template = await reader.ReadToEndAsync();
        return template;
    }

    public static string? GetSubjectFromResource(this EmailTemplate emailTemplate)
    {
        var resourceManager = new ResourceManager(typeof(EmailSubjects));
        var subject = resourceManager.GetString(name: emailTemplate.ToString());
        return subject;
    }
}