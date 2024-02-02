// See https://aka.ms/new-console-template for more information


using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Numerics;
using System.Security.Principal;
using System.Text.Json;

namespace Complex_JSON
{
    class JSONParser
    {

        // Main Method 
        static public void Main(String[] args)
        {
            //1. Get all fulfilled orders
            GetFulfilledOrders();
            //2. Get all orders that is to be shipped to USA
            GetOrderToBeShippedToUSA();
            //3. Get all orders on a particular date
            GetOrdersAt(Convert.ToDateTime("2022-12-05"));
            //4.Get all orders order by date
            GetAllOrder(); //Orderr by Order Date
            //5. Total order in a month
            GetAllOrdersinMonth(12,2022);
        }

        public static void GetAllOrdersinMonth(int orderMonth, int orderYear)
        {
            try
            {
                using StreamReader reader = new StreamReader(getDataPath("OrderData.json"));
                var json = reader.ReadToEnd();

                //JSON Parse
                OrderDetails[] orderDetails = JsonConvert.DeserializeObject<OrderDetails[]>(json);

                int slNo = 1;
                int itemNumber = 0;
                bool ordersFound = false;
                string prodName = string.Empty;
                double orderAmt = 0;
                List<OrderItem> prodNames = new List<OrderItem>();
                Dictionary<int, List<OrderItem>> prodDetails = new Dictionary<int, List<OrderItem>>();

                if (orderDetails != null)
                {
                    foreach (var orders in orderDetails)
                    {
                       
                        if (orders.CreatedAt.Year == orderYear && orders.CreatedAt.Month == orderMonth)
                        {
                            ordersFound = true;
                            foreach (var items in orders.OrderItems)
                            {
                                prodNames.Add(items);
                                if (prodDetails.ContainsKey(items.ProductId))
                                {
                                    prodNames.Single(x => x.ProductId == items.ProductId).Quantity += prodDetails[items.ProductId].Single(x => x.ProductId == items.ProductId).Quantity;
                                    prodDetails[items.ProductId] = prodNames;
                                }
                                else
                                {
                                    prodDetails.Add(items.ProductId, prodNames);
                                }
                                prodNames= new List<OrderItem> ();
                            }
                        }
                    }
                    if (ordersFound)
                    {
                        itemNumber = 0;//There will be only one record in the list
                        slNo = 1;
                        Console.WriteLine($"\n *********TOTAL ORDERS MADE ON {DateAndTime.MonthName(orderMonth)}-{orderYear} *********\n");

                        Console.WriteLine(" SL#\t ID\t\tProduct \t\t\t\t\t Qty \t\tPrice \t\t Amount");
                        Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
                        foreach (var item in prodDetails)
                        {
                            prodName = (item.Value[itemNumber].ProductName.Length > 25) ? item.Value[itemNumber].ProductName.Substring(0, 25) : item.Value[itemNumber].ProductName.PadRight(25);
                            Console.WriteLine($" {slNo}\t {item.Key} \t\t{prodName} \t\t\t {item.Value[itemNumber].Quantity} \t\t{item.Value[itemNumber].UnitPrice} \t\t {Math.Round(item.Value[itemNumber].Quantity * item.Value[itemNumber].UnitPrice, 2)}");
                            slNo++;
                            orderAmt += item.Value[itemNumber].Quantity * item.Value[itemNumber].UnitPrice;
                        }
                        Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"Order Total Amount: {Math.Round(orderAmt, 2)}\n");
                    }
                    else
                    { 
                        Console.WriteLine($"\nNo Orders found on {DateAndTime.MonthName(orderMonth)} {orderYear}");
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
            }
            finally { }
        }
        public static void GetAllOrder()
        {

            try
            {
                using StreamReader reader = new StreamReader(getDataPath("OrderData.json"));
                var json = reader.ReadToEnd();

                //JSON Parse
                OrderDetails[] orderDetails = JsonConvert.DeserializeObject<OrderDetails[]>(json);

                Console.WriteLine("\n *********ALL ORDERS (ORDER DATE ORDERED) *********\n");

                int slNo = 1;
                string prodName = string.Empty;
                double orderAmt = 0;
                if (orderDetails != null)
                {
                    foreach (var orders in orderDetails.OrderBy(x => x.CreatedAt))
                    {
                        slNo = 1;
                        orderAmt = 0;
                        Console.Write($" Order ID: {orders.OrderId.ToString().PadRight(45)}");
                        Console.WriteLine($" Order Number: {orders.OrderNumber}");
                        Console.Write($" Customer Name: {orders.CustomerName.PadRight(40)}");
                        Console.WriteLine($" Seller Name: {orders.SellerName}");
                        Console.WriteLine($" Order Date: {orders.CreatedAt.ToString("dd MMM yyyy")}\n");

                        Console.WriteLine(" SL#\t Product \t\t\t\t\t Qty \t\t Price \t\t Amount");
                        Console.WriteLine("------------------------------------------------------------------------------------------------");

                        foreach (var items in orders.OrderItems)
                        {
                            prodName = (items.ProductName.Length > 25) ? items.ProductName.Substring(0, 25) : items.ProductName.PadRight(25);
                            Console.WriteLine($" {slNo}.\t {prodName}\t\t\t {items.Quantity} \t\t{items.UnitPrice.ToString("#.00")}\t\t {Math.Round(items.Quantity * items.UnitPrice, 2).ToString("#.00")}");
                            slNo++;
                            orderAmt += items.Quantity * items.UnitPrice;
                        }
                        Console.WriteLine("------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"\t\t\t\t\t\t\t\tOrder Total Amount: {Math.Round(orderAmt, 2)}");

                        Console.WriteLine("\n\n");

                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
            }
            finally { }

        }

        public static void GetOrdersAt(DateTime orderDt)
        {

            try
            {
                using StreamReader reader = new StreamReader(getDataPath("OrderData.json"));
                var json = reader.ReadToEnd();

                //JSON Parse
                OrderDetails[] orderDetails = JsonConvert.DeserializeObject<OrderDetails[]>(json);
                //Console.WriteLine(json);

                Console.WriteLine("\n *********ORDERS MADE ON " + orderDt + " *********\n");

                int slNo = 1;
                bool ordersFound = false;
                string prodName = string.Empty;
                if (orderDetails != null)
                {
                    foreach (var orders in orderDetails)
                    {
                        slNo = 1;
                        if (orders.CreatedAt.Year == orderDt.Year && orders.CreatedAt.Month == orderDt.Month && orders.CreatedAt.Day == orderDt.Day)
                        {
                            ordersFound = true;
                            Console.Write($" Order ID: {orders.OrderId.ToString().PadRight(45)}");
                            Console.WriteLine($" Order Number: {orders.OrderNumber}");
                            Console.Write($" Customer Name: {orders.CustomerName.PadRight(40)}");
                            Console.WriteLine($" Seller Name: {orders.SellerName}");
                            Console.WriteLine($" Order Date: {orders.CreatedAt.ToString("dd MMM yyyy hh:mm:ss tt")}\n");

                            Console.WriteLine(" SL#\t Product \t\t\t\t\t Qty \t\t Price \t\t Amount");
                            Console.WriteLine("------------------------------------------------------------------------------------------------");

                            foreach (var items in orders.OrderItems)
                            {
                                prodName = (items.ProductName.Length > 25) ? items.ProductName.Substring(0, 25) : items.ProductName.PadRight(25);
                                Console.WriteLine($" {slNo}.\t {prodName}\t\t\t {items.Quantity} \t\t{items.UnitPrice}\t\t {Math.Round(items.Quantity * items.UnitPrice, 2)}");
                                slNo++;
                            }
                            Console.WriteLine("\n\n");
                        }
                    }
                    if (!ordersFound)
                    {
                        Console.WriteLine($"No Orders found on {orderDt.ToShortDateString()} !");
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
            }
            finally { }

        }
        public static void GetOrderToBeShippedToUSA()
        {
            try
            {
                using StreamReader reader = new StreamReader(getDataPath("OrderData.json"));
                var json = reader.ReadToEnd();

                //JSON Parse
                OrderDetails[] orderDetails = JsonConvert.DeserializeObject<OrderDetails[]>(json);

                Console.WriteLine("\n *************ORDERS TO BE SHIPPED TO USA *************\n");
                Console.WriteLine(" Order ID\t Order Number \t\t Status");
                Console.WriteLine("--------------------------------------------------");
                if (orderDetails != null)
                {
                    foreach (var orders in orderDetails)
                    {
                        if (orders.ShippingAddress.Country == "USA")
                        {
                            Console.WriteLine($" {orders.OrderId} \t\t {orders.OrderNumber} \t\t {orders.OrderStatus}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
            }
            finally { }

        }
        public static void GetFulfilledOrders()
        {
            try
            {
                using StreamReader reader = new StreamReader(getDataPath("OrderData.json"));
                var json = reader.ReadToEnd();

                //JSON Parse
                OrderDetails[] orderDetails = JsonConvert.DeserializeObject<OrderDetails[]>(json);

                Console.WriteLine("\n *************LIST OF FULFILLED ORDERS *************\n");

                Console.WriteLine(" Order ID\t Order Number \t\t Status");
                Console.WriteLine("--------------------------------------------------");
                if (orderDetails != null)
                {
                    foreach (var orders in orderDetails)
                    {
                        if (orders.OrderStatus == "Fulfilled")
                        {
                            Console.WriteLine($" {orders.OrderId} \t\t {orders.OrderNumber} \t\t {orders.OrderStatus}");
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
            }
            finally { }

        }

        //Getting the file path
        public static string getDataPath(string fileName)
        {
            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string datapath = dir + "/Data/" + fileName;

            if (!File.Exists(datapath))
            {
                Console.WriteLine("file does not found!");
                return string.Empty;
            }
            return datapath;
        }
    }
}