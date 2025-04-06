using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ArticalProject.code;

public class EmailSender : IEmailSender
{
    // Asynchronous method to send an email
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Create a new SMTP client for sending emails
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            // Set the port number for the SMTP server
            Port = 587,
            // Set the credentials for the SMTP server
            Credentials = new NetworkCredential("isaac.melad.isaac@gmail.com", "gsztbymiboqnxpma"),
            // Enable SSL for secure email sending
            EnableSsl = true
        };

        // Send the email asynchronously
        await smtpClient.SendMailAsync("isaac.melad.isaac@gmail.com", email, subject, htmlMessage);
    }
}