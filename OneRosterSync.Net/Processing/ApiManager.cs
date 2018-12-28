﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneRosterSync.Net.Extensions;
using OneRosterSync.Net.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OneRosterSync.Net.Processing
{
    public class ApiManager
    {
        HttpClient client;
        ILogger Logger;

        public ApiManager(ILogger logger)
        {
            // "https://localhost:44312/api/MockApi/update";
            Logger = logger;

            client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44312/api/mockapi/")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        }

        public async Task<ApiResponse> Post(string entity, object data) 
        {
            try
            {
                // create post data
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // submit request
                Logger.Here().LogInformation($"Posting\n {json}");
                HttpResponseMessage response = await client.PostAsync(entity, content);

                // retrieve response
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Logger.Here().LogInformation($"Response: {response.StatusCode}\n {responseBody}");

                // parse response
                ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                return result;
            }
            catch (Exception ex)
            {
                string message = $"Error communicating with LMS\n {ex.Message}";
                Logger.Here().LogInformation(message);
                return new ApiResponse { Success = false, ErrorMessage = message };
            }
        }
    }
}