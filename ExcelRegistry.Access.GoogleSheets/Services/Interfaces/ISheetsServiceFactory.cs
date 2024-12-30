using Google.Apis.Sheets.v4;

namespace ExcelRegistry.Access.GoogleSheets.Services.Interfaces;

public interface ISheetsServiceFactory
{
    SheetsService Create();
}