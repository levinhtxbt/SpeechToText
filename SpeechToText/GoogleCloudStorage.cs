using System;
using System.IO;
using Google.Cloud.Storage.V1;

namespace SpeechToText
{
    public class GoogleCloudStorage
    {
        public static string CreateBucket(string filePath)
        {
            string projectId = "itc-speechtotext-1572401001631";
            StorageClient storageClient = StorageClient.Create();
            string bucketName = "speedtotext20192";
            try
            {
                storageClient.CreateBucket(projectId, bucketName);
            }
            catch (Google.GoogleApiException e) when (e.Error.Code == 409)
            {
                // The bucket already exists.
                Console.WriteLine(e.Error.Message);
            }

            return null;
        }

        public static string UploadFile(string bucketName, string localPath, string objectName = null)
        {
            try
            {
                var storage = StorageClient.Create();
                using (var f = File.OpenRead(localPath))
                {
                    objectName = objectName ?? Path.GetFileName(localPath);
                    storage.UploadObject(bucketName, objectName, null, f);
                }
                return $"gs://{GoogleConstant.BUCKET_NAME}/{objectName}";
            }
            catch (Exception e)
            {
                return string.Empty;
            }
            
        }

        public static Object GetObject(string bucketName, string objectName)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(bucketName, objectName);

            return storageObject;
        }
    }
}