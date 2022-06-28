using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.ComponentModel.DataAnnotations;
using mail = System.Net.Mail;
using System.Threading.Tasks;

namespace Petlance
{
    class EmailService
    {
        const string email = "petlance.official@gmail.com";
        const string password = "nliifakqylgdpyrh";

        public static string Code { get; protected set; }

        public static async Task SendVerification(User user, string text)
        {
            GenerateCode();
            MimeMessage emailMessage = new MimeMessage()
            {
                Subject = "Email verification",
                Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $"Hello, {user.Name}<br><br>"
                    + text
                    + $"<br><br><h1 align=\"center\"><b>{Code}</b></h1>"
                    + "<br><br>Kind regards, Petlance"
                }
            };
            emailMessage.From.Add(new MailboxAddress("Petlance", email));
            emailMessage.To.Add(new MailboxAddress("", user.Email));

            await Send(emailMessage);
        }

        private static void GenerateCode()
        {
            Random random = new Random();
            Code = "";
            for (int i = 0; i < 4; i++)
                Code += random.Next(0, 3) switch
                {
                    0 => (char)random.Next(65, 91),
                    1 => (char)random.Next(97, 123),
                    _ => (char)random.Next(0, 10),
                };
        }

        //public static async Task ForgotPassword(User user)
        //{
        //    Random random = new Random();
        //    Code = "";
        //    for (int i = 0; i < 4; i++)
        //        switch (random.Next(0, 3))
        //        {
        //            case 0:
        //                Code += (char)random.Next(65, 91);
        //                break;
        //            case 1:
        //                Code += (char)random.Next(97, 123);
        //                break;
        //            default:
        //                Code += random.Next(0, 10);
        //                break;
        //        }
        //    MimeMessage emailMessage = new MimeMessage()
        //    {
        //        Subject = "Email verification",
        //        Body = new TextPart(MimeKit.Text.TextFormat.Html)
        //        {
        //            Text = $"Hello {user.Name}<br><br>"
        //            + "You registered an account on Petlance, before being able to use your account you need to verify that this is your email address by entering this code:"
        //            + $"<br><br><h1 align=\"center\"><b>{Code}</b></h1><br><br>"
        //            + "Kind regards, Petlance"
        //        }
        //    };
        //    emailMessage.From.Add(new MailboxAddress("Petlance", email));
        //    emailMessage.To.Add(new MailboxAddress("", user.Email));

        //    using SmtpClient client = new SmtpClient();
        //    await client.ConnectAsync("smtp.gmail.com", 587, false);
        //    await client.AuthenticateAsync(email, password);
        //    await client.SendAsync(emailMessage);
        //    await client.DisconnectAsync(true);
        //}
        public static bool CheckEmail(string email)
        {
            var valid = true;

            try
            {
                var emailAddress = new mail.MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }
        public static async Task SendEmail(User user, string subject, string text)
        {
            MimeMessage emailMessage = new MimeMessage()
            {
                Subject = subject,
                Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $"Hello, {user.Name}<br><br>"
                    + text
                    + "<br><br>Kind regards, Petlance"
                }
            };
            emailMessage.From.Add(new MailboxAddress("Petlance", email));
            emailMessage.To.Add(new MailboxAddress("", user.Email));

            await Send(emailMessage);

        }

        private static async Task Send(MimeMessage emailMessage)
        {
            using SmtpClient client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync(email, password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}