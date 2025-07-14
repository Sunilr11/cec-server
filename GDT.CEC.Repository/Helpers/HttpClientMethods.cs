using GDT.CEC.Repository.Models.HttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository.Helpers
{
    public class HttpClientMethods
    {
        public static async Task<HttpResponseMessage> PostDataAsync(HttpClientRequestModel oRequest, Dictionary<string,string> PostData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(oRequest.BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(oRequest.ResponseDataType));

                if (!string.IsNullOrEmpty(oRequest.AuthToken))
                    client.DefaultRequestHeaders.Add("Authorization", oRequest.AuthTokenType + " " + oRequest.AuthToken);

                HttpContent content = new FormUrlEncodedContent(PostData);
                HttpResponseMessage response = await client.PostAsync(oRequest.MethodNameOrUrl, content);
                return response;
            }
        }
        public static async Task<HttpResponseMessage>PostDataAsync(HttpClientRequestModel oRequest, object postData)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(oRequest.BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(oRequest.ResponseDataType));

                if (!string.IsNullOrEmpty(oRequest.AuthToken))
                    client.DefaultRequestHeaders.Add("Authorization", oRequest.AuthTokenType + " " + oRequest.AuthToken);

                var jsonRequest = JsonConvert.SerializeObject(postData);
                HttpContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response =await client.PostAsync(oRequest.MethodNameOrUrl, content);
                
                return response;
            }
        }
        public static async Task<HttpResponseMessage> GetDataAsync(HttpClientRequestModel oRequest)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(oRequest.BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(oRequest.ResponseDataType));

                if (!string.IsNullOrEmpty(oRequest.AuthToken))
                    client.DefaultRequestHeaders.Add("Authorization", oRequest.AuthTokenType + " " + oRequest.AuthToken);

                HttpResponseMessage response = await client.GetAsync(oRequest.MethodNameOrUrl);

                return response;
            }
        }
        public static async Task<HttpResponseMessage> PutDataAsync(HttpClientRequestModel oRequest, object postData)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(oRequest.BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(oRequest.ResponseDataType));

                if (!string.IsNullOrEmpty(oRequest.AuthToken))
                    client.DefaultRequestHeaders.Add("Authorization", oRequest.AuthTokenType + " " + oRequest.AuthToken);

                var jsonRequest = JsonConvert.SerializeObject(postData);
                HttpContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(oRequest.MethodNameOrUrl, content);

                return response;
            }
        }
    }
}
