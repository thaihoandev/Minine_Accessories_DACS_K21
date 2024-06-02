namespace Accessories_Store.Services
{
    public interface IMyEmailSender
    {
        void SendEmail(string email, string subject, string HtmlMessage);
    }
}
