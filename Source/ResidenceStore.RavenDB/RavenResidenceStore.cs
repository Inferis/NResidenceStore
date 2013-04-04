namespace ResidenceStore.RavenDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Mailer;
    using Raven.Client;

    public class RavenResidenceStore : ResidenceStoreBase
    {
        private readonly IDocumentStore store;

        public RavenResidenceStore(IDocumentStore store, IResidenceStoreMailer mailer)
            : base(mailer)
        {
            this.store = store;
            store.Conventions.RegisterIdConvention<ResidenceInfo>((dbname, commands, residence) => "residence/" + string.Join("", residence.Email.Select(x => ((int)x).ToString("x2"))) + HttpUtility.UrlEncode(residence.Residence));
        }

        public override List<ResidenceInfo> ResidencesForEmail(string email)
        {
            using (var session = store.OpenSession()) {
                return session.Query<ResidenceInfo>().Where(r => r.Email == email).ToList();
            }
        }

        public override int Count
        {
            get
            {
                using (var session = store.OpenSession()) {
                    return session.Query<ResidenceInfo>().Count();
                }
            }
        }

        protected override ResidenceInfo GetResidence(string email, string residence)
        {
            using (var session = store.OpenSession()) {
                return session.Query<ResidenceInfo>().FirstOrDefault(r => r.Email == email && r.Residence == residence);
            }
        }

        private ResidenceInfo GetResidence(string email, string residence, IDocumentSession session)
        {
            IDisposable disposable = null;
            if (session == null) {
                disposable = session = store.OpenSession();
            }
            try {
                return session.Query<ResidenceInfo>().FirstOrDefault(r => r.Email == email && r.Residence == residence);
            }
            finally {
                if (disposable != null) disposable.Dispose(); ;                
            }
        }

        protected override void PutResidence(ResidenceInfo residenceInfo)
        {
            using (var session = store.OpenSession()) {
                var current = GetResidence(residenceInfo.Email, residenceInfo.Residence, session);
                if (current != null) {
                    current.VerificationToken = residenceInfo.VerificationToken;
                    current.Verified = residenceInfo.Verified;
                    current.AuthorizationToken = residenceInfo.AuthorizationToken;
                    current.UserInfo = residenceInfo.UserInfo;
                    session.Store(current);
                }
                else
                    session.Store(residenceInfo);
                session.SaveChanges();
            }
        }

        protected override void RemoveResidence(ResidenceInfo residenceInfo)
        {
            using (var session = store.OpenSession()) {
                var current = GetResidence(residenceInfo.Email, residenceInfo.Residence, session);
                if (current != null) {
                    session.Delete(current);
                    session.SaveChanges();
                }
            }

        }

        protected override ResidenceInfo GetUnverifiedResidenceWithVerificationToken(string verificationToken)
        {
            using (var session = store.OpenSession()) {
                return session.Query<ResidenceInfo>().FirstOrDefault(r => !r.Verified && r.VerificationToken == verificationToken);
            }
        }

        protected override ResidenceInfo GetVerifiedResidenceWithAnyToken(string token)
        {
            using (var session = store.OpenSession()) {
                return session.Query<ResidenceInfo>().FirstOrDefault(r => r.Verified && (r.VerificationToken == token || r.AuthorizationToken == token));
            }
        }
    }
}
