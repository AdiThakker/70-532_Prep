using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.IO;

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

            Console.ReadKey();
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
            Connection.GetDatabase().StringSet(key, "John Doe");

            // Retrieve
            Console.WriteLine(Connection.GetDatabase().StringGet(key));

            // Delete
            Connection.GetDatabase().KeyDelete(key);

        }
    }
}
