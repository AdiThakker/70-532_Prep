using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.IO;
using System.Threading;

namespace RedisCacheApp
{
    public class Program
    {
        private static IConfigurationRoot configuration;
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer
                        .Connect($"{configuration.GetSection("redisHostName")},abortConnect=false,ssl=true,password={configuration.GetSection("redisKey")}");
        });

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            configuration = builder.Build();

            Console.WriteLine("Hello Redis!");

            ManageKeys();

            // Transactions
            ManageTransactions();

            Console.ReadKey();
        }

        private static void ManageTransactions()
        {
            var cache = Connection.GetDatabase();
            var transaction = cache.CreateTransaction();

            var withdraw = cache.StringDecrementAsync("Withdraw", -100);
            var deposit = cache.StringIncrementAsync("Deposit", 100);

            bool result = transaction.Execute();

            Console.WriteLine(withdraw.Result);
            Console.WriteLine(deposit.Result);
            
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public static void ManageKeys()
        {
            string key = "Name";

            // Store key
            var db = Connection.GetDatabase();
            db.StringSet(key, "John Doe");

            // Retrieve
            Console.WriteLine(db.StringGet(key));

            // Delete
            db.KeyDelete(key);

            // Set Key with Expiry
            db.StringSet(key, "John Doe", TimeSpan.FromSeconds(3));
            Thread.Sleep(4000);

            Console.WriteLine(db.StringGet(key));

        }
    }
}
