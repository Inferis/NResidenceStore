namespace ResidenceStore.Web
{
    using System;
    using System.Collections.Generic;
    using Mailer;
    using System.Web;

    public class WebApplicationResidenceStore : ResidenceStoreBase
    {
        private readonly string applicationKey;

        public WebApplicationResidenceStore(IResidenceStoreMailer mailer)
            : this(null, mailer)
        {

        }

        public WebApplicationResidenceStore(string applicationKey, IResidenceStoreMailer mailer)
            : base(mailer)
        {
            this.applicationKey = string.IsNullOrEmpty(applicationKey) ? "WebApplicationResidenceStore" : applicationKey;
        }

        private void BackingStore(Action<ResidenceStoreBase> storeAccessor)
        {
            BackingStore(store => { storeAccessor(store); return 0; });
        }

        private T BackingStore<T>(Func<ResidenceStoreBase, T> storeAccessor)
        {
            HttpContext.Current.Application.Lock();
            try {
                var store = (HttpContext.Current.Application[applicationKey] as InMemoryResidenceStore) ?? new InMemoryResidenceStore(Mailer);
                var result = storeAccessor(store);
                HttpContext.Current.Application[applicationKey] = store;
                return result;
            }
            finally {
                HttpContext.Current.Application.UnLock();
            }
        }

        public override bool HasResidenceForEmail(string email)
        {
            return BackingStore(store => store.HasResidenceForEmail(email));
        }

        public override List<ResidenceInfo> ResidencesForEmail(string email)
        {
            return BackingStore(store => store.ResidencesForEmail(email));
        }

        public override void RemoveAllResidencesForEmail(string email)
        {
            BackingStore(store => store.RemoveAllResidencesForEmail(email));
        }

        public override int Count
        {
            get
            {
                return BackingStore(store => store.Count);
            }
        }

        protected internal override ResidenceInfo GetResidence(string email, string residence)
        {
            return BackingStore(store => store.GetResidence(email, residence));
        }

        protected internal override void PutResidence(ResidenceInfo residenceInfo)
        {
            BackingStore(store => store.PutResidence(residenceInfo));
        }

        protected internal override void RemoveResidence(ResidenceInfo residenceInfo)
        {
            BackingStore(store => store.RemoveResidence(residenceInfo));
        }

        protected internal override ResidenceInfo GetUnverifiedResidenceWithVerificationToken(string verificationToken)
        {
            return BackingStore(store => store.GetUnverifiedResidenceWithVerificationToken(verificationToken));
        }

        protected internal override ResidenceInfo GetVerifiedResidenceWithAnyToken(string token)
        {
            return BackingStore(store => store.GetVerifiedResidenceWithAnyToken(token));
        }
    }
}
