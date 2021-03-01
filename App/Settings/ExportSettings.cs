namespace App.Settings
{
    public class ExportSettings
    {
        public string SqlServerName { get; set; }

        public string SqlDatabaseName { get; set; }

        public string BacPacFilePath1 { get; set; }

        public string BacPacFilePath2 { get; set; }

        public AzureSettings AzureSettings { get; set; }
    }
}
