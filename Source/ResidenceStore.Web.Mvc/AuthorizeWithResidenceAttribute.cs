namespace ResidenceStore.Web.Mvc
{
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using ResidenceStore;

    public class AuthorizeWithResidenceAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var header = filterContext.HttpContext.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(header) && header.StartsWith("Token ")) {
                var match = Regex.Match(header, @"token=(['""])(.*?)\1");
                string token = null;
                if (match.Success)
                    token = match.Groups[2].Value;

                if (!string.IsNullOrEmpty(token)) {
                    var provider = filterContext.Controller as IResidenceStoreProvider;
                    IResidenceStore store = null;
                    if (provider != null) store = provider.ResidenceStore;
                    if (store == null)
                        store = AuthorizeWithResidence.Store;

                    if (store != null) {
                        var residence = store.ResidenceWithAuthorizationToken(token);
                        if (residence != null) {
                            return; // OK
                        }
                    }
                }
            }

            base.OnAuthorization(filterContext);
        }

    }

}