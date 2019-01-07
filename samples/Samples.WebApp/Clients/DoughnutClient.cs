using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Samples.WebApp.Clients
{
    public interface IDoughnutClient
    {
        Task<List<Doughnut>> GetAll();
    }

    public class DoughnutClient : IDoughnutClient
    {
        private readonly HttpClient _httpClient;

        public DoughnutClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5001");
        }

        public async Task<List<Doughnut>> GetAll()
        {
            var httpResponseMessage = await _httpClient.GetAsync("api/doughnut");
            httpResponseMessage.EnsureSuccessStatusCode();
            var doughnut = await httpResponseMessage.Content.ReadAsAsync<List<Doughnut>>();
            return doughnut;
        }
    }

    public class Doughnut
    {
        public string Id { get; set; }
        public string Color { get; set; }
        public int Price { get; set; }
        public int OwnerId { get; set; }
    }
}
