using Codice.Client.Common;

namespace Unity.PlasticSCM.Editor.Tool
{
    internal static class AuthToken
    {
        internal static string GetForServer(string server)
        {
            ServerProfile serverProfile = CmConnection.Get().
                GetProfileManager().GetProfileForServer(server);

            string authToken = serverProfile != null ?
                CmConnection.Get().
                    BuildWebApiTokenForCloudEditionForUser(
                        serverProfile.Server, 
                        serverProfile.GetSEIDWorkingMode(), 
                        serverProfile.SecurityConfig):
                CmConnection.Get().
                    BuildWebApiTokenForCloudEditionForUser(
                        server, 
                        ClientConfig.Get().GetSEIDWorkingMode(), 
                        ClientConfig.Get().GetSecurityConfig());

            if (string.IsNullOrEmpty(authToken))
            {
                authToken = CmConnection.Get().
                    BuildWebApiTokenForCloudEditionDefaultUser();
            }

            return authToken;
        }
    }
}