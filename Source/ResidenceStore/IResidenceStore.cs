namespace ResidenceStore
{
    using System.Collections.Generic;
    using Mailer;

    public interface IResidenceStore
    {
        IResidenceStoreMailer Mailer { get; }

        string GenerateVerificationToken(string email, string residence);
        ResidenceInfo ConfirmVerificationToken(string verificationToken);
        ResidenceInfo GenerateNewAuthorizationToken(string verificationToken);
        ResidenceInfo ResidenceWithAuthorizationToken(string token);
        void RevokeAuthorizationToken(string token);
        List<ResidenceInfo> ResidencesForEmail(string email);
    }
}