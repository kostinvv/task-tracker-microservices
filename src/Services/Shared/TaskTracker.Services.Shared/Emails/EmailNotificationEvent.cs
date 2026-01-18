namespace TaskTracker.Services.Shared.Emails;

public record EmailNotificationEvent(
    string Email,
    string Subject,
    string Body)
{
    /// <summary>
    /// Создает новый экземпляр события уведомления по электронной почте с валидацией входных данных.
    /// </summary>
    /// <param name="email">Адрес электронной почты получателя.</param>
    /// <param name="subject">Тема письма.</param>
    /// <param name="body">Текст (тело) письма.</param>
    /// <returns>Новый экземпляр <see cref="EmailNotificationEvent"/>.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если любой из параметров равен null, пуст или состоит только из пробелов.</exception>
    public static EmailNotificationEvent Create(
        string email, 
        string subject,
        string body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        
        return new EmailNotificationEvent(email, subject, body);
    }
}