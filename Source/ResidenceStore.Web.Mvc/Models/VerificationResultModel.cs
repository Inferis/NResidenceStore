namespace ResidenceStore.Web.Mvc.Models
{
    using ResidenceStore;

    public class VerificationResultModel
    {
        public ResidenceInfo Residence { get; set; }

        public VerificationResultModel(ResidenceInfo residence)
        {
            Residence = residence;
        }

        public bool Success { get { return Residence != null; } }
    }
}