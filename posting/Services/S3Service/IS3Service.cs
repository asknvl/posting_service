namespace posting.Services.S3Service
{
    public interface IS3Service
    {
        Task<S3ItemInfo> Upload(byte[] bytes, string extension);
        Task<(byte[], S3ItemInfo)> Download(string storage_id);
    }

    public class S3ItemInfo
    {
        public string? storage_id { get; set; } = null;
        public string? extension { get; set; } = null;
        public string? url { get; set; } = null;
    }
}
