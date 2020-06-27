//https://docs.microsoft.com/en-us/azure/cognitive-services/form-recognizer/quickstarts/client-library?tabs=windows&pivots=programming-language-csharp
//https://github.com/microsoft/OCR-Form-Tools
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

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
            var url = HttpContext.Request.Host.ToString();
            string fileurl = "https://" + url + fi["path"];

            ViewBag._file = fileurl;
            ViewBag._doctype = fi["type"];

            var docinfo = dsa.GetFileProperties(pid,fid);

            string _html = "";
            if (docinfo.Count > 0)
            {
                if (docinfo[0].docProps.ContainsKey("doc.type"))
                    _html += "<h5 class='text-warning'>doc.type : " + docinfo[0].docProps["doc.type"].Value.ToString() + "</h5>";
                if (docinfo[0].docProps.ContainsKey("org.name"))
                    _html += "<h5>org.name : " + docinfo[0].docProps["org.name"].Value.ToString() + "</h5>";
                if (docinfo[0].docProps.ContainsKey("cust.name"))
                    _html += "<h5>cust.name : " + docinfo[0].docProps["cust.name"].Value.ToString() + "</h5>";
                if (docinfo[0].docProps.ContainsKey("cust.dob"))
                    _html += "<h5>cust.dob : " + docinfo[0].docProps["cust.dob"].Value.ToString() + "</h5>";
                if (docinfo[0].docProps.ContainsKey("cust.ssn"))
                    _html += "<h5>cust.ssn : " + docinfo[0].docProps["cust.ssn"].Value.ToString() + "</h5>";
                if (docinfo[0].docProps.ContainsKey("cust.addr"))
                    _html += "<h5>cust.addr : " + docinfo[0].docProps["cust.addr"].Value.ToString() + "</h5>";

                if (fi["type"].ToString() == "UNKNOWN")
                    _html += "<a class='btn btn-primary float-right ml-2' href='/classify/" + pid + "/" + fid + "/" + docinfo[0].docProps["doc.type"].Value.ToString() + "'>Accept Classification</a>";

            }

            ViewBag._html = _html;
            ViewBag.pID = pid;
            ViewBag.fID = fid;
            return View("File");
        }

        [Route("identify/{pid?}/{fid?}")]
        public async Task<IActionResult> ClassifyFile(string pid, string fid)
        {

            DSActions dsa = new DSActions(_env);                      
            var fi = dsa.GetFileInfo(fid);
            ViewBag._file = fi["path"];
            
            docu3cAPI d3 = new docu3cAPI();
            var url = HttpContext.Request.Host.ToString();
            string fileurl = "https://" + url + fi["path"];

            //fileurl = "https://docworksweb.azurewebsites.net/data/1593291961/ACCS_STMT_Aaron%20B%20McDaniel.pdf";

            var docinfo = await d3.ClassifyDocument("comp", fileurl);
            dsa.SaveFileProperties(pid,fid, docinfo);

            string _html = "";
            if (docinfo.Count > 0)
            {
                if (docinfo[0].docProps.ContainsKey("doc.type"))
                    _html += "<h5 class='text-warning'>doc.type : " + docinfo[0].docProps["doc.type"].Value.ToString() + "</h5>";
                if (docinfo[0].docProps.ContainsKey("org.name"))
                    _html += "<h5>org.name : " + docinfo[0].docProps["org.name"].Value.ToString() + "</h5>";
                if (docinfo[0].docProps.ContainsKey("cust.name"))
                    _html += "<h5>cust.name : " + docinfo[0].docProps["cust.name"].Value.ToString() + "</h5>";
                if (docinfo[0].docProps.ContainsKey("cust.dob"))
                    _html += "<h5>cust.dob : " + docinfo[0].docProps["cust.dob"].Value.ToString() + "</h5>";
                if (docinfo[0].docProps.ContainsKey("cust.ssn"))
                    _html += "<h5>cust.ssn : " + docinfo[0].docProps["cust.ssn"].Value.ToString() + "</h5>";
                if (docinfo[0].docProps.ContainsKey("cust.addr"))
                    _html += "<h5>cust.addr : " + docinfo[0].docProps["cust.addr"].Value.ToString() + "</h5>";

                if (fi["type"].ToString() == "UNKNOWN")
                    _html += "<a class='btn btn-primary float-right ml-2' href='/classify/" + pid + "/" + fid + "/" + docinfo[0].docProps["doc.type"].Value.ToString() + "'>Accept Classification</a>";
            }

            ViewBag._html = _html;
            ViewBag.pID = pid;
            ViewBag.fID = fid;
            return View("File");
        }

        [Route("classify/{pid?}/{fid?}/{ftype}")]
        public IActionResult SaveFileClassification(string pid,string fid, string ftype)
        {
            DSActions dsa = new DSActions(_env);
            var fi = dsa.SaveFileClassification(fid, ftype);
            ViewBag._file = fi["path"];
            ViewBag._doctype = fi["type"];
            string _html = "<h3>Classification : " + fi["type"].ToString() + "</h3>";
            ViewBag._html = _html;
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
