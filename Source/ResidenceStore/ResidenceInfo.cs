namespace ResidenceStore
{
    public class ResidenceInfo
    {
        public ResidenceInfo()
        {
        }

        public ResidenceInfo(string email, string residence, string userInfo)
        {
            Email = email;
            Residence = residence;
            UserInfo = userInfo;
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
        public string UserInfo { get; set; }
        public bool Verified { get; set; }
    }
}