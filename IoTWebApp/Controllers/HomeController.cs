using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IoTWebApp.Models;
using IoTWebApp.Service;

namespace IoTWebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            ViewData["Devices"] = new List<NewDeviceModel>()
            {
                new NewDeviceModel
                {
                    DeviceName = "Test 1",
                    DeviceId = "r302r809328r9u23t9hwefoijowe",
                    Status = "Online",
                    LastActivityUpdated = new DateTime()
                },
                                new NewDeviceModel
                {
                    DeviceName = "Test 1",
                    DeviceId = "r302r809328r9u23t9hwefoijowe",
                    Status = "Online",
                    LastActivityUpdated = new DateTime()
                },
                                                new NewDeviceModel
                {
                    DeviceName = "Test 1",
                    DeviceId = "r302r809328r9u23t9hwefoijowe",
                    Status = "Online",
                    LastActivityUpdated = new DateTime()
                }
            };

            ViewData["Devices"] = await HubService.GetDevices();


            return View();
        }

        public IActionResult ViewDevice()
        {
            return View();
        }

        public IActionResult RegisterDevice()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDeviceAsync(NewDeviceModel newDeviceModel)
        {
            if(newDeviceModel != null)
            {
                await HubService.RegisterDevice(newDeviceModel.DeviceId, newDeviceModel.DeviceName);
                return this.RedirectToAction("Index");
            }
            return View ();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
