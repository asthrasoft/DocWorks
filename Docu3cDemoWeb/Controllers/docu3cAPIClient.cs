using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
//using System.Text.Json;
using Newtonsoft.Json;

namespace Docu3cDemoWeb
{
    [Serializable]
    public class docu3clist : List<docu3c>
    {
        public string html;
    }

    [Serializable]
    public class docu3c
    {
        public string docID { get; set; }
        public string docURL { get; set; }
        public string docType { get; set; }
        public string docModelID { get; set; }
        public string docParseErrorMsg { get; set; }
        public Dictionary<string, docu3cProp> docProps { get; set; }
    }
    [Serializable]
    public class docu3cProp
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public float Confidence { get; set; }
        public docu3cProp Child { get; set; }
    }
    [Serializable]
    public class docu3cInput
    {
        public string doc_type { get; set; }
        public string formUri { get; set; }
    }

    public class docu3cAPIClient
    {
        string SERVICE_URL = "https://docworksapi.azurewebsites.net/api/docu3cAPI/";
        //string SERVICE_URL = "https://localhost:44371/api/docu3cAPI/";
        string API_KEY = "SAMPLETESTAPIKEY";

        public  docu3clist ClassifyDocument(string doc_type, string formUri)
        {
            docu3clist docs = new docu3clist();
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(SERVICE_URL);
                //For local web service, comment out this line.
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);

                docu3cInput inputJson = new docu3cInput();
                inputJson.doc_type = doc_type; inputJson.formUri = formUri;

                var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                request.Content = new StringContent(JsonConvert.SerializeObject(inputJson));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = client.SendAsync(request).Result;

                var jsonString = response.Content.ReadAsStringAsync().Result;
                docs = JsonConvert.DeserializeObject<docu3clist>(jsonString);

                return docs;
            }
            catch (Exception ex)
            {
                docu3c doc = new docu3c();
                doc.docParseErrorMsg = ex.Message;
                docs.Add(doc);
                return docs;
            }
        }

        public static string SetDocHTML(docu3clist docs)
        {
            string _html = "";
            if (docs.Count > 0)
            {
                if (docs[0].docProps.ContainsKey("doc.type"))
                    _html += "<h6 class='text-warning'>doc.type : <span class='text-success'>" + docs[0].docProps["doc.type"].Value.ToString() + "</span></h6>";
                if (docs[0].docProps.ContainsKey("org.name"))
                    _html += "<h6 class='text-warning'>org.name : <span class='text-success'>" + docs[0].docProps["org.name"].Value.ToString() + "</span></h6>";
                if (docs[0].docProps.ContainsKey("cust.name"))
                    _html += "<h6 class='text-warning'>cust.name : <span class='text-success'>" + docs[0].docProps["cust.name"].Value.ToString() + "</span></h6>";
                if (docs[0].docProps.ContainsKey("cust.dob"))
                    _html += "<h6 class='text-warning'>cust.dob : <span class='text-success'>" + docs[0].docProps["cust.dob"].Value.ToString() + "</span></h6>";
                if (docs[0].docProps.ContainsKey("cust.ssn"))
                    _html += "<h6 class='text-warning'>cust.ssn : <span class='text-success'>" + docs[0].docProps["cust.ssn"].Value.ToString() + "</span></h6>";
                if (docs[0].docProps.ContainsKey("cust.addr"))
                    _html += "<h6 class='text-warning'>cust.addr : <span class='text-success'>" + docs[0].docProps["cust.addr"].Value.ToString() + "</span></h6>";
            }
            return _html;
        }

    }
}
