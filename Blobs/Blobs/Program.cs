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
            //create a connection to the storage account using the connection string in Appsettings
            string connectionstring = System.Configuration.ConfigurationManager.AppSettings.Get("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionstring);

            //Use said storage account to create a Blob Client

            CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();

            //Create a container
            CloudBlobContainer container = BlobClient.GetContainerReference("objective2");
            container.CreateIfNotExists();

            //Create a Blob

            CloudBlockBlob blob = container.GetBlockBlobReference("examobjectives");

            //use Blob to upload from a file stream

            UploadBlob(blob);

            ListAttributes(container);

            SetMetadata(container);
            ListMetadata(container);
            CopyBlob(container);
            UploadBlobSubdirectory(container);

            Console.WriteLine("All done, press any key to continue");
            Console.ReadKey();

        }

        static void UploadBlob(CloudBlockBlob blob)
        {
            using (var fileStream = System.IO.File.OpenRead(@"D:/AzureNotes/BlobUpload/notes.txt"))
            {
                blob.UploadFromStream(fileStream);
            }
        }

        static void ListAttributes(CloudBlobContainer container)
        {
            container.FetchAttributes();
            Console.WriteLine("Container name: " + container.StorageUri.PrimaryUri.ToString());
            Console.WriteLine("Last Modified: " + container.Properties.LastModified.ToString());
        }

        static void SetMetadata(CloudBlobContainer container)
        {
            container.Metadata.Clear();
            container.Metadata.Add("Author", "Enda Folan");
            container.Metadata["authoredOn"] = "May 22, 2017";
            container.SetMetadata();
        }

        static void ListMetadata(CloudBlobContainer container)
        {
            container.FetchAttributes();
            Console.WriteLine("Metadata: ");
            foreach(var i in container.Metadata)
            {
                Console.WriteLine("Key: " + i.Key + " Value: " + i.Value + "\n\n");
            }
        }

        //Copying blobs within Azure Storage
        static void CopyBlob(CloudBlobContainer container)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("examobjectives");
            CloudBlockBlob copyBlockBlob = container.GetBlockBlobReference("examobjectives-copy");
            copyBlockBlob.StartCopyAsync(new Uri(blockBlob.Uri.AbsoluteUri));
        }

        //Creating directories and subdirectories within containers
        static void UploadBlobSubdirectory(CloudBlobContainer container)
        {
            CloudBlobDirectory directory = container.GetDirectoryReference("parent-directory");
            CloudBlobDirectory subdirectory = directory.GetDirectoryReference("subdirectory");
            CloudBlockBlob blockBlob = subdirectory.GetBlockBlobReference("newexamobjectives");

            using (var filestream = System.IO.File.OpenRead(@"D:/AzureNotes/BlobUpload/notes.txt"))
            {
                blockBlob.UploadFromStream(filestream);
            }
        }
    }
}
