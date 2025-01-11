using Amazon.S3;
using Amazon.S3.Model;
using posting.Services.PostgresDBService;

namespace posting.Services.S3Service
{
    public class S3Service : IS3Service
    {
        #region vars        
        IAmazonS3 client;
        string? url, user, password, bucket = string.Empty;
        #endregion
        public S3Service(IConfiguration config)
        {
            url = config["S3:ServiceURL"];
            user = config["S3:user"];
            password = config["S3:password"];

            client = new AmazonS3Client(

               user,
               password,

                new AmazonS3Config()
                {
                    ServiceURL = url,
                    ForcePathStyle = true
                }
            );
        }

        #region helpers
        string getExtensionFromMimeType(string input)
        {
            var res = input;
            var index = input.IndexOf('/');
            if (index >= 0)
                res = input.Substring(index + 1);
            return res;
        }
        #endregion

        #region public
        public async Task<S3ItemInfo> Upload(byte[] bytes, string extension)
        {
            string key = $"{Guid.NewGuid()}.{extension}";

            try
            {

                using (var stream = new MemoryStream(bytes))
                {
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = bucket,
                        Key = key,
                        InputStream = stream
                    };

                    var response = await client.PutObjectAsync(putRequest);
                }

                var request = new GetPreSignedUrlRequest()
                {
                    BucketName = bucket,
                    Key = key,
                    Expires = DateTime.UtcNow.AddYears(1)
                };

                var url = await client.GetPreSignedURLAsync(request);

                return new S3ItemInfo()
                {
                    storage_id = key,
                    extension = extension,
                    url = url
                };

            }
            catch (Exception ex)
            {
                throw new Exception($"S3 Upload error: {ex.Message}");
            }
        }

        public async Task Delete(string storage_id)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucket,
                    Key = storage_id
                };

                var response = await client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch (Exception ex)
            {
                throw new Exception($"S3 delete error: {ex.Message}");
            }
        }


        public async Task<(byte[], S3ItemInfo)> Download(string storage_id)
        {
            byte[] bytes = new byte[0];
            S3ItemInfo s3info = new S3ItemInfo();

            try
            {
                var getRequest = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = storage_id
                };

                using (var response = await client.GetObjectAsync(getRequest))
                using (var responseStream = response.ResponseStream)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await responseStream.CopyToAsync(memoryStream);
                        bytes = memoryStream.ToArray();
                    }

                    s3info.extension = getExtensionFromMimeType(response.Headers.ContentType);
                }

                var request = new GetPreSignedUrlRequest()
                {
                    BucketName = bucket,
                    Key = storage_id,
                    Expires = DateTime.UtcNow.AddYears(1)
                };

                var url = await client.GetPreSignedURLAsync(request);

                s3info.storage_id = storage_id;
                s3info.url = url;

                return (bytes, s3info);

            }
            catch (Exception ex)
            {
                throw new Exception($"S3 Download error: {ex.Message}");
            }
        }

        public async Task<S3ItemInfo> GetInfo(string storage_id)
        {
            S3ItemInfo s3info = new S3ItemInfo();

            try
            {
                var getRequest = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = storage_id
                };

                using (var response = await client.GetObjectAsync(getRequest))
                {
                    s3info.extension = getExtensionFromMimeType(response.Headers.ContentType);
                }


                var request = new GetPreSignedUrlRequest()
                {
                    BucketName = bucket,
                    Key = storage_id,
                    Expires = DateTime.UtcNow.AddYears(1)
                };

                var url = await client.GetPreSignedURLAsync(request);

                s3info.storage_id = storage_id;
                s3info.url = url;

                return s3info;

            }
            catch (Exception ex)
            {
                throw new Exception($"S3 Download error: {ex.Message}");
            }
        }

        public async Task<bool> CheckExists(string storage_id)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucket,
                    Key = storage_id
                };
                var response = await client.GetObjectMetadataAsync(request);
                return true; // If the call succeeds, the file exists
            }
            catch (AmazonS3Exception e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false; // The file does not exist
                                  // Rethrow the exception if it's not a Not Found exception
                throw;
            }
        }
        #endregion
    }
}
