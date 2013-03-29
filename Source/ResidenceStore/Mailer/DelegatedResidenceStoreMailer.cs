namespace ResidenceStore.Mailer
{
    using System;

    public class DelegatedResidenceStoreMailer : IResidenceStoreMailer
    {
        private readonly Action<string, string> handler;

        public DelegatedResidenceStoreMailer(Action<string, string> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            this.handler = handler;
        }

        public void SendVerificationMail(string email, string verificationToken)
        {
            handler(email, verificationToken);
        }
    }
}