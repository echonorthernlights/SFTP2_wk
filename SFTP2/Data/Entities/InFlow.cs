namespace SFTP2.Data.Entities
{
    public class InFlow
    {
        public string Id { get; set; }
        public string ServerAddress { get; set; }
        public string ArchivePath { get; set; }

        // Navigation property
        public OutFlow OutFlow { get; set; }

        // Foreign key property
        public string OutFlowId { get; set; }


    }
}
