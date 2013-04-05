namespace $rootnamespace$
{
    using System;
    using System.Net.Mail;
    using ResidenceStore.Mailer;

    public class ResidenceStoreMailer : IResidenceStoreMailer
    {
        public void SendVerificationMail(string email, string verificationToken, Func<string, string> linkGenerator)
        {
            var sender = new MailAddress("noreply@yoursite.net", "Your Site Name");
            var message = new MailMessage() {
                Sender = sender,
                From = sender,
                Subject = "Residence Verification needed",
                Body = "Please verify your residence at " + linkGenerator(verificationToken)
            };
            message.To.Add(new MailAddress(email));
            message.ReplyToList.Add(sender);
            new SmtpClient().Send(message);
        }
    }
}