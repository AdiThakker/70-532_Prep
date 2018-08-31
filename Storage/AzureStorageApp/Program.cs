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

            // Queue 
            if (!QueueOperations("532squeue").Result)
                return;

            // Table 

            // Blob


            // File

            Console.ReadKey();
        }

        private static string GetConnectionString()
        {
            return $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};";
        }

        private static async Task<bool> QueueOperations(string queueName)
        {
            try
            {
                storageAccount = CloudStorageAccount.Parse(GetConnectionString());
                var queueClient = storageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(queueName);

                // Create Queue
                await queue.CreateIfNotExistsAsync();

                // Clear Messages if any
                await queue.ClearAsync();

                // Add Message
                await queue.AddMessageAsync(new CloudQueueMessage("Hello Azure Queue!"));

                await queue.FetchAttributesAsync();

                Console.WriteLine($"Approximate Messages: {queue.ApproximateMessageCount}");

                // Peek Message
                var message = await queue.PeekMessageAsync();
                Console.WriteLine($"Peek Message: {message.ToString()}");

                // Retrieve Message
                message = await queue.GetMessageAsync();                    
                Console.WriteLine($"Retrieve Message: {message.ToString()}");

                // Delete to finish processing
                await queue.DeleteMessageAsync(message);

                await queue.FetchAttributesAsync();

                Console.WriteLine($"Approximate Messages: {queue.ApproximateMessageCount}");

                // Delete Queue
                await queue.DeleteIfExistsAsync();

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
