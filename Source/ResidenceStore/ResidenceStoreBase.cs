namespace ResidenceStore
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using Mailer;

    public abstract class ResidenceStoreBase : IResidenceStore
    {
        protected ResidenceStoreBase(IResidenceStoreMailer mailer)
        {
            if (mailer == null)
                throw new ArgumentNullException("mailer");

            Mailer = mailer;
        }

        public IResidenceStoreMailer Mailer { get; private set; }

        public string GenerateVerificationToken(string email, string residence, string userInfo)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(residence))
                return null;

            // find existing
            email = email.Trim();
            residence = residence.Trim();
            var residenceInfo = GetResidence(email, residence) ?? new ResidenceInfo(email, residence, userInfo);

            // generate verificiationtoken
            var token = GenerateToken(email + residence);
            residenceInfo.VerificationToken = token;
            residenceInfo.Verified = false;
            residenceInfo.AuthorizationToken = null;
            PutResidence(residenceInfo);

            if (Mailer != null)
                Mailer.SendVerificationMail(email, token);

            return token;
        }


        public ResidenceInfo ConfirmVerificationToken(string verificationToken)
        {
            if (string.IsNullOrEmpty(verificationToken))
                return null;

            // find existing
            verificationToken = verificationToken.Trim();
            var residenceInfo = GetUnverifiedResidenceWithVerificationToken(verificationToken);
            if (residenceInfo == null)
                return null;

            residenceInfo.Verified = true;
            PutResidence(residenceInfo);

            return residenceInfo;
        }

        public ResidenceInfo GenerateNewAuthorizationToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            token = token.Trim();
            var residenceInfo = GetVerifiedResidenceWithAnyToken(token);
            if (residenceInfo == null)
                return null;

            token = GenerateToken(residenceInfo.Email + residenceInfo.Residence + token);
            residenceInfo.VerificationToken = null;
            residenceInfo.AuthorizationToken = token;
            PutResidence(residenceInfo);

            return residenceInfo;
        }


        public ResidenceInfo ResidenceWithAuthorizationToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            token = token.Trim();
            var residence = GetVerifiedResidenceWithAnyToken(token);
            return residence != null && residence.AuthorizationToken == token ? residence : null;
        }

        public void RevokeAuthorizationToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return;

            token = token.Trim();
            var residenceInfo = GetVerifiedResidenceWithAnyToken(token);
            if (residenceInfo != null)
                RemoveResidence(residenceInfo);
        }

        public abstract List<ResidenceInfo> ResidencesForEmail(string email);
        public abstract int Count { get; }

        protected abstract ResidenceInfo GetResidence(string email, string residence);
        protected abstract void PutResidence(ResidenceInfo residenceInfo);
        protected abstract void RemoveResidence(ResidenceInfo residenceInfo);
        protected abstract ResidenceInfo GetUnverifiedResidenceWithVerificationToken(string verificationToken);
        protected abstract ResidenceInfo GetVerifiedResidenceWithAnyToken(string token);

        protected virtual string GenerateToken(string value)
        {
            value = value.Trim();
            value = value + DateTime.Now.Ticks.ToString();

            var salt = new byte[value.Length];
            new RNGCryptoServiceProvider().GetBytes(salt);
            var hash = new Rfc2898DeriveBytes(value, salt, 10000).GetBytes(20);
            return BitConverter.ToString(hash).Replace("-", "");
        }

    }
}