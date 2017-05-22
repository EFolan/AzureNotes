using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureTables
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionstring = System.Configuration.ConfigurationManager.AppSettings.Get("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionstring);

            //Create a table client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            //Create a reference to a table (whether it exists or not.)
            CloudTable table = tableClient.GetTableReference("FirstTestTable");

            table.CreateIfNotExists();






            //Here is everything to do with Queues
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference("thisisaqueue");

            queue.CreateIfNotExists();

            CloudQueueMessage newmessage = new CloudQueueMessage("This is the fourth message");
            queue.AddMessage(newmessage);

            CloudQueueMessage oldMessage = queue.GetMessage();
            Console.WriteLine(oldMessage.AsString);








            //Here is everything to do with Table storage

            TableBatchOperation batchOperation = new TableBatchOperation();

            //Below: altered to use a TableBatchOperation rather than executing each insert one after the other.

            //CarEntity newCar = new CarEntity(124, 2012, "VW", "Beetle", "Sunshine purple");
            //batchOperation.Insert(newCar);

            //newCar = new CarEntity(125, 2012, "Honda", "Civic", "Periwinkle blue");
            //batchOperation.Insert(newCar);

            //newCar = new CarEntity(126, 1984, "BMW", "M404", "Dystopia Grey");
            //batchOperation.Insert(newCar);

            //newCar = new CarEntity(127, 2010, "Ford", "Model T", "Original Black");
            //batchOperation.Insert(newCar);

            //table.ExecuteBatch(batchOperation);

            TableQuery<CarEntity> carQuery = new TableQuery<CarEntity>();

            foreach(CarEntity thisCar in table.ExecuteQuery(carQuery))
            {
                Console.WriteLine(thisCar.Make + " " + thisCar.Model + " " + thisCar.Colour + " " + thisCar.Year);
            }

            //Lets do a retrieve:
            TableOperation retrieve = TableOperation.Retrieve<CarEntity>("car", "123");

            TableResult result = table.Execute(retrieve);

            if (result.Result == null)
            {
                Console.WriteLine("Not found");
            }
            else
            {
                Console.WriteLine("Found the car: " + ((CarEntity)result.Result).Make + " " + ((CarEntity)result.Result).Model);
            }

            Console.WriteLine("Table created, press any key to continue...");
            Console.ReadKey();
        }
    }

    public class CarEntity : TableEntity
    {
        public CarEntity(int id, int year, string make, string model, string colour)
        {
            this.UniqueID = id;
            this.Year = year;
            this.Make = make;
            this.Model = model;
            this.Colour = colour;
            this.PartitionKey = "car";
            this.RowKey = id.ToString();
        }

        public CarEntity() { }

        public int UniqueID { get; set; }

        public int Year { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public string Colour { get; set; }
    }
}
