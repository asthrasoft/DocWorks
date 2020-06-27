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
    public class docu3c
    {
        public string docID;
        public string docURL;
        public string docType;
        public string docModelID;
        public string docParseErrorMsg;
        public docu3cProps docProps;
    }
    public class docu3cProps
    {
        public string Name;
        public string Label;
        public string Value;
        public float Confidence;
        public docu3cProps Child;
    }
    public class docu3cAPI
    {
        string endpoint = "https://docworksformrecognizer.cognitiveservices.azure.com/";
        AzureKeyCredential credential = new AzureKeyCredential("a1c9181e05d7460591840dc90d38153e");

        public  async Task<List<docu3c>> ParseDocument(string doc_type, string formUri)
        {
            List<docu3c> docs = new List<docu3c>();
            string modelId = GetModelID(doc_type);
            try
            {
                WebClient wc = new WebClient();
                byte[] imageBytes = wc.DownloadData(formUri);
                var stream = new MemoryStream(imageBytes);

                FormRecognizerClient recognizerClient = new FormRecognizerClient(new Uri(endpoint), credential);
                //var forms = await recognizerClient.StartRecognizeCustomFormsFromUri(modelId,new Uri(formUri)).WaitForCompletionAsync();
                Response<IReadOnlyList<RecognizedForm>> forms = await recognizerClient.StartRecognizeCustomForms(modelId, stream).WaitForCompletionAsync();

                foreach (RecognizedForm form in forms.Value)
                {
                    docu3c doc = new docu3c();
                    doc.docID = formUri;
                    doc.docURL = formUri;
                    doc.docType = form.FormType;
                    foreach (FormField field in form.Fields.Values)
                    {
                        docu3cProps prop = new docu3cProps();
                        prop.Name = field.Name;
                        if (field.LabelText != null) prop.Label = field.LabelText;
                        if (field.ValueText != null) prop.Value = field.ValueText;
                        prop.Confidence = field.Confidence;
                    }
                    docs.Add(doc);
                }
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

        private string GetModelID(string doc_type)
        {
            Dictionary<string, string> kv = new Dictionary<string, string>();
            kv.Add("drivers_lic", "31dc74ed-a341-4ea3-ad5e-7a4e2c6597c8");

            return kv[doc_type];
        }
    }
}
