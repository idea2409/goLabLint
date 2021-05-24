using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using golablint.Data;
using golablint.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace golablint.Controllers {
    public class OtherController : Controller {
        
        // Uri baseAddress = new Uri("https://reallabbook.azurewebsites.net/api/ToolsAPI?limit=3");
       
        // HttpClient client;
       
        // public OtherController()
        // {
        //     client = new HttpClient();
        //     client.BaseAddress = baseAddress;
        // }
        
        public ActionResult Index()
        {
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
                var responseTask = client.GetAsync("ToolsAPI");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Other>>();
                    readTask.Wait();

                    other = readTask.Result;
                }
                else //web api sent error response 
                {

                    other = Enumerable.Empty<Other>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(other);
        }

    }
}