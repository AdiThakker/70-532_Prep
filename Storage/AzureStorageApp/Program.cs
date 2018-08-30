using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace AzureStorageApp
{
    public class Program
    {
        private static string accountName = "532s";
        private static string accountKey = "";
        private static CloudStorageAccount storageAccount;

        public static void Main(string[] args)
        {
            Console.WriteLine(GetConnectionString());
            var result = CreateQueue("532squeue").Result;
            Console.ReadKey();
        }

        private static string GetConnectionString()
        {
            return $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};";
        }

        private static async Task<bool> CreateQueue(string queueName)
        {
            try
            {
                storageAccount = CloudStorageAccount.Parse(GetConnectionString());
                var queueClient = storageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(queueName);
                await queue.CreateIfNotExistsAsync();
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
