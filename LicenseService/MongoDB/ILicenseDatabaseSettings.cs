namespace LicenseService.MongoDB
{
    public interface ILicenseDatabaseSettings
    {
        string LicenseCollection { get; set; }
        string ConnectionString { get; set; }
        string Database { get; set; }
    }
}


