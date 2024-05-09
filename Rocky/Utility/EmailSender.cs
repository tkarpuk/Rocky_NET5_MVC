using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Threading.Tasks;

namespace Rocky.Utility
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // stub
            await Task.Run(() => Console.WriteLine($"Send message to {email}. Subject: {subject}."));
        }
    }
}
