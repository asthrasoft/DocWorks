﻿//https://docs.microsoft.com/en-us/azure/cognitive-services/form-recognizer/quickstarts/client-library?tabs=windows&pivots=programming-language-csharp
//https://github.com/microsoft/OCR-Form-Tools
using System;
using System.IO;
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
            var value = DirSize(new DirectoryInfo(_env.WebRootPath + "/data/"));
            ViewBag.DirInfo = (value / (double)Math.Pow(1024, 2)).ToString("0.00") + " MB";
            return View();
        }

        private long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }
        public IActionResult Clear()
        {
            DSActions dsa = new DSActions(_env);
            ViewBag._ds = dsa.ClearData();
            var value = DirSize(new DirectoryInfo(_env.WebRootPath + "/data/"));
            ViewBag.DirInfo = (value / (double)Math.Pow(1024, 2)).ToString("0.00") + " MB";
            return View("Settings");
        }

        public IActionResult LogOff()
        {
            return View("Site");
        }
    }
}
