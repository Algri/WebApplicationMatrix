using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplicationMatrix.Models;

namespace WebApplicationMatrix.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Output(Data data)
        {             
            int result = data.GetOutputResult();
            ViewData["Output"] = result.ToString();
            return View(data);
        }
    }
}
