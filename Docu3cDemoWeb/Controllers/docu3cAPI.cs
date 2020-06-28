using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.AI;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using Azure.AI.FormRecognizer.Training;


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
        public string docID;
        public string docURL;
        public string docType;
        public string docModelID;
        public string docParseErrorMsg;
        public Dictionary<string,docu3cProp> docProps;
    }
    [Serializable]
    public class docu3cProp
    {
        public string Name;
        public string Label;
        public string Value;
        public float Confidence;
        public docu3cProp Child;
    }
    public class docu3cAPI
    {
        string endpoint = "https://docworksformrecognizer.cognitiveservices.azure.com/";
        AzureKeyCredential credential = new AzureKeyCredential("a1c9181e05d7460591840dc90d38153e");

        public  async Task<docu3clist> ClassifyDocument(string doc_type, string formUri)
        {
            docu3clist docs = new docu3clist();
            string modelId = GetModelID(doc_type);
            try
            {
                WebClient wc = new WebClient();
                byte[] imageBytes = wc.DownloadData(formUri);
                var stream = new MemoryStream(imageBytes);

                FormRecognizerClient recognizerClient = new FormRecognizerClient(new Uri(endpoint), credential);
                //var forms = await recognizerClient.StartRecognizeCustomFormsFromUri(modelId,new Uri(formUri)).WaitForCompletionAsync();
                //Response<IReadOnlyList<RecognizedForm>>
                var forms = await recognizerClient.StartRecognizeCustomForms(modelId, stream).WaitForCompletionAsync();

                foreach (RecognizedForm form in forms.Value)
                {
                    docu3c doc = new docu3c();
                    doc.docID = formUri;
                    doc.docURL = formUri;
                    doc.docType = form.FormType;
                    doc.docProps = new Dictionary<string, docu3cProp>();
                    foreach (FormField field in form.Fields.Values)
                    {
                        if (field != null)
                        {
                            docu3cProp prop = new docu3cProp();
                            prop.Name = field.Name;
                            if (field.LabelText != null) prop.Label = field.LabelText;
                            if (field.Name == "doc.type") prop.Value = field.ValueText.Text.Replace(" ", "_");
                            else prop.Value = field.ValueText.Text;
                            prop.Confidence = field.Confidence;
                            doc.docProps.Add(field.Name, prop);
                        }
                    }
                    docs.Add(doc);
                }

                docs.html = docu3cAPI.SetDocHTML(docs);
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

        private string GetModelID(string doc_type)
        {
            Dictionary<string, string> kv = new Dictionary<string, string>();
            kv.Add("doc", "6c74493b-bc11-4dc1-a042-f14251dcbc12");
            kv.Add("comp", "93d753ee-a98b-4eb5-8276-48f845039718");
            kv.Add("drivers_lic", "31dc74ed-a341-4ea3-ad5e-7a4e2c6597c8");

            return kv[doc_type];
        }

    }
}
