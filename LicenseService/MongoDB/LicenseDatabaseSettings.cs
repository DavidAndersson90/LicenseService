namespace LicenseService.MongoDB
{
    public class LicenseDatabaseSettings : ILicenseDatabaseSettings
    {
        public string LicenseCollection { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
    }
}
