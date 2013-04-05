namespace $rootnamespace$.Controllers
{
    using ResidenceStore;
    using ResidenceStore.Web;
    using ResidenceStore.Web.Mvc;

    public class ResidenceController : ResidenceVerifierController
    {
        /// <summary>
        /// Example residence store controller. You should probably change the used residence store implementation
        /// to fit your own needs.
        /// </summary>
        public ResidenceController()
            : base(new WebApplicationResidenceStore(new ResidenceStoreMailer()))
        {

        }

        protected override void OnResidenceConfirmed(ResidenceInfo residence)
        {
            // A new residence has been confirmed by the user (the link in the email was clicked)
            // You could create a new account in your own system here, or store the residence 
            // information in your DB for later use, for example
        }

        protected override void OnResidenceReauthorized(ResidenceInfo residence)
        {
            // A residence has been reauthorized. This happens when a previously
            // authorized residence checks verification status again, which will
            // result in a new AuthorizationToken to be generated.
            // Use this method to update your own bookkeeping if necessary.
        }

        protected override void OnResidenceDeleted(ResidenceInfo residence)
        {
            // Access to residence has been revoked. The residence info passed is no longer in the residence store.
            // Use this method to delete user info or update your own bookkeeping when applicable.
        }
    }
}