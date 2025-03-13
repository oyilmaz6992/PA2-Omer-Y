using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


public class MailService
{
    private readonly IConfiguration _configuration;

    
    //dependency injection for configuration settings.

    /// <param name="configuration">Application configuration settings.</param>
    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //sends email with recipeint subject and content
    /// <param name="recipientEmail">The email address of the recipient.</param>
    /// <param name="emailSubject">The subject of the email.</param>
    /// <param name="emailBody">The body content of the email (HTML enabled).</param>
    public async Task SendEmailAsync(string recipientEmail, string emailSubject, string emailBody)
    {
        //retrieve sender credentials 
        string senderEmail = _configuration["EmailSettings:Email"];
        string senderPassword = _configuration["EmailSettings:Password"];

        //configure SMTP client settings
        using var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            EnableSsl = true,
        };

        //the message
        using var emailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail),
            Subject = emailSubject,
            Body = emailBody,
            IsBodyHtml = true, // Enables HTML formatting in the email body
        };

        //adds ecipient to the message
        emailMessage.To.Add(recipientEmail);

        //send email 
        await smtpClient.SendMailAsync(emailMessage);
    }
}
