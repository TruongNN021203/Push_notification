using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;

namespace Laundry_Notification.ShareLibary.Helper
{
    public static class FirebaseInitializer
    {
        private static bool _isInitialized = false;
        private static readonly object _lock = new object();

        public static void Initialize(IConfiguration config)
        {
            if (_isInitialized) return;

            lock (_lock)
            {
                if (_isInitialized) return;

                var credentialPath = config["Firebase:CredentialPath"];
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(credentialPath)
                    });
                }

                _isInitialized = true;
            }
        }
    }
}
