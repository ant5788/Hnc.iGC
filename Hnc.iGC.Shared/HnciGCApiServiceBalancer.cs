using System.Net.Http.Json;

namespace Hnc.iGC
{
    public class HnciGCApiServiceBalancer : IHnciGCApiBalancer
    {
        private readonly HnciGCApiOptions options;

        public HttpClient Client { get; }

        public HnciGCApiServiceBalancer(HttpClient client, HnciGCApiOptions options)
        {
            client.BaseAddress = new Uri(options.BaseAddress);
            Client = client;
            this.options = options;
        }
        public async Task<IEnumerable<BalancerDto>?> GetBalancers()
        {
            return await Client.GetFromJsonAsync<IEnumerable<BalancerDto>>(options.GetBalancer);
        }

        public async Task<BalancerDto?> GetBalancer(string deviceId)
        {
            return await Client.GetFromJsonAsync<BalancerDto>($"{options.GetBalancer}/{deviceId}");
        }

        public async Task<BalancerDto?> PostBalancer(BalancerDto dto)
        {
            var response = await Client.PostAsJsonAsync(options.PostBalancer, dto);
            return await response.Content.ReadFromJsonAsync<BalancerDto>();
        }

        public async Task DeleteBalancer(string deviceId)
        {
            await Client.DeleteAsync($"{options.DeleteBalancer}/{deviceId}");
        }
    }
}
