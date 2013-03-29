namespace ResidenceStore
{
    public class ResidenceInfo
    {
        public ResidenceInfo()
        {
        }

        public ResidenceInfo(string email, string residence)
        {
            Email = email;
            Residence = residence;
        }

        public ResidenceInfo(ResidenceInfo residenceInfo)
        {
            Email = residenceInfo.Email;
            Residence = residenceInfo.Residence;
            AuthorizationToken = residenceInfo.AuthorizationToken;
            VerificationToken = residenceInfo.VerificationToken;
            Verified = residenceInfo.Verified;
        }

        public string Email { get; set; }
        public string Residence { get; set; }
        public string VerificationToken { get; set; }
        public string AuthorizationToken { get; set; }
        public bool Verified { get; set; }
    }
}