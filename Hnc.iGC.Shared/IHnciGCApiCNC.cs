using System.Collections.Generic;
using System.Threading.Tasks;

using Refit;

namespace Hnc.iGC
{
    public interface IHnciGCApiCNC
    {
        [Get("/api/CNC")]
        Task<IEnumerable<CNCDto>> GetCNCs();

        [Get("/api/CNC/{deviceId}")]
        Task<CNCDto> GetCNC(string deviceId);

        [Post("/api/CNC")]
        Task<CNCDto> PostCNC([Body] CNCDto dto);

        [Delete("/api/CNC/{deviceId}")]
        Task DeleteCNC(string deviceId);
    }
}
