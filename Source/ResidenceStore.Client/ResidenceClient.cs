namespace ResidenceStore.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    public class ResidenceClient
    {
        private string verifier;

        public ResidenceClient(string verifier)
        {
            if (string.IsNullOrEmpty(verifier))
                throw new ArgumentNullException("verifier");
            if (!verifier.EndsWith("/"))
                verifier += "/";
            this.verifier = verifier;
        }

        public List<string> Emails
        {
            get { return null; }
        }

        public void RegisterResidenceForEmail(string email, Action<object> completion)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");

            //NSString* residenceId;
            //@synchronized(_key) {
            //    NSDictionary* residence = [self residenceForEmail:email];
            //    if (!residence) {
            //        residenceId = [self generateTokenForEmail:email];
            //        residence = @{ @"email": email, @"residence": residenceId };
            //        [self updateResidence:residence];
            //    }
            //    else {
            //        residenceId = residence[@"residence"];
            //    }
            //}

            //string residenceId;
            //var residence = ResidenceForEmail(email);
            //if (residence == null) {
            //    residenceId = GenerateTokenForEmail();
            //    residence = 
            //}

            //Post(verifier, null, new { email, residence }, (statusCode, json) => {
            //    if (statusCode/100 != 2) {
            //        // error   
            //        completion(false);
            //    }
            //});

            //var request = WebRequest.CreateHttp(verifier);
            //request.Accept = "application/json";
            //request.Method = "POST";
            //request.BeginGetRequestStream(r => {
            //    using (var writer = new StreamWriter(request.EndGetRequestStream(r))) {
            //        writer.Write("email=");
            //        writer.Write(email);
            //        writer.Write("&residence=");
            //        writer.Write(residence);
            //    }
            //}, null);

            //request.BeginGetResponse(r => {
            //    var response = request.EndGetResponse(r);


            //}, null);
        }

#if WP7
        private void Post(string url, object headers, object query, Action<int, object> callback)
        {
        }
#else
        private void Post(string url, object headers, object query, Action<int, dynamic> callback)
        {
        }
#endif


    }
}

/*

- (NSString*)generateTokenForEmail:(NSString*)email {
if (email.length == 0)
    return nil;
    
NSString* digest = [self uniqueIdentifierForEmail:email];
    
uint8_t sha1hash[CC_SHA1_DIGEST_LENGTH];
NSData* encoded = [digest dataUsingEncoding:NSUTF8StringEncoding];
    
if (!CC_SHA1(encoded.bytes, encoded.length, sha1hash)) {
    return nil;
}
    
NSMutableString* hex = [NSMutableString stringWithCapacity:CC_SHA1_DIGEST_LENGTH * 2];
for(int i=0; i<CC_SHA1_DIGEST_LENGTH; i++){
    [hex appendFormat:@"%02X", sha1hash[i]];
}
return hex;
}
- (NSArray*)allEmails;
- (BOOL)isEmailRegistered:(NSString*)email;
- (NSString*)residenceTokenForEmail:(NSString*)email;

- (BOOL)removeAllResidences;

- (void)registerResidenceForEmail:(NSString*)email completion:(void(^)(BOOL success, NSError* error))completion;
- (void)registerResidenceForEmail:(NSString*)email userInfo:(NSDictionary*)userInfo completion:(void(^)(BOOL success, NSError* error))completion;
- (void)verifyResidenceForEmail:(NSString*)email completion:(void(^)(BOOL success, NSError* error))completion;
- (void)removeResidenceForEmail:(NSString*)email completion:(void(^)(BOOL success, NSError* error))completion;

- (NSString*)uniqueIdentifierForEmail:(NSString*)email;

+ (IIResidenceStore*)storeWithVerifier:(NSString*)verifier;

*/