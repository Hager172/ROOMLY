namespace ROOMLY.DTOs.email
{
    public class NullEmailSendercs:IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
