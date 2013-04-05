namespace ResidenceStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mailer;

    public class InMemoryResidenceStore : ResidenceStoreBase
    {
        private readonly List<ResidenceInfo> residences = new List<ResidenceInfo>();

        public InMemoryResidenceStore() : this(new DiscardingResidenceStoreMailer())
        {
            
        }

        public InMemoryResidenceStore(IResidenceStoreMailer mailer)
            : base(mailer)
        {

        }

        protected internal override ResidenceInfo GetResidence(string email, string residence)
        {
            return residences.FirstOrDefault(r => string.Compare(r.Email, email, StringComparison.InvariantCultureIgnoreCase) == 0 && residence == r.Residence);
        }

        protected internal override void PutResidence(ResidenceInfo residenceInfo)
        {
            var old = GetResidence(residenceInfo.Email, residenceInfo.Residence);
            if (old != null)
                residences.Remove(old);
            residences.Add(new ResidenceInfo(residenceInfo));
        }

        protected internal override void RemoveResidence(ResidenceInfo residenceInfo)
        {
            residences.Remove(residenceInfo);
        }

        public override bool HasResidenceForEmail(string email)
        {
            return residences
                .Any(r => string.Compare(r.Email, email, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public override List<ResidenceInfo> ResidencesForEmail(string email)
        {
            return residences
                .Where(r => string.Compare(r.Email, email, StringComparison.InvariantCultureIgnoreCase) == 0)
                .ToList();
        }

        public override void RemoveAllResidencesForEmail(string email)
        {
            var all = ResidencesForEmail(email);
            foreach (var residenceInfo in all) {
                RemoveResidence(residenceInfo);
            }
        }

        public override int Count
        {
            get { return residences.Count; }
        }

        protected internal override ResidenceInfo GetVerifiedResidenceWithAnyToken(string token)
        {
            return residences.FirstOrDefault(r => r.Verified && (token == r.VerificationToken || token == r.AuthorizationToken));
        }

        protected internal override ResidenceInfo GetUnverifiedResidenceWithVerificationToken(string verificationToken)
        {
            return residences.FirstOrDefault(r => !r.Verified && verificationToken == r.VerificationToken);
        }
    }
}