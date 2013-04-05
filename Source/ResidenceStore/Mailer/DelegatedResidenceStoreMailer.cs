namespace ResidenceStore.Mailer
{
    using System;

    public class DelegatedResidenceStoreMailer : IResidenceStoreMailer
    {
        private readonly Action<string, string, string> handler;

        public DelegatedResidenceStoreMailer(Action<string, string, string> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            this.handler = handler;
        }

        public void SendVerificationMail(string email, string verificationToken, Func<string, string> linkGenerator)
        {
            handler(email, verificationToken, linkGenerator(verificationToken));
        }
    }
}