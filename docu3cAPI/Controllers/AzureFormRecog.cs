//https://docs.microsoft.com/en-us/azure/cognitive-services/form-recognizer/quickstarts/client-library?tabs=windows&pivots=programming-language-csharp
//https://github.com/microsoft/OCR-Form-Tools

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
    public class AzureFormRecog
    {
        string endpoint = "https://docworksformrecognizer.cognitiveservices.azure.com/";
        AzureKeyCredential credential = new AzureKeyCredential("a1c9181e05d7460591840dc90d38153e");

        //public async Task RunFormRecognizerClient()
        //{


        //    string trainingDataUrl = "<SAS-URL-of-your-form-folder-in-blob-storage>";
        //    //string formUrl = "<SAS-URL-of-a-form-in-blob-storage>";

        //    string receiptUrl = "https://docs.microsoft.com/azure/cognitive-services/form-recognizer/media"
        //    + "/contoso-allinone.jpg";

        //    // Call Form Recognizer scenarios:
        //    Console.WriteLine("Get form content...");
        //    await GetContent(recognizerClient, formUrl);

        //    Console.WriteLine("Analyze receipt...");
        //    await AnalyzeReceipt(recognizerClient, receiptUrl);

        //    //Console.WriteLine("Train Model with training data...");
        //    //Guid modelId = await TrainModel(trainingClient, trainingDataUrl);

        //    //Console.WriteLine("Analyze PDF form...");
        //    //await AnalyzePdfForm(recognizerClient, modelId, formUrl);

        //    //Console.WriteLine("Manage models...");
        //    //await ManageModels(trainingClient, trainingDataUrl);
        //}

        public async Task<List<string>> ParseForm(string formurl)
        {
            List<string> retstr = new List<string>();
            retstr.Add("<h3>starting Output Rendering</h3>");
            //Response<IReadOnlyList<FormPage>> formPages = await recognizerClient.StartRecognizeContentFromUri(new Uri(invoiceUri)).WaitForCompletionAsync();
            FormRecognizerClient recognizerClient = new FormRecognizerClient(new Uri(endpoint), credential);
            var formPages = await recognizerClient.StartRecognizeContentFromUri(new Uri(formurl)).WaitForCompletionAsync();

            foreach (FormPage page in formPages.Value)
            {
                retstr.Add($"Form Page {page.PageNumber} has {page.Lines.Count}" + $" lines.");

                for (int i = 0; i < page.Lines.Count; i++)
                {
                    FormLine line = page.Lines[i];
                    retstr.Add($"    Line {i} has {line.Words.Count}" +
                        $" word{(line.Words.Count > 1 ? "s" : "")}," +
                        $" and text: '{line.Text}'.");
                }

                for (int i = 0; i < page.Tables.Count; i++)
                {
                    FormTable table = page.Tables[i];
                    retstr.Add($"Table {i} has {table.RowCount} rows and" +
                        $" {table.ColumnCount} columns.");
                    foreach (FormTableCell cell in table.Cells)
                    {
                        retstr.Add($"    Cell ({cell.RowIndex}, {cell.ColumnIndex})" +
                            $" contains text: '{cell.Text}'.");
                    }
                }
            }
            retstr.Add("<h3>End Output Rendering</h3>");

            return retstr;
        }

        //public async Task<List<string>> RecognizeReceipt(string receiptUri)
        //{
        //    List<string> retstr = new List<string>();
        //    retstr.Add("<h3>starting Output Rendering</h3>");
        //    //Response<IReadOnlyList<RecognizedReceipt>> receipts = await recognizerClient.StartRecognizeReceiptsFromUri(new Uri(receiptUri)).WaitForCompletionAsync();
        //    FormRecognizerClient recognizerClient = new FormRecognizerClient(new Uri(endpoint), credential);
        //    var receipts = await recognizerClient.StartRecognizeReceiptsFromUri(new Uri(receiptUri)).WaitForCompletionAsync();
        //    foreach (var receipt in receipts.Value)
        //    {
        //        USReceipt usReceipt = receipt.AsUSReceipt();

        //        string merchantName = usReceipt.MerchantName?.Value ?? default;
        //        DateTime transactionDate = usReceipt.TransactionDate?.Value ?? default;
        //        IReadOnlyList<USReceiptItem> items = usReceipt.Items ?? default;

        //        retstr.Add($"Recognized USReceipt fields:");
        //        retstr.Add($"    Merchant Name: '{merchantName}', with confidence " +
        //            $"{usReceipt.MerchantName.Confidence}");
        //        retstr.Add($"    Transaction Date: '{transactionDate}', with" +
        //            $" confidence {usReceipt.TransactionDate.Confidence}");

        //        for (int i = 0; i < items.Count; i++)
        //        {
        //            USReceiptItem item = usReceipt.Items[i];
        //            retstr.Add($"    Item {i}:  Name: '{item.Name.Value}'," +
        //                $" Quantity: '{item.Quantity?.Value}', Price: '{item.Price?.Value}'");
        //            retstr.Add($"    TotalPrice: '{item.TotalPrice.Value}'");
        //        }


        //        float subtotal = usReceipt.Subtotal?.Value ?? default;
        //        float tax = usReceipt.Tax?.Value ?? default;
        //        float tip = usReceipt.Tip?.Value ?? default;
        //        float total = usReceipt.Total?.Value ?? default;

        //        retstr.Add($"    Subtotal: '{subtotal}', with confidence" +
        //            $" '{usReceipt.Subtotal.Confidence}'");
        //        retstr.Add($"    Tax: '{tax}', with confidence '{usReceipt.Tax.Confidence}'");
        //        retstr.Add($"    Tip: '{tip}', with confidence '{usReceipt.Tip?.Confidence ?? 0.0f}'");
        //        retstr.Add($"    Total: '{total}', with confidence '{usReceipt.Total.Confidence}'");
        //    }

        //    return retstr;
        //}

        //public async Task<string> TrainModelWithoutLabelsAsync(string trainingDataUrl)
        //{
        //    var trainingClient = new FormTrainingClient(new Uri(endpoint), credential);
        //    CustomFormModel model = await trainingClient.StartTrainingAsync(new Uri(trainingDataUrl)).WaitForCompletionAsync();

        //    Console.WriteLine($"Custom Model Info:");
        //    Console.WriteLine($"    Model Id: {model.ModelId}");
        //    Console.WriteLine($"    Model Status: {model.Status}");
        //    Console.WriteLine($"    Created On: {model.CreatedOn}");
        //    Console.WriteLine($"    Last Modified: {model.LastModified}");

        //    foreach (CustomFormSubModel subModel in model.Models)
        //    {
        //        Console.WriteLine($"SubModel Form Type: {subModel.FormType}");
        //        foreach (CustomFormModelField field in subModel.Fields.Values)
        //        {
        //            Console.Write($"    FieldName: {field.Name}");
        //            if (field.Label != null)
        //            {
        //                Console.Write($", FieldLabel: {field.Label}");
        //            }
        //            Console.WriteLine("");
        //        }
        //    }
        //    return model.ModelId;
        //}

        //public async Task<string> TrainModelWithLabelsAsync(string trainingDataUrl)
        //{
        //    //labels should be in <filename>.pdf.labels.json
        //    var trainingClient = new FormTrainingClient(new Uri(endpoint), credential);
        //    CustomFormModel model = await trainingClient.StartTrainingAsync(new Uri(trainingDataUrl)).WaitForCompletionAsync();

        //    Console.WriteLine($"Custom Model Info:");
        //    Console.WriteLine($"    Model Id: {model.ModelId}");
        //    Console.WriteLine($"    Model Status: {model.Status}");
        //    //Console.WriteLine($"    Requested on: {model.RequestedOn}");
        //    //Console.WriteLine($"    Completed on: {model.CompletedOn}");

        //    //foreach (CustomFormSubmodel submodel in model.Submodels)
        //    //{
        //    //    Console.WriteLine($"Submodel Form Type: {submodel.FormType}");
        //    //    foreach (CustomFormModelField field in submodel.Fields.Values)
        //    //    {
        //    //        Console.Write($"    FieldName: {field.Name}");
        //    //        if (field.Accuracy != null)
        //    //        {
        //    //            Console.Write($", Accuracy: {field.Accuracy}");
        //    //        }
        //    //        Console.WriteLine("");
        //    //    }
        //    //}
        //    return model.ModelId;
        //}
        //public  async Task<List<string>> AnalyzePdfForm(string modelId, string formUri)
        //{
        //    List<string> retstr = new List<string>();
        //    retstr.Add("<h3>starting Output Rendering</h3>");
        //    try
        //    {

        //        WebClient wc = new WebClient();
        //        byte[] imageBytes = wc.DownloadData(formUri);
        //        var stream = new MemoryStream(imageBytes);

        //        FormRecognizerClient recognizerClient = new FormRecognizerClient(new Uri(endpoint), credential);
        //        //var forms = await recognizerClient.StartRecognizeCustomFormsFromUri(modelId,new Uri(formUri)).WaitForCompletionAsync();
        //        Response<IReadOnlyList<RecognizedForm>> forms = await recognizerClient.StartRecognizeCustomForms(modelId, stream).WaitForCompletionAsync();

        //        foreach (RecognizedForm form in forms.Value)
        //        {
        //            retstr.Add($"Form of type: {form.FormType}");
        //            foreach (FormField field in form.Fields.Values)
        //            {
        //                retstr.Add($"Field '{field.Name}: ");

        //                if (field.LabelText != null)
        //                {
        //                    retstr.Add($"    Label: '{field.LabelText.Text}");
        //                }

        //                retstr.Add($"    Value: '{field.ValueText.Text}");
        //                retstr.Add($"    Confidence: '{field.Confidence}");
        //            }
        //        }

        //        return retstr;

        //    }
        //    catch (Exception ex)
        //    {
        //        retstr.Add("<h3>" + ex .Message + "</h3>");
        //        return retstr;

        //    }
        //}

        //public async Task ManageModels(string trainingFileUrl)
        //{
        //    var trainingClient = new FormTrainingClient(new Uri(endpoint), credential);

        //    AccountProperties accountProperties = trainingClient.GetAccountProperties();
        //    Console.WriteLine($"Account has {accountProperties.CustomModelCount} models.");
        //    Console.WriteLine($"It can have at most {accountProperties.CustomModelLimit}" +
        //        $" models.");

        //    // List the first ten or fewer models currently stored in the account.
        //    Pageable<CustomFormModelInfo> models = trainingClient.GetModelInfos();

        //    foreach (CustomFormModelInfo modelInfo in models.Take(10))
        //    {
        //        Console.WriteLine($"Custom Model Info:");
        //        Console.WriteLine($"    Model Id: {modelInfo.ModelId}");
        //        Console.WriteLine($"    Model Status: {modelInfo.Status}");
        //        Console.WriteLine($"    Created On: {modelInfo.CreatedOn}");
        //        Console.WriteLine($"    Last Modified: {modelInfo.LastModified}");
        //    }

        //    // Create a new model to store in the account
        //    CustomFormModel model = await trainingClient.StartTrainingAsync(
        //        new Uri(trainingFileUrl)).WaitForCompletionAsync();

        //    // Get the model that was just created
        //    CustomFormModel modelCopy = trainingClient.GetCustomModel(model.ModelId);

        //    Console.WriteLine($"Custom Model {modelCopy.ModelId} recognizes the following" +
        //        " form types:");

        //    foreach (CustomFormSubModel subModel in modelCopy.Models)
        //    {
        //        Console.WriteLine($"SubModel Form Type: {subModel.FormType}");
        //        foreach (CustomFormModelField field in subModel.Fields.Values)
        //        {
        //            Console.Write($"    FieldName: {field.Name}");
        //            if (field.Label != null)
        //            {
        //                Console.Write($", FieldLabel: {field.Label}");
        //            }
        //            Console.WriteLine("");
        //        }
        //    }

        //    // Delete the model from the account.
        //    //trainingClient.DeleteModel(model.ModelId);
        //}

        private string GenID()
        {
            //https://stackoverflow.com/questions/11313205/generate-a-unique-id
            //https://codereview.stackexchange.com/questions/233452/generate-unique-id-in-c
            //StringBuilder builder = new StringBuilder();
            //Enumerable
            //   .Range(65, 26)
            //    .Select(e => ((char)e).ToString())
            //    .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
            //    .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
            //    .OrderBy(e => Guid.NewGuid())
            //    .Take(6)
            //    .ToList().ForEach(e => builder.Append(e));
            //string id = builder.ToString();

            string id = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            return id;
        }

    }
}
