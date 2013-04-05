namespace ResidenceStore.Tests
{
    using System;
    using System.Linq;
    using ResidenceStore;
    using Xunit;

    public class InMemoryResidenceStoreTests
    {
        [Fact]
        public void Store_shouldnothave_residences_when_created()
        {
            var store = new InMemoryResidenceStore();
            Assert.Equal(0, store.Count);
        }

        [Fact]
        public void ResidenceForEmail_returns_residenceinfo_with_correct_fields_after_GenerateVerificationToken()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);

            var info = store.ResidencesForEmail(email).Single();
            Assert.Equal(email, info.Email);
            Assert.Equal(residence, info.Residence);
            Assert.False(info.Verified);
            Assert.Equal(token, info.VerificationToken);
            Assert.Null(info.AuthorizationToken);
        }

        [Fact]
        public void Emails_should_contain_email_after_GenerateVerificationToken()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            store.GenerateVerificationToken(email, residence, null, null);

            Assert.Equal(1, store.ResidencesForEmail(email).Count);
        }

        [Fact]
        public void GenerateVerificationToken_called_twice_should_reset_verification_token()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var first = store.GenerateVerificationToken(email, residence, null, null);
            var second = store.GenerateVerificationToken(email, residence, null, null);

            Assert.NotEqual(first, second);
        }

        [Fact]
        public void GenerateVerificationToken_called_twice_should_not_created_new_residence()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            store.GenerateVerificationToken(email, residence, null, null);
            store.GenerateVerificationToken(email, residence, null, null);

            var info = store.ResidencesForEmail(email).Single();
            Assert.Equal(email, info.Email);
        }

        [Fact]
        public void ConfirmVerificationToken_should_return_info_with_correct_fields()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);

            var info = store.ConfirmVerificationToken(token);
            Assert.Equal(email, info.Email);
            Assert.Equal(residence, info.Residence);
            Assert.True(info.Verified);
            Assert.Equal(token, info.VerificationToken);
            Assert.Null(info.AuthorizationToken);
        }

        [Fact]
        public void ConfirmVerificationToken_with_invalid_token_should_return_null()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);

            var info = store.ConfirmVerificationToken(token + token);
            Assert.Null(info);
        }

        [Fact]
        public void Cannot_ConfirmVerificationToken_same_token_twice()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);

            var info = store.ConfirmVerificationToken(token);
            var info2 = store.ConfirmVerificationToken(token);
            Assert.Null(info2);
        }

        [Fact]
        public void Can_generate_authorizationtoken_after_verificationtoken_confirmed()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);
            store.ConfirmVerificationToken(token);

            var info = store.GenerateNewAuthorizationToken(token);
            Assert.NotNull(info.AuthorizationToken);
            Assert.NotEmpty(info.AuthorizationToken);
        }

        [Fact]
        public void Cannot_generate_authorizationtoken_before_verificationtoken_confirmed()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);

            var info = store.GenerateNewAuthorizationToken(token);
            Assert.Null(info);
        }

        [Fact]
        public void Calling_GenerateNewAuthorizationToken_with_confirmed_verificationtoken_clears_verificationtoken()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);
            store.ConfirmVerificationToken(token);

            var info = store.GenerateNewAuthorizationToken(token);
            Assert.Null(info.VerificationToken);
        }

        [Fact]
        public void Can_use_verificationtoken_only_once_to_GenerateNewAuthorizationToken()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);
            store.ConfirmVerificationToken(token);

            store.GenerateNewAuthorizationToken(token);
            var info = store.GenerateNewAuthorizationToken(token);
            Assert.Null(info);
        }

        [Fact]
        public void Calling_GenerateNewAuthorizationToken_with_authorizationtoken_renews_token()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);
            store.ConfirmVerificationToken(token);

            var info = store.GenerateNewAuthorizationToken(token);
            var info2 = store.GenerateNewAuthorizationToken(info.AuthorizationToken);

            Assert.NotNull(info2.AuthorizationToken);
            Assert.NotEmpty(info2.AuthorizationToken);
            Assert.NotEqual(info.AuthorizationToken, info2.AuthorizationToken);
        }

        [Fact]
        public void Can_use_token_for_GenerateNewAuthorizationToken_only_once()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);
            store.ConfirmVerificationToken(token);
            store.GenerateNewAuthorizationToken(token);

            var info = store.GenerateNewAuthorizationToken(token);
            var info2 = store.GenerateNewAuthorizationToken(token);

            Assert.Null(info);
        }

        [Fact]
        public void RevokeAuthorizationToken_removes_residence_info()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);
            store.ConfirmVerificationToken(token);
            token = store.GenerateNewAuthorizationToken(token).AuthorizationToken;

            store.RevokeAuthorizationToken(token);

            Assert.Equal(0, store.Count);
        }

        [Fact]
        public void RevokeAuthorizationToken_with_invalid_token_keep_residence_info()
        {
            const string email = "foo@bar.baz";
            var residence = Guid.NewGuid().ToString("N");

            var store = new InMemoryResidenceStore();
            var token = store.GenerateVerificationToken(email, residence, null, null);
            store.ConfirmVerificationToken(token);
            token = store.GenerateNewAuthorizationToken(token).AuthorizationToken;

            store.RevokeAuthorizationToken(token + token);

            Assert.Equal(1, store.Count);
        }
    }
}
