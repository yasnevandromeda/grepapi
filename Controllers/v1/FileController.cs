using Amazon.S3;
using grepapi.Models;
using Microsoft.AspNetCore.Mvc;


namespace grepapi.Controllers
{
    [Route("api/v1/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly GrepContext _grepContext;

        public FileController(GrepContext context)
        {
            _grepContext = context;
        }

        [HttpPost()]
        public async Task<string> Post(List<IFormFile> files, [FromHeader] long CompanyId, [FromHeader] string Password, string ext)
        {
            CompanyController.CheckAccess(CompanyId, Password, _grepContext);

            var bucketName = "grepbucket";
            Guid fileKey = Guid.NewGuid();
            var uploadObjectKey = fileKey.ToString() + "." + ext;

            AmazonS3Config configsS3 = new AmazonS3Config
            {
                ServiceURL = "https://s3.yandexcloud.net",
            };
            AmazonS3Client s3client = new AmazonS3Client(configsS3);

            var file = files.First();

            MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);

            try
            {
                await s3client.PutObjectAsync(new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = uploadObjectKey,
                    ContentType = $"image/{ext}",
                    InputStream = ms
                });
            }
            catch (AmazonS3Exception ex)
            {
                throw; // Re-throw to be caught by outer try-catch
            }


            return fileKey.ToString();
        }

    }
}
