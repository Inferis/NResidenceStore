﻿namespace ResidenceStore.Web.Nancy
{
    using System.Linq;
    using Models;
    using ResidenceStore;
    using global::Nancy;

    public class ResidenceModule : NancyModule
    {
        public ResidenceModule(IResidenceStore residenceStore)
            : this("/residence", residenceStore)
        {
            
        }
        
        public ResidenceModule(string modulePath, IResidenceStore residenceStore)
            : base(modulePath)
        {
            Options["/"] = parameters => {
                var response = new Response() {
                    StatusCode = HttpStatusCode.OK,
                    Headers = { { "Allow", "POST,GET,DELETE,OPTIONS" } } 
                };
                return response;
            };

            // starts the flow. Post email and residence to the store
            Post["/"] = parameters => {
                string email = Request.Form.email;
                string residence = Request.Form.residence;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(residence))
                    return HttpStatusCode.BadRequest;

                var token = residenceStore.GenerateVerificationToken(email, residence);
                return Response.AsJson(new {
                    email = email,
                    verificationToken = token
                });
            };

            // link back from the email
            Get["/{verificationToken}"] = parameters => {
                var verificationToken = parameters.verificationToken;
                if (string.IsNullOrEmpty(verificationToken))
                    return HttpStatusCode.BadRequest;

                var residence = residenceStore.ConfirmVerificationToken(verificationToken);
                return GetVerificationResultView(residence);
            };

            // user asks if the residence has been verified
            // this wil generate a new authorization token
            Get["/"] = parameters => {
                var token = Request.Headers.Authorization;
                var residence = Request.Headers["X-Residence"].LastOrDefault();
                if (string.IsNullOrEmpty(token) && string.IsNullOrEmpty(residence))
                    return HttpStatusCode.BadRequest;

                var info = residenceStore.GenerateNewAuthorizationToken(token);
                if (info == null) {
                    return HttpStatusCode.Unauthorized;
                }

                return Response.AsJson(new {
                    email = info.Email,
                    authorizationToken = info.AuthorizationToken
                });
            };

            Delete["/{token}"] = parameters => {
                var token = parameters.token;
                var residence = Request.Headers["X-Residence"].LastOrDefault();
                if (string.IsNullOrEmpty(token) && string.IsNullOrEmpty(residence))
                    return HttpStatusCode.BadRequest;

                var info = residenceStore.ResidenceWithAuthorizationToken(token);
                if (info == null) {
                    return HttpStatusCode.Unauthorized;
                }

                residenceStore.RevokeAuthorizationToken(token);
                return HttpStatusCode.OK;
            };
        }

        protected virtual object GetVerificationResultView(ResidenceInfo residence)
        {
            return View["Residence/VerificationResult", new VerificationResultModel(residence)];
        }
    }
}