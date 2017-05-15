using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Blobs
{
    class Program
    {
        static void Main(string[] args)
        {
            string storageconnection = System.Configuration.ConfigurationManager.AppSettings.Get("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageconnection);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("objective2");

            container.CreateIfNotExists();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference("examobjectives");

            using (var fileStream = System.IO.File.OpenRead(@"C:\Users\Enda\Desktop\mofo.txt.txt"))
            {
                blockBlob.UploadFromStream(fileStream);
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
