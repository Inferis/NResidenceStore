namespace ResidenceStore.Web.Mvc
{
    using System.Net;
    using System.Web.Mvc;
    using Models;

    public abstract class ResidenceVerifierController : Controller
    {
        private readonly IResidenceStore residenceStore;

        protected ResidenceVerifierController(IResidenceStore residenceStore)
        {
            this.residenceStore = residenceStore;
        }

        [HttpOptions, ActionName("Index")]
        public ActionResult Options()
        {
            Response.Headers["Allow"] = "POST,GET,DELETE,OPTIONS";
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        // starts the flow. Post email and residence to the residenceStore
        [HttpPost, ActionName("Index")]
        public ActionResult Register(string email, string residence, string userinfo)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(residence))
                return new HttpStatusCodeResult(HttpStatusCode.OK);

            var token = residenceStore.GenerateVerificationToken(email, residence, userinfo, GenerateVerificationLink);
            OnResidenceRegistered(new ResidenceInfo(email, residence, userinfo) { VerificationToken = token });
            return Json(new {
                email = email,
                verificationToken = token
            });
        }

        [HttpGet, ActionName("Index")]
        public ActionResult Verify(string token)
        {
            if (string.IsNullOrEmpty(token)) {
                // user asks if the residence has been verified
                // this wil generate a new authorization token
                token = Request.Headers["Authorization"];
                var residence = Request.Headers["X-Residence"];
                if (string.IsNullOrEmpty(token) && string.IsNullOrEmpty(residence))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var info = residenceStore.GenerateNewAuthorizationToken(token);
                if (info == null) {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }

                OnResidenceReauthorized(info);

                return Json(new {
                    email = info.Email,
                    authorizationToken = info.AuthorizationToken
                }, JsonRequestBehavior.AllowGet);
            }
            else {
                // link back from the email
                var residence = residenceStore.ConfirmVerificationToken(token);
                if (residence != null) {
                    OnResidenceConfirmed(residence);
                }

                return GetVerificationResultView(residence);
            }
        }

        [HttpDelete, ActionName("Index")]
        public ActionResult Delete(string token)
        {
            var residence = Request.Headers["X-Residence"];
            if (string.IsNullOrEmpty(token) && string.IsNullOrEmpty(residence))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var info = residenceStore.ResidenceWithAuthorizationToken(token);
            if (info == null) {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            residenceStore.RevokeAuthorizationToken(token);
            OnResidenceDeleted(info);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        protected virtual ViewResult GetVerificationResultView(ResidenceInfo residence)
        {
            return View("VerificationResult", new VerificationResultModel(residence));
        }

        protected virtual string GenerateVerificationLink(string token)
        {
            return Url.Action("Index", new { token });
        }

        protected virtual void OnResidenceRegistered(ResidenceInfo residence) { }
        protected abstract void OnResidenceConfirmed(ResidenceInfo residence);
        protected virtual void OnResidenceReauthorized(ResidenceInfo residence) { }
        protected abstract void OnResidenceDeleted(ResidenceInfo residence);
    }
}
