using Refit;

namespace Hnc.iGC
{
    public interface IHnciGCApiBalancer
    {
        [Get("/api/Balancer")]
        Task<IEnumerable<BalancerDto>?> GetBalancers();

        [Get("/api/Balancer/{deviceId}")]
        Task<BalancerDto?> GetBalancer(string deviceId);

        [Post("/api/Balancer")]
        Task<BalancerDto?> PostBalancer([Body] BalancerDto dto);

        [Delete("/api/Balancer/{deviceId}")]
        Task DeleteBalancer(string deviceId);
    }
}
