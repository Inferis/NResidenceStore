namespace ResidenceStore.Mailer
{
    public class DiscardingResidenceStoreMailer : IResidenceStoreMailer
    {
        public void SendVerificationMail(string email, string verificationToken)
        {
        }
    }
}