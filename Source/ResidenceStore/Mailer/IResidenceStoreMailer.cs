namespace ResidenceStore.Mailer
{
    public interface IResidenceStoreMailer
    {
        void SendVerificationMail(string email, string verificationToken);
    }
}