using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Facilitator;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int customerId = 1;
            StringBasedCommunicationFacilitatorServiceClient cli = new StringBasedCommunicationFacilitatorServiceClient();
            var requestBag = new RequestBagGetCustomerOrders()
            {
                RQHeader = new RequestHeaderGetCustomerOrders() { },
                RQBody = new List<RequestBodyGetCustomerOrders>() { new RequestBodyGetCustomerOrders { CustomerId = customerId } }.ToArray()
            };
            var responseBag = cli.GetCustomerOrders(requestBag);

            Console.WriteLine("Customer Id: " + customerId);
            Console.WriteLine("Total number of orders: " + responseBag.RSHeader.TotalNumOfOrders);
            Console.WriteLine();
            for (int i = 0; i < responseBag.RSHeader.TotalNumOfOrders; i++)
            {
                ResponseBodyGetCustomerOrders customerOrder = responseBag.RSBody[i];
                Console.WriteLine("Order Number: " + i);
                Console.WriteLine("Name: " + customerOrder._Orders.Name);
                Console.WriteLine("Desc: " + customerOrder._Orders.Desc);
                Console.WriteLine("Quantity: " + customerOrder._Orders.Quantity);
                Console.WriteLine("Price: " + customerOrder._Orders.Price);
                Console.WriteLine("Amount: " + customerOrder._Orders.Amount);
                Console.WriteLine();
            }
            
            Console.ReadKey();
        }
    }
}
