using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
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
            //if (!QueueOperations("532squeue").Result)
            //    return;

            // Table
            //if (!TableOperations("Customers").Result)
            //    return;

            // Blob
            //if (!BlobOperations("files").Result)
            //    return;

            // File
            if (!FileOperations("files").Result)
                return;
            Console.ReadKey();
        }

        private static async Task<bool> TableOperations(string tableName)
        {
            try
            {
                storageAccount = CloudStorageAccount.Parse(GetConnectionString());
                var tableClient = storageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();

                // Insert
                CustomerEntity customer = new CustomerEntity("Harp", "Walter");
                customer.Email = "Walter@contoso.com";
                customer.PhoneNumber = "425-555-0101";

                TableOperation operation = TableOperation.Insert(customer);
                await table.ExecuteAsync(operation);

                // Retrieve
                operation = TableOperation.Retrieve<CustomerEntity>("Harp", "Walter");
                var query = await table.ExecuteAsync(operation);
                Console.WriteLine(((CustomerEntity)query.Result).PartitionKey);

                // Delete
                await table.DeleteIfExistsAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;

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

        private static async Task<bool> BlobOperations(string containerName)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(GetConnectionString());
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName);
                await container.CreateIfNotExistsAsync();

                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await container.SetPermissionsAsync(permissions);

                var blockBlob = container.GetBlockBlobReference("blob1");
                await blockBlob.UploadTextAsync("Test blob");

                var blobContents = await blockBlob.DownloadTextAsync();
                Console.WriteLine(blobContents);

                var blockBlob2 = container.GetBlockBlobReference("blob2");
                await blockBlob2.StartCopyAsync(blockBlob);

                blobContents = await blockBlob2.DownloadTextAsync();
                Console.WriteLine(blobContents);

                await container.DeleteIfExistsAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private static async Task<bool> FileOperations(string shareName)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(GetConnectionString());
                var fileClient = storageAccount.CreateCloudFileClient();
                var share = fileClient.GetShareReference(shareName);
                await share.CreateIfNotExistsAsync();

                // Create a new shared access policy and define its constraints.
                SharedAccessFilePolicy sharedPolicy = new SharedAccessFilePolicy()
                {
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessFilePermissions.Read | SharedAccessFilePermissions.Write
                };
                var permissions = await share.GetPermissionsAsync();
                permissions.SharedAccessPolicies.Add("SA Policy", sharedPolicy);
                await share.SetPermissionsAsync(permissions);

                // Generate a SAS for a file in the share and associate this access policy with it.
                var root = share.GetRootDirectoryReference();
                var directory = root.GetDirectoryReference("logs");
                await directory.CreateIfNotExistsAsync();
                var file = directory.GetFileReference("Log1.txt");
                var sasToken = file.GetSharedAccessSignature(null, "SA Policy");
                Uri fileSasUri = new Uri(file.StorageUri.PrimaryUri.ToString() + sasToken);

                // Create a new CloudFile object from the SAS, and write some text to the file.
                CloudFile fileSas = new CloudFile(fileSasUri);
                await fileSas.UploadTextAsync("This write operation is authorized via SAS.");
                Console.WriteLine(await fileSas.DownloadTextAsync());

                await share.DeleteIfExistsAsync();

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
