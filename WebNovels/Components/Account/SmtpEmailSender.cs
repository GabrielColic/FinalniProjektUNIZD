using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebNovels.Models;

namespace WebNovels.Components.Account
{
    internal sealed class SmtpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IConfiguration _config;
        public SmtpEmailSender(IConfiguration config) => _config = config;

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
            => SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by <a href=\"{confirmationLink}\">clicking here</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
            => SendEmailAsync(email, "Reset your password",
                $"Reset your password by <a href=\"{resetLink}\">clicking here</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
            => SendEmailAsync(email, "Reset code",
                $"Use this code to reset your password: {System.Net.WebUtility.HtmlEncode(resetCode)}");

        private async Task SendEmailAsync(string to, string subject, string html)
        {
            var s = _config.GetSection("Email:Smtp");
            using var client = new SmtpClient(s["Host"], int.Parse(s["Port"] ?? "587"))
            {
                EnableSsl = bool.Parse(s["EnableSsl"] ?? "true"),
                Credentials = new NetworkCredential(s["User"], s["Password"])
            };

            var from = s["From"] ?? s["User"];
            using var msg = new MailMessage(from!, to, subject, html) { IsBodyHtml = true };
            await client.SendMailAsync(msg);
        }
    }
}
