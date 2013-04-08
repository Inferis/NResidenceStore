namespace ResidenceStore.Tests
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Mailer;
    using MvcContrib.TestHelper;
    using MvcContrib.TestHelper.Fakes;
    using Web.Mvc;
    using Web.Mvc.Controllers;
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
            RouteTable.Routes.MapResidenceRoutes(
                "Residence", // Route name
                "residence", // URL with parameters
                new { controller = "Residence" } // Parameter defaults
            );

            token = Guid.NewGuid().ToString("N");
        }

        [Fact]
        public void Route_GET_Root_MapsTo_Verify_WithNullToken()
        {
            "~/residence/".WithMethod(HttpVerbs.Get).ShouldMapTo<ResidenceController>(x => x.Verify());
        }

        [Fact]
        public void Route_GET_RootWithToken_MapsTo_Verify_WithToken()
        {
            ("~/residence/" + token).WithMethod(HttpVerbs.Get).ShouldMapTo<ResidenceController>(x => x.Confirm(token));
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


        [Fact]
        public void GenerateVerificationToken_Generates_CorrectLink()
        {
            string link = null;
            var store = new Mock<IResidenceStore>();

            store.Setup(x => x.GenerateVerificationToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<string, string>>())).Returns<string, string, string, Func<string, string>>((e, t, ui, lg) => {
                link = lg(t);
                return token;
            });

            var controller = new ResidenceController(store.Object);
            controller.SetFakeControllerContext();
            controller.Request.SetupRequestUrl("http://foo.bar/residence");
            controller.Register("foo@bar.baz", "1234", null);

            Assert.Equal("http://foo.bar/residence/" + token, link);
        }
    }
}
