namespace ResidenceStore.Mailer
{
    using System;

    public class DiscardingResidenceStoreMailer : IResidenceStoreMailer
    {
        public void SendVerificationMail(string email, string verificationToken, Func<string, string> linkGenerator)
        {
        }
    }
}