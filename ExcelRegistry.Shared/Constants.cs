namespace ExcelRegistry.Shared;

public class Constants
{
    public class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

    public class JwtToken
    {
        public const int DefaultExpirationInHours = 24;
    }

    public class GoogleSheets
    {
        public const int DefaultRangeColumnStart = 0;
        public const int DefaultRangeRowStart = 1;
        public const int DefaultRangeColumnEnd = 0;
        public const int DefaultRangeRowEnd = 9999;
    }

    public class Databases
    {
        public const string ExcelRegistryUsersDb = "ExcelRegistryUsers";
    }

    public class ExcelRegistryUsersDbTables
    {
        public const string Users = "Users";
        public const string Roles = "Roles";
    }

    public class RegentSheet
    {
        public const string SheetName = "Regentai";
        public const int RageColumnEndForRegents = 6;
    }
}
