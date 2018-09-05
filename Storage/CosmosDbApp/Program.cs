using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CosmosDbApp
{
    class Program
    {
        private static string dbUri = "";
        private static string authKey = "";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Cosmos DB!");

            var result = CosmosDbOperations().Result;            

            Console.ReadKey();

        }

        private static async Task<bool> CosmosDbOperations()
        {
            try
            {
                using (DocumentClient client = new DocumentClient(new Uri(dbUri), authKey))
                {
                    // Create Db
                    Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "Family_DB" });
                    Console.WriteLine($"Created Database: id - {database.Id} and selfLink - {database.SelfLink}");

                    // Create Collection
                    var documentCollection = new DocumentCollection();
                    documentCollection.Id = "Family_Collection";
                    documentCollection.PartitionKey.Paths.Add("/Address/State");

                    DocumentCollection collection = await client.CreateDocumentCollectionIfNotExistsAsync(database.SelfLink, documentCollection);

                    // Get Collection
                    //collection = await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(database.Id, collection.Id));
                    //Console.WriteLine($"Found Collection {collection}");

                    // Delete Collection
                    await client.DeleteDocumentCollectionAsync(collection.SelfLink);
                    
                    // Delete Db
                    await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(database.Id));
                    Console.WriteLine($"Database {database.Id} deleted.");

                }

                return true;
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine($"{de.StatusCode} error occurred: {de.Message}, Message: {baseException.Message}");
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine($"Error: {e.Message}, Message: {baseException.Message}");
            }
            return false;
        }
    }
}
