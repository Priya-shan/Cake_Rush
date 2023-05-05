using Cake_Rush.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Cake_Rush.Controllers
{
    public class ApiRequests<T> : Controller
    {
        public string Baseurl = "https://localhost:7001/";
        public async Task<string> postRequest(string url, T model)
        {
            Console.WriteLine("entered  post request");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var Res = await client.PostAsJsonAsync<T>(url, model);

                if (Res.Content.Headers.ContentLength.GetValueOrDefault() > 0)
                {
                    return await Res.Content.ReadAsStringAsync();
                }
                else
                {
                    return Res.StatusCode + " (No content)";
                }
            }
        }
        public async Task<List<T>> getRequest(string url)
        {
            List<T> modelList = new List<T>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(url);
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    Console.Write("res  " + response);
                    modelList = JsonConvert.DeserializeObject<List<T>>(response);
                }
                return modelList;
            }
        }
        public async Task<T> getRequestById(string url, int id)
        {
            //Console.WriteLine("\n\n\nFinal SubCat Map Id -->" + id);
            T model = default(T);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(url);
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    Console.Write("res  " + response);
                    model = JsonConvert.DeserializeObject<T>(response);
                    return model;
                }

            }
            return model;
        }
        public async Task<string> putRequest(string url, int id, T model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var Res = await client.PutAsJsonAsync<T>(url, model);

                if (Res.IsSuccessStatusCode)
                {
                    return "updated successfully";
                }
                else
                {
                    return "update failed";
                }
            }

        }
        public async Task<string> deleteRequest(string url, int id)
        {
            Console.WriteLine("entered delete request");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {

                    return "deleted successfully";
                }
                else
                {
                    return "delete failed";
                }
            }
        }

    }
}
