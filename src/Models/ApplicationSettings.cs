namespace Linn.Common.Service.Models
{
    using System.Collections.Generic;

    using Linn.Common.Configuration;

    public class ApplicationSettings
    {
        public Dictionary<string, string> Settings { get; } = new Dictionary<string, string>();

        public static ApplicationSettings GetDefaults()
        {
            var appSettings = new ApplicationSettings();
            appSettings.Settings["cognitoHost"] = ConfigurationManager.Configuration["COGNITO_HOST"];
            appSettings.Settings["appRoot"] = ConfigurationManager.Configuration["APP_ROOT"];
            appSettings.Settings["proxyRoot"] = ConfigurationManager.Configuration["PROXY_ROOT"];
            appSettings.Settings["cognitoClientId"] = ConfigurationManager.Configuration["COGNITO_CLIENT_ID"];
            appSettings.Settings["cognitoDomainPrefix"] = ConfigurationManager.Configuration["COGNITO_DOMAIN_PREFIX"];
            appSettings.Settings["entraLogoutUri"] = ConfigurationManager.Configuration["ENTRA_LOGOUT_URI"];
            return appSettings;
        }
    }
}
