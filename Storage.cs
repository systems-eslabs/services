using System;
using System.IO;
using Google.Apis.Storage.v1;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;

namespace eslabs_storage
{
    public class Storage
    {
        static readonly string[] StorageScope = { StorageService.Scope.DevstorageReadWrite };

        public StorageResposne BinaryUpload(string fileName, string bucketName, byte[] data, bool isSaveLocal)
        {
            string path = RetrunFilePath(fileName, bucketName);

            StorageResposne objResposne = new StorageResposne();

            using (var stream = new FileStream(path, FileMode.Create))
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();

                PushToCloudStorage(fileName, bucketName, isSaveLocal, path, objResposne, stream);

                return objResposne;
            }
        }

        private static string RetrunFilePath(string fileName, string bucketName)
        {
            string dirPath = Path.Combine(
                         Directory.GetCurrentDirectory(), "wwwroot", bucketName);

            Directory.CreateDirectory(dirPath);

            string path = Path.Combine(dirPath, fileName);
            return path;
        }

        public StorageResposne FormDataUpload(string fileName, string bucketName, IFormFile file, bool isSaveLocal)
        {
            string path = RetrunFilePath(fileName, bucketName);

            StorageResposne objResposne = new StorageResposne();

            if (file == null || file.Length == 0)
                return objResposne;

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyToAsync(stream);
                PushToCloudStorage(fileName, bucketName, isSaveLocal, path, objResposne, stream);
                return objResposne;
            }

        }

        private void PushToCloudStorage(string fileName, string bucketName, bool isSaveLocal, string path, StorageResposne objResposne, FileStream stream)
        {
            GoogleCredential credential;
            using (var gstream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(gstream)
                    .CreateScoped(StorageScope);
            }

            var client = StorageClient.Create(credential);


            string bucketFileName = bucketName + "/" + fileName;
            var obj2 = client.UploadObject("elabs", bucketFileName, getFileStorageContext(path), stream);


            objResposne.LocalFilePath = path;
            objResposne.BucketFilePath = "https://storage.cloud.google.com/elabs/" + bucketFileName;

            if (!isSaveLocal)
            {
                File.Delete(path);
            }
        }
        
        private string getFileStorageContext(string path)
        {
            switch (Path.GetExtension(path).ToLower())
            {
                case "xlsx": return "application/vnd.ms-excel";
                default: return "image/jpeg";
            }
        }
    }
}
