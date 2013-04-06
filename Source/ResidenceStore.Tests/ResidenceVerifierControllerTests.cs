namespace ResidenceStore.Tests
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using MvcContrib.TestHelper;
    using MvcContrib.TestHelper.Fakes;
    using Web.Mvc;
    using Xunit;
    using Moq;


    public class ResidenceVerifierControllerTests
    {
        private string token;

        private class ResidenceController : ResidenceVerifierController
        {
            public ResidenceInfo RegisterResidence { get; set; }
            public ResidenceInfo ReauthorizedResidence { get; set; }
            public ResidenceInfo ConfirmedResidence { get; set; }
            public ResidenceInfo DeletedResidence { get; set; }

            public ResidenceController(IResidenceStore residenceStore)
                : base(residenceStore)
            {
                
            }

            protected override void OnResidenceRegistered(ResidenceInfo residence)
            {
                RegisterResidence = residence;
            }

            protected override void OnResidenceReauthorized(ResidenceInfo residence)
            {
                ReauthorizedResidence = residence;
            }

            protected override void OnResidenceConfirmed(ResidenceInfo residence)
            {
                ConfirmedResidence = residence;
            }

            protected override void OnResidenceDeleted(ResidenceInfo residence)
            {
                DeletedResidence = residence;
            }
        }

        public ResidenceVerifierControllerTests()
        {
            RouteTable.Routes.Clear();
            RouteTable.Routes.MapRoute(
                "Residence", // Route name
                "residence/{token}", // URL with parameters
                new { controller = "Residence", action = UrlParameter.Optional, token = UrlParameter.Optional } // Parameter defaults
            );

            token = Guid.NewGuid().ToString("N");
        }

        [Fact]
        public void Route_GET_Root_MapsTo_Verify_WithNullToken()
        {
            "~/residence/".WithMethod(HttpVerbs.Get).ShouldMapTo<ResidenceController>(x => x.Verify(null));
        }

        [Fact]
        public void Route_GET_RootWithToken_MapsTo_Verify_WithToken()
        {
            var r= ("~/residence/" + token).WithMethod(HttpVerbs.Get);
            var r2 = ("~/residence").WithMethod(HttpVerbs.Get);
            var r3 = ("~/residence/" + token).WithMethod(HttpVerbs.Delete);
            ("~/residence/" + token).WithMethod(HttpVerbs.Get).ShouldMapTo<ResidenceController>(x => x.Verify(token));
        }

        [Fact]
        public void Route_POST_RootWithToken_MapsTo_Register()
        {
            ("~/residence").WithMethod(HttpVerbs.Post).ShouldMapTo<ResidenceController>(x => x.Register(null, null, null));
        }

        [Fact]
        public void Route_DELETE_RootWithToken_MapsTo_Delete_WithToken()
        {
            ("~/residence/" + token).WithMethod(HttpVerbs.Delete).ShouldMapTo<ResidenceController>(x => x.Delete(token));
        }
    }
}
