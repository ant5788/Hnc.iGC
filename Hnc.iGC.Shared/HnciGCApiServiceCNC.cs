using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Hnc.iGC
{
    public class HnciGCApiServiceCNC : IHnciGCApiCNC
    {
        private readonly HnciGCApiOptions options;

        public HttpClient Client { get; }

        public HnciGCApiServiceCNC(HttpClient client, HnciGCApiOptions options)
        {
            client.BaseAddress = new Uri(options.BaseAddress);
            Client = client;
            this.options = options;
        }

        public async Task<IEnumerable<CNCDto>> GetCNCs()
        {
            return await Client.GetFromJsonAsync<IEnumerable<CNCDto>>(options.GetCNC) ?? new List<CNCDto>();
        }

        public async Task<CNCDto> GetCNC(string deviceId)
        {
            return await Client.GetFromJsonAsync<CNCDto>($"{options.GetCNC}/{deviceId}") ?? new CNCDto { DeviceId = deviceId };
        }

        public async Task<CNCDto> PostCNC(CNCDto dto)
        {
            var response = await Client.PostAsJsonAsync(options.PostCNC, dto);
            return await response.Content.ReadFromJsonAsync<CNCDto>() ?? dto;
        }

        public async Task DeleteCNC(string deviceId)
        {
            await Client.DeleteAsync($"{options.DeleteCNC}/{deviceId}");
        }
    }
}
