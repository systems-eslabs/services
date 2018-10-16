using System;
using System.IO;
using Google.Apis.Storage.v1;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Common;

namespace StorageService
{
    public class Storage
    {
        //static readonly string[] StorageScope = { StorageService.Scope.DevstorageReadWrite };

        public BaseReturn<EStorageResponse> BinaryUpload(EStorageRequest storageRequest, byte[] data)
        {
            BaseReturn<EStorageResponse> baseObject = new BaseReturn<EStorageResponse>();
            EStorageResponse objResposne = null;
            try
            {
                string path = getLocalFilePath(storageRequest);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    objResposne = PushToCloudStorage(storageRequest, stream,path);
                }
                baseObject.Success = true;
                baseObject.Data = objResposne;
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Message = "Error Occured!";
                baseObject.Exception = ex;
            }
            return baseObject;
        }

        public BaseReturn<EStorageResponse> FormDataUpload(EStorageRequest storageRequest, IFormFile file)
        {
            BaseReturn<EStorageResponse> baseObject = new BaseReturn<EStorageResponse>();
            EStorageResponse objResposne = null;
            try
            {
                if (file != null && file.Length > 0)
                {
                    string path = getLocalFilePath(storageRequest);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyToAsync(stream);
                        objResposne = PushToCloudStorage(storageRequest, stream, path);
                    }
                    baseObject.Success = true;
                    baseObject.Data = objResposne;
                }
                else
                {
                    baseObject.Success = false;
                    baseObject.Message = "File is empty.";
                }

            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Message = "Error Occured!";
                baseObject.Exception = ex;
            }
            return baseObject;
        }

        private string getLocalFilePath(EStorageRequest storageRequest)
        {
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", storageRequest.BucketName);
            Directory.CreateDirectory(dirPath);
            return Path.Combine(dirPath, storageRequest.FileName);
        }

        private EStorageResponse PushToCloudStorage(EStorageRequest storageRequest, FileStream stream, string path)
        {
            EStorageResponse objResposne = new EStorageResponse();

            var credential = GoogleCredential.GetApplicationDefault();
            var client = StorageClient.Create(credential);

            string bucketFileName = storageRequest.BucketName + "/" + storageRequest.FileName;
            client.UploadObject(storageRequest.ProjectName, bucketFileName, getContentType(path), stream);

            objResposne.LocalFilePath = path;
            objResposne.BucketFilePath = "https://storage.cloud.google.com/" + storageRequest.ProjectName + "/" + bucketFileName;

            if (!storageRequest.isSaveLocal)
            {
                File.Delete(path);
            }
            return objResposne;
        }

        private string getContentType(string path)
        {
            switch (Path.GetExtension(path).ToLower())
            {
                case "xlsx": return "application/vnd.ms-excel";
                default: return "image/jpeg";
            }
        }
    }
}
