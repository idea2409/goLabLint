using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using golablint.Controllers;
using golablint.Data;
using golablint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace goLabLint.wwwroot.ViewComponents {
    public class OtherCard : ViewComponent {
       public async Task<IViewComponentResult> InvokeAsync(int? limit, string role, string find = "") {
        //     List<Other> modelList = new List<Other>();
        //     HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/user").Result;
        //     if (response.IsSuccessStatusCode)
        //     {
        //         string data = response.Content.ReadAsStringAsync().Result;
        //         modelList = JsonConvert.DeserializeObject<List<Other>>(data);
        //     }
        //     else //web api sent error response 
        //     {
        //         ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
        //     }
            // return View();
            IEnumerable<Other> other = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://reallabbook.azurewebsites.net/api/");
                //HTTP GET
                var responseTask = client.GetAsync("ToolsAPI?limit=3");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Other>>();
                    readTask.Wait();

                    other = readTask.Result;
                    ViewBag.equipmentList = JObject.Parse(JsonConvert.SerializeObject(other)).GetValue("Value");
                }
                else //web api sent error response 
                {
                    //log response status here..

                    other = Enumerable.Empty<Other>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return await Task.FromResult((IViewComponentResult) View("OtherCard"));
        }

        // Uri baseAddress = new Uri("https://reallabbook.azurewebsites.net/api/ToolsAPI?limit=3");
       
        // HttpClient client;
       
        // // // private readonly ApplicationDbContext _db;

        // public OtherCard()
        // {
        //     client = new HttpClient();
        //     client.BaseAddress = baseAddress;
        // }
        // public async Task<IViewComponentResult> InvokeAsync(int? limit, string role, string find = "") {
        //     List<Other> modelList = new List<Other>();
        //     HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/user").Result;
        //     if (response.IsSuccessStatusCode)
        //     {
        //         var data = response.Content.ReadAsStringAsync().Result;
        //         // modelList = JsonConvert.DeserializeObject<List<Other>>(data);
        //         ViewBag.equipmentList = JObject.Parse(JsonConvert.SerializeObject(data)).GetValue("Value");
        //     }
        //     else //web api sent error response 
        //     {
        //         ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
        //     }
        //     ViewBag.role = role;
        //     return await Task.FromResult((IViewComponentResult) View("OtherCard"));
        // }

    }
}