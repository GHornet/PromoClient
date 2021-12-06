using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace PromoClient
{
    public class Item
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CreationData { get; set; }
        public string ExpirationData { get; set; }
        public string Expired { get; set; }
        public string Used { get; set; }
        public string MoneyDiscount { get; set; }
        public string MoneyValue { get; set; }
        public string PercentDiscount { get; set; }
        public string PercentValue { get; set; }
        public string ItemDiscount { get; set; }
        public string ItemValue { get; set; }
        public string TimeDiscount { get; set; }
        public string DayTimeDiscountValue { get; set; }
        public string NightTimeDiscountValue { get; set; }
    }

    internal class Program
    {

        static HttpClient client = new HttpClient();
        static void ShowItem(Item item)
        {
            Console.WriteLine("Name =" + item.Name);
            Console.WriteLine("CreationData =" + item.CreationData);
            Console.WriteLine("ExpirationData =" + item.ExpirationData);
            Console.WriteLine("Expired =" + item.Expired);
            Console.WriteLine("Used =" + item.Used);
            Console.WriteLine("MoneyDiscount =" + item.MoneyDiscount);
            Console.WriteLine("MoneyValue =" + item.MoneyValue);
            Console.WriteLine("PercentDiscount =" + item.PercentDiscount);
            Console.WriteLine("PercentValue =" + item.PercentValue);
            Console.WriteLine("ItemDiscount =" + item.ItemDiscount);
            Console.WriteLine("ItemValue =" + item.ItemValue);
            Console.WriteLine("TimeDiscount =" + item.TimeDiscount);
            Console.WriteLine("DayTimeDiscountValue =" + item.DayTimeDiscountValue);
            Console.WriteLine("NightTimeDiscountValue =" + item.NightTimeDiscountValue);
            Console.WriteLine();
        }

        static async Task<Uri> CreateItemAsync(Item item)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/items", item);
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        static async Task<Item> GetItemAsync(string path)
        {
            Item item = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                item = await response.Content.ReadAsAsync<Item>();
            }
            return item;
        }

        static async Task<Item> UpdateItemAsync(Item item)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/items/{item.Id}", item);
            response.EnsureSuccessStatusCode();

            item = await response.Content.ReadAsAsync<Item>();
            return item;
        }

        static async Task<HttpStatusCode> DeleteItemAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/items/{id}");
            return response.StatusCode;
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("https://localhost:5001/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new PromoCode
                Item item = new Item();
                item.Name = "1Q2W3E4R5T";
                item.CreationData = "05.12.2021";
                item.ExpirationData = "05.01.2021";
                item.Expired = "No";
                item.Used = "No";
                item.MoneyDiscount = "Yes";
                item.MoneyValue = "100";
                item.PercentDiscount = "No";
                item.PercentValue = "0";
                item.ItemDiscount = "No";
                item.ItemValue = "0";
                item.TimeDiscount = "No";
                item.DayTimeDiscountValue = "0";
                item.NightTimeDiscountValue = "0";

                var url = await CreateItemAsync(item);
                Console.WriteLine($"Created at url =" + url.ToString());

                
                item = await GetItemAsync(url.PathAndQuery);
                ShowItem(item);

                Console.WriteLine("Updating MoneyValue...");
                item.MoneyValue = "999999";
                await UpdateItemAsync(item);

                item = await GetItemAsync(url.PathAndQuery);
                ShowItem(item);

                var statusCode = await DeleteItemAsync(Convert.ToString(item.Id));
                Console.WriteLine("Deleting...");
                Console.WriteLine("Status =" + (int)statusCode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }
    }
}
