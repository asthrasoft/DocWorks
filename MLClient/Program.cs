//https://azure.github.io/learnAnalytics-UsingAzureMachineLearningforAIWorkloads/lab08-consuming_a_scoring_service/0_README.html
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace MLClient
{
    class Program
    {
        // Web Service URL (update it with your service url)
        //private const string SERVICE_URL = "http://<service ip address>:80/api/v1/service/<service name>/score";
        //private const string API_KEY = "your service key";

        private const string SERVICE_URL = "http://52091d57-3240-4e9a-82d6-1a61bf468ad3.centralus.azurecontainer.io/score";
        private const string API_KEY = "VNknm4mIjCew38moNLsaSXEbBdxd8Lrg";

        static void Main(string[] args)
        {
            Program.PostRequest();
            Console.ReadLine();
        }

        private static void PostRequest()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(SERVICE_URL);
            //For local web service, comment out this line.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);

            var inputJson = new List<RequestPayload>();
            RequestPayload payload = new RequestPayload();
            List<InputDf> inputDf = new List<InputDf>();
            inputDf.Add(new InputDf()
            {
                feature1 = 23.4,
                feature2 = 37.2,
            });
            payload.Input_df_list = inputDf;
            inputJson.Add(payload);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                request.Content = new StringContent(JsonConvert.SerializeObject(payload));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = client.SendAsync(request).Result;

                Console.Write(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
            }
        }
        public class InputDf
        {
            public double feature1 { get; set; }
            public double feature2 { get; set; }
        }
        public class RequestPayload
        {
            [JsonProperty("input_df")]
            public List<InputDf> Input_df_list { get; set; }
        }
    }
}
