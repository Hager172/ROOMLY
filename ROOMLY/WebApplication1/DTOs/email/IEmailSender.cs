namespace ROOMLY.DTOs.email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);

    }
}
