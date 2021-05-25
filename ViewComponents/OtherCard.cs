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
        public async Task<IViewComponentResult> InvokeAsync(int? limit, string find = "") {
            using(var client = new HttpClient()) {
                client.BaseAddress = new Uri("https://reallabbook.azurewebsites.net/api/");

                if (limit.HasValue) {
                    var responseTask = client.GetAsync("ToolsAPI?limit=" + limit.Value);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode) {
                        var readTask = result.Content.ReadAsAsync<JArray>();
                        readTask.Wait();

                        ViewBag.equipmentList = readTask.Result;
                    } else {

                        ViewBag.equipmentList = Enumerable.Empty<JArray>();
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                } else {
                    var responseTask = client.GetAsync("ToolsAPI");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode) {
                        var readTask = result.Content.ReadAsAsync<JArray>();
                        readTask.Wait();

                        ViewBag.equipmentList = readTask.Result;
                    } else {

                        ViewBag.equipmentList = Enumerable.Empty<JArray>();
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            return await Task.FromResult((IViewComponentResult) View("OtherCard"));
        }
    }
}