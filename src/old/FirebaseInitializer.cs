using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace servartur;

public static class FirebaseInitializer
{
    public static void Initialize()
    {
        var credential = GoogleCredential.FromFile("./firebase_private_key.json");
        FirebaseApp.Create(new AppOptions
        {
            Credential = credential,
        });
    }
}