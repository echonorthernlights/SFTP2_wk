namespace SFTP2.Data.Entities
{
    public class OutFlow
    {
        public string Id { get; set; }
        public string ServerAddress { get; set; }
        public string RemotePath { get; set; }

        // Navigation property
        public InFlow InFlow { get; set; }

        // Foreign key property
        public string InFlowId { get; set; }
    }

}
