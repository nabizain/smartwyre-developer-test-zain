using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System;

namespace Smartwyre.DeveloperTest.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Smartwyre Rebate Calculator ===");
            Console.WriteLine();

            // Accept inputs from command line arguments OR prompt user
            string rebateIdentifier;
            string productIdentifier;
            decimal volume;

            if (args.Length >= 3)
            {
                rebateIdentifier = args[0];
                productIdentifier = args[1];
                if (!decimal.TryParse(args[2], out volume))
                {
                    Console.WriteLine("Invalid volume argument.");
                    return;
                }
            }
            else
            {
                Console.Write("Enter Rebate Identifier: ");
                rebateIdentifier = Console.ReadLine();

                Console.Write("Enter Product Identifier: ");
                productIdentifier = Console.ReadLine();

                Console.Write("Enter Volume: ");
                while (!decimal.TryParse(Console.ReadLine(), out volume))
                {
                    Console.Write("Invalid input. Please enter a valid number for Volume: ");
                }
            }

            // Set up dependencies (normally done via dependency injection)
            IRebateDataStore rebateDataStore = new RebateDataStore();
            IProductDataStore productDataStore = new ProductDataStore();
            IRebateService rebateService = new RebateService(rebateDataStore, productDataStore);

            // Create request
            var request = new CalculateRebateRequest
            {
                RebateIdentifier = rebateIdentifier,
                ProductIdentifier = productIdentifier,
                Volume = volume
            };

            // Run calculation
            var result = rebateService.Calculate(request);

            Console.WriteLine();
            Console.WriteLine("=== Calculation Result ===");
            Console.WriteLine($"Rebate Identifier : {rebateIdentifier}");
            Console.WriteLine($"Product Identifier: {productIdentifier}");
            Console.WriteLine($"Volume            : {volume}");
            Console.WriteLine($"Success           : {result.Success}");
            Console.WriteLine("===========================");
            Console.WriteLine();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
