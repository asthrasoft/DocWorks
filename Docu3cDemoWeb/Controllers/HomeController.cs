//https://docs.microsoft.com/en-us/azure/cognitive-services/form-recognizer/quickstarts/client-library?tabs=windows&pivots=programming-language-csharp
//https://github.com/microsoft/OCR-Form-Tools
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Docu3cDemoWeb.Models;

namespace Docu3cDemoWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
