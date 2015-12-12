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
            int i = 0;
            foreach (var responseBodyItem in responseBag.RSBody)
            {
                Console.WriteLine("Order Number: " + i++);
                Console.WriteLine("Name: " + responseBodyItem._Order.Name);
                Console.WriteLine("Desc: " + responseBodyItem._Order.Desc);
                Console.WriteLine("Quantity: " + responseBodyItem._Order.Quantity);
                Console.WriteLine("Price: " + responseBodyItem._Order.Price);
                Console.WriteLine("Amount: " + responseBodyItem._Order.Amount);
                Console.WriteLine();
            }
            
            Console.ReadKey();
        }
    }
}
