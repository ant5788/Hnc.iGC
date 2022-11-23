using System.Collections.Generic;
using System.Threading.Tasks;

using Refit;

namespace Hnc.iGC
{
    public interface IHnciGCApiTemperBox
    {
        [Get("/api/TemperBox")]
        Task<IEnumerable<TemperBoxDto>> GetTemperBoxs();

        [Get("/api/TemperBox/{deviceId}")]
        Task<TemperBoxDto> GetTemperBox(string deviceId);

        [Post("/api/TemperBox")]
        Task<TemperBoxDto> PostTemperBox([Body] TemperBoxDto dto);

        [Delete("/api/TemperBox/{deviceId}")]
        Task DeleteTemperBox(string deviceId);
    }
}
