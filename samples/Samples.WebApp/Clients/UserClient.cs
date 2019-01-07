using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Samples.WebApp.Clients
{
    public interface IUserClient
    {
        Task<User> GetById(int id);
    }

    public class UserClient : IUserClient
    {
        private readonly HttpClient _httpClient;

        public UserClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5002");

        }

        public async Task<User> GetById(int id)
        {
            var httpResponseMessage = await _httpClient.GetAsync($"api/user/{id}");
            httpResponseMessage.EnsureSuccessStatusCode();
            var doughnut = await httpResponseMessage.Content.ReadAsAsync<User>();
            return doughnut;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
