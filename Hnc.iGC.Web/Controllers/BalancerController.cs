using AutoMapper;

using Hnc.iGC.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hnc.iGC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalancerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BalancerController> logger;

        public BalancerController(ApplicationDbContext context, IMapper mapper, ILogger<BalancerController> logger)
        {
            _context = context;
            _mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// 读取Balancer数据
        /// </summary>
        /// <returns>设备Dto</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BalancerDto>>> GetBalancers()
        {
            try
            {
                var dData = await _context.Balancers.Where(p => p.CreationTime < DateTime.Now.AddHours(-1)).ToListAsync();
                _context.Balancers.RemoveRange(dData);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除一小时前的CNC数据出错");
            }

            var ids = await _context.Balancers.Select(s => s.DeviceId).Distinct().ToListAsync();
            var entities = ids.Select(deviceId =>
                    _context.Balancers
                    .Where(p => p.DeviceId == deviceId)
                    .OrderByDescending(p => p.CreationTime)
                    .FirstOrDefault())
                    .OrderBy(o => o?.Name).ToList();
            return _mapper.Map<List<BalancerDto>>(entities);
        }

        /// <summary>
        /// 读取Balancer数据
        /// </summary>
        /// <param name="deviceId">设备Id</param>
        /// <returns>设备Dto</returns>
        [HttpGet("{deviceId}")]
        public async Task<ActionResult<BalancerDto>> GetBalancer(string deviceId)
        {
            var entity = await _context.Balancers
                       .Where(p => p.DeviceId == deviceId)
                       .OrderByDescending(p => p.CreationTime)
                       .FirstOrDefaultAsync();
            return _mapper.Map<BalancerDto>(entity);
        }

        /// <summary>
        /// 创建或更新Balancer数据（覆盖）
        /// </summary>
        /// <param name="dto">设备Dto</param>
        /// <returns>设备Dto</returns>
        [HttpPost]
        public async Task<ActionResult<BalancerDto>> PostBalancer(BalancerDto dto)
        {
            var entity = await _context.Balancers
                       .OrderByDescending(p => p.CreationTime)
                       .FirstOrDefaultAsync(p => p.DeviceId == dto.DeviceId);
            entity = _mapper.Map<Balancer>(dto);

            _context.Balancers.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBalancer), new { deviceId = dto.DeviceId }, dto);
        }

        /// <summary>
        /// 删除Balancer
        /// </summary>
        /// <param name="deviceId">设备Id</param>
        /// <returns></returns>
        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteBalancer(string deviceId)
        {
            var cncs = await _context.Balancers
                       .Where(p => p.DeviceId == deviceId)
                       .ToListAsync();

            if (cncs == null)
            {
                return NotFound();
            }

            _context.Balancers.RemoveRange(cncs);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
