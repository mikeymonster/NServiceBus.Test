using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NServiceBus.Test.Application;

namespace NServiceBus.Test.Controllers
{
    //[Route("Home/Index/")]
    public class HomeController : Controller
    {
        private ISender _sender;
        public HomeController(ISender sender)
        {
            _sender = sender;
        }

        // GET: Home
        public IActionResult Index()
        {
            return View();
        }

        // POST: Home/SEND/
        //[HttpPost]
        //[Route("Send")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Send()
        {
            try
            {
                var message = "This better work";
                await _sender.Send(message);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}