using ExcelRegistry.Access.GoogleSheets.Services.Interfaces;
using ExcelRegistry.Shared.Options;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Options;

namespace ExcelRegistry.Access.GoogleSheets.Services
{
    public class SheetsServiceFactory(IOptions<GoogleSheetsOptions> googleSheetsOptions) : ISheetsServiceFactory
    {
        private readonly static string[] Scopes = [SheetsService.Scope.Spreadsheets];

        public SheetsService Create() => new(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromJson(googleSheetsOptions.Value.JsonCredentials).CreateScoped(Scopes),
        });
    }
}
