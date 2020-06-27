//https://docs.microsoft.com/en-us/azure/cognitive-services/form-recognizer/quickstarts/client-library?tabs=windows&pivots=programming-language-csharp
//https://github.com/microsoft/OCR-Form-Tools
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Docu3cDemoWeb.Controllers
{
    public class PortController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public PortController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            DSActions dsa = new DSActions(_env);
            ViewBag._ds = dsa.GetDataSet();
            return View();
        }

        public IActionResult New()
        {
            return View();
        }
        public IActionResult Save(string pName, string cName)
        {
            DSActions dsa = new DSActions(_env);
            ViewBag._ds = dsa.SavePortfolio(pName, cName);
            return View("Index");
        }
        [Route("port/{id?}/files")]
        public IActionResult Files(string id)
        {
            DSActions dsa = new DSActions(_env);
            ViewBag._ds = dsa.GetDataSet();
            ViewBag.pID = id;
            return View();
        }

        ////[Route("port/{id?}/upload")]
        ////public IActionResult upload(string id)
        ////{
        ////    DSActions dsa = new DSActions(_env);
        ////    ViewBag._ds = dsa.GetDataSet();
        ////    ViewBag.pID = id;
        ////    return View();
        ////}

        [HttpPost]
        [Route("port/{id?}/upload")]
        public IActionResult upload(string id)
        {
            //List<IFormFile> files
            var files = Request.Form.Files;
            DSActions dsa = new DSActions(_env);
            dsa.SaveFiles(id, files);
            ViewBag._ds = dsa.GetDataSet();
            ViewBag.pID = id;
            return View("Files");
        }

        [Route("file/{pid?}/{fid?}")]
        public IActionResult ShowFile(string pid, string fid)
        {
            DSActions dsa = new DSActions(_env);
            var fi = dsa.GetFileInfo(fid);
            ViewBag._file = fi["path"];
            ViewBag._doctype = fi["type"];
            string _html = "<h3>Classification : " + fi["type"].ToString()  + "</h3>";
            ViewBag._html = _html;
            ViewBag.pID = pid;
            ViewBag.fID = fid;
            return View("File");
        }

        [Route("identify/{pid?}/{fid?}")]
        public async Task<IActionResult> IdentifyFile(string pid, string fid)
        {

            DSActions dsa = new DSActions(_env);                      
            var fi = dsa.GetFileInfo(fid);
            ViewBag._file = fi["path"];
            
            //ML Action to identify DocType
            docu3cAPI d3 = new docu3cAPI();
            var url = HttpContext.Request.Host.ToString();
            string fileurl = url + fi["path"];
            var docinfo = await d3.ParseDocument("doc", fileurl);
            string _type = docinfo[0].docProps["docType"].Value.ToString();
            _type = _type.Replace(" ", "_");

            ViewBag._doctype = _type;
            string _html = "<h3 class='text-warning'>Classification : " + _type + "</h3>" +
            "<a class='btn btn-primary float-right ml-2' href='/classify/" + pid + "/" + fid + "/" + _type + "'>Accept Classification</a>";
            ViewBag._html = _html;
            ViewBag.pID = pid;
            ViewBag.fID = fid;
            return View("File");
        }

        [Route("classify/{pid?}/{fid?}/{ftype}")]
        public IActionResult ClassifyFile(string pid,string fid, string ftype)
        {
            DSActions dsa = new DSActions(_env);
            var fi = dsa.ClassifyFile(fid, ftype);
            ViewBag._file = fi["path"];
            ViewBag._doctype = fi["type"];
            string _html = "<h3>Classification : " + fi["type"].ToString() + "</h3>";
            ViewBag._html = _html;
            ViewBag.pID = pid;
            ViewBag.fID = fid;
            return View("File");
        }

        [Route("parse/{pid?}/{fid?}")]
        public IActionResult ParseFile(string pid, string fid)
        {
            DSActions dsa = new DSActions(_env);
            var fi = dsa.GetFileInfo(fid);
            ViewBag._file = fi["path"];
            ViewBag._doctype = fi["type"];

            //ML Action to identify DocType



            ViewBag._html = "<h3>File Attributes are as follows : <h3>" +
                "<h6>FileType : DRIVER'S LICENSE</h6>" +
                "<h6>Licence Number : KJURTN34H</h6>";
            ViewBag.pID = pid;
            ViewBag.fID = fid;
            return View("File");
        }

        [Route("delete/{pid?}/{fid?}")]
        public IActionResult DeleteFile(string pid, string fid)
        {
            DSActions dsa = new DSActions(_env);
            ViewBag._ds = dsa.DeleteFile(pid, fid);
            ViewBag.pID = pid;
            return View("Files");
        }
    }
}
