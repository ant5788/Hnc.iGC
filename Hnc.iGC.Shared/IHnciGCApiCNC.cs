using System.Collections.Generic;
using System.Threading.Tasks;

using Refit;

namespace Hnc.iGC
{
    // 以上⽅法定义⼀个 REST API 接⼝，该接⼝定义了 GetCNC 函数，
    // 该函数通过 HTTP GET 请求去访问服务器的 "/api/CNC/{deviceId}" 路径并把返回的结果封装为 CNCDto 对象返回
    // ，其中 URL 路径中 {deviceId} 的值为 GetCNC 函数的 deviceId 参数取值，
    // 然后，通过 RestService 类⽣成IGitHubApi 的代理实现，通过代理直接调⽤ Web API 接⼝。
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
