namespace ResidenceStore.Mailer
{
    using System;

    public interface IResidenceStoreMailer
    {
        void SendVerificationMail(string email, string verificationToken, Func<string, string> linkGenerator);
    }
}