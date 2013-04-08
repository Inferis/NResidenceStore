namespace ResidenceStore.Web.Mvc.Controllers
{
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Models;

    public abstract class ResidenceVerifierController : Controller
    {
        private readonly IResidenceStore residenceStore;

        protected ResidenceVerifierController(IResidenceStore residenceStore)
        {
            this.residenceStore = residenceStore;
        }

        protected IResidenceStore ResidenceStore { get { return residenceStore; } }

        [HttpOptions]
        public ActionResult Options()
        {
            Response.Headers["Allow"] = "POST,GET,DELETE,OPTIONS";
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        // starts the flow. Post email and residence to the residenceStore
        [HttpPost]
        public ActionResult Register(string email, string residence, string userinfo)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(residence))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var token = residenceStore.GenerateVerificationToken(email, residence, userinfo, GenerateVerificationLink);
            OnResidenceRegistered(new ResidenceInfo(email, residence, userinfo) { VerificationToken = token });
            return Json(new {
                email = email,
                verificationToken = token
            });
        }

        [HttpGet]
        public ActionResult Verify()
        {
            // user asks if the residence has been verified
            // this wil generate a new authorization token
            var authorization = Request.Headers["Authorization"];
            string token = null;
            if (authorization.StartsWith("Token")) {
                var match = Regex.Match(authorization, @"token=(['""])(.*?)\1");
                if (match.Success)
                    token = match.Groups[2].Value;
            }
            else {
                token = authorization;
            }
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

        [HttpGet]
        public ActionResult Confirm(string token)
        {
            if (string.IsNullOrEmpty(token)) 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // link back from the email
            var residence = residenceStore.ConfirmVerificationToken(token);
            if (residence != null) {
                OnResidenceConfirmed(residence);
            }

            return GetVerificationResultView(residence);
        }

        [HttpDelete]
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
            return Url.Action("Confirm", null, new { token }, Request.Url.Scheme);
        }

        protected virtual void OnResidenceRegistered(ResidenceInfo residence) { }
        protected abstract void OnResidenceConfirmed(ResidenceInfo residence);
        protected virtual void OnResidenceReauthorized(ResidenceInfo residence) { }
        protected abstract void OnResidenceDeleted(ResidenceInfo residence);
    }
}
