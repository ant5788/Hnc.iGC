using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Hnc.iGC
{
    public class HnciGCApiServiceTemperBox : IHnciGCApiTemperBox
    {
        private readonly HnciGCApiOptions options;

        public HttpClient Client { get; }

        public HnciGCApiServiceTemperBox(HttpClient client, HnciGCApiOptions options)
        {
            client.BaseAddress = new Uri(options.BaseAddress);
            Client = client;
            this.options = options;
        }

        public async Task<IEnumerable<TemperBoxDto>> GetTemperBoxs()
        {
            return await Client.GetFromJsonAsync<IEnumerable<TemperBoxDto>>(options.GetTemperBox) ?? new List<TemperBoxDto>();
        }

        public async Task<TemperBoxDto> GetTemperBox(string deviceId)
        {
            return await Client.GetFromJsonAsync<TemperBoxDto>($"{options.GetTemperBox}/{deviceId}") ?? new TemperBoxDto { DeviceId = deviceId };
        }

        public async Task<TemperBoxDto> PostTemperBox(TemperBoxDto dto)
        {
            var response = await Client.PostAsJsonAsync(options.PostTemperBox, dto);
            return await response.Content.ReadFromJsonAsync<TemperBoxDto>() ?? dto;
        }

        public async Task DeleteTemperBox(string deviceId)
        {
            await Client.DeleteAsync($"{options.DeleteTemperBox}/{deviceId}");
        }
    }
}
