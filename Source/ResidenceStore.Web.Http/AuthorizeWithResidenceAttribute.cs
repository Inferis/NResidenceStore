namespace ResidenceStore.Web.Http
{
    using System.Text.RegularExpressions;
    using System.Web.Http;

    public class AuthorizeWithResidenceAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null &&
                actionContext.Request.Headers.Authorization.Scheme == "Token") {
                var header = actionContext.Request.Headers.Authorization.Parameter;
                var match = Regex.Match(header, @"token=(['""])(.*?)\1");
                string token = null;
                if (match.Success) 
                    token = match.Groups[2].Value;
    
                if (!string.IsNullOrEmpty(token)) {
                    var provider = actionContext.ControllerContext.Controller as IResidenceStoreProvider;
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

            base.OnAuthorization(actionContext);
        }
    }
}