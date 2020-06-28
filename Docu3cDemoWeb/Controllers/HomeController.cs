//https://docs.microsoft.com/en-us/azure/cognitive-services/form-recognizer/quickstarts/client-library?tabs=windows&pivots=programming-language-csharp
//https://github.com/microsoft/OCR-Form-Tools
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Docu3cDemoWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            List<string> retstr = new List<string>();
            ViewBag.ResultHTML = retstr;
            return View();
        }
        public IActionResult Site()
        {
            List<string> retstr = new List<string>();
            ViewBag.ResultHTML = retstr;
            return View();
        }
        public IActionResult Settings()
        {
            DSActions dsa = new DSActions(_env);
            ViewBag._ds = dsa.GetDataSet();
            return View();
        }

        public IActionResult Clear()
        {
            DSActions dsa = new DSActions(_env);
            ViewBag._ds = dsa.ClearData();
            return View("Settings");
        }

        public IActionResult LogOff()
        {
            return View("Site");
        }

        public IActionResult Parse()
        {
            List<string> retstr = new List<string>();
            ViewBag.ResultHTML = retstr;
            return View();
        }
        public IActionResult Analyze()
        {
            List<string> retstr = new List<string>();
            ViewBag.ResultHTML = retstr;
            return View();
        }
        public IActionResult Recognize()
        {
            List<string> retstr = new List<string>();
            ViewBag.ResultHTML = retstr;
            return View();
        }

        [HttpPost]
        public IActionResult parse(string file)
        {
            AzureFormRecog afr = new AzureFormRecog();
            var t1 = afr.ParseForm(file);
            Task.WaitAll(t1);

            ViewBag.ResultHTML = t1.Result;
            return View("Parse");
        }


        [HttpPost]
        public IActionResult recognize(string file)
        {
            AzureFormRecog afr = new AzureFormRecog();
            //var t1 = afr.RecognizeReceipt(file);
            //Task.WaitAll(t1);

            ViewBag.ResultHTML = "";//t1.Result;
            return View("Recognize");
        }

        [HttpPost]
        public IActionResult analyze(string file)
        {
            string modelid = "31dc74ed-a341-4ea3-ad5e-7a4e2c6597c8";
            AzureFormRecog afr = new AzureFormRecog();
            //var t1 = afr.AnalyzePdfForm(modelid,file);
            //Task.WaitAll(t1);

            ViewBag.ResultHTML = "";// t1.Result;
            return View("Analyze");
        }

    }
}
