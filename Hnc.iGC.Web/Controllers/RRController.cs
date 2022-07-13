using Hnc.iGC.Web.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

using System.Text;
using System.Text.Json;

namespace Hnc.iGC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RRController : ControllerBase
    {
        public RRController(IFileProvider fileProvider, IConfiguration configuration)
        {
            FileProvider = fileProvider;
            Configuration = configuration;
            StoredFilesPath = Configuration[nameof(StoredFilesPath)] ?? @"C:\ProgramData\Hnc\iGC\CNC_Files";
        }

        public IFileProvider FileProvider { get; }

        public IConfiguration Configuration { get; }

        public string StoredFilesPath { get; }

        [HttpGet("Layout")]
        public async Task<string> GetLayout()
        {
            var file = FileProvider.GetFileInfo("rr_layout.json");
            if (file.Exists)
            {
                using var s = file.CreateReadStream();
                using var sr = new StreamReader(s);
                return await sr.ReadToEndAsync();
            }
            else
            {
                List<RRLayoutItem> layoutItems = new()
                {
                    new RRLayoutItem("RESET") { Id = 1, LightOn = "D2.1=1", LightOff = "D2.1=0", KeyDown = "D2.0=1", KeyUp = "D2.0=0" },
                    new RRLayoutItem("单段") { Id = 2, LightOn = "D10.1=1", LightOff = "D10.1=0", KeyDown = "D10.0=1", KeyUp = "D10.0=0" },
                    new RRLayoutItem("选择停止") { Id = 3, LightOn = "D10.3=1", LightOff = "D10.3=0", KeyDown = "D10.2=1", KeyUp = "D10.2=0" },
                    new RRLayoutItem("跳段") { Id = 4, LightOn = "D10.5=1", LightOff = "D10.5=0", KeyDown = "D10.4=1", KeyUp = "D10.4=0" },
                    new RRLayoutItem("空运行") { Id = 5, LightOn = "D10.7=1", LightOff = "D10.7=0", KeyDown = "D10.6=1", KeyUp = "D10.6=0" },
                    new RRLayoutItem("Z轴锁定") { Id = 6, LightOn = "D10.9=1", LightOff = "D10.9=0", KeyDown = "D10.8=1", KeyUp = "D10.8=0" },
                    new RRLayoutItem("MST锁定") { Id = 7, LightOn = "D10.11=1", LightOff = "D10.11=0", KeyDown = "D10.10=1", KeyUp = "D10.10=0" },
                    new RRLayoutItem("手摇试切") { Id = 8, LightOn = "D10.13=1", LightOff = "D10.13=0", KeyDown = "D10.12=1", KeyUp = "D10.12=0" },
                    new RRLayoutItem("防护门") { Id = 9, LightOn = "D10.15=1", LightOff = "D10.15=0", KeyDown = "D10.14=1", KeyUp = "D10.14=0" },
                    new RRLayoutItem("冷却") { Id = 10, LightOn = "D10.17=1", LightOff = "D10.17=0", KeyDown = "D10.16=1", KeyUp = "D10.16=0" },
                    new RRLayoutItem("中心出水") { Id = 11, LightOn = "D10.19=1", LightOff = "D10.19=0", KeyDown = "D10.18=1", KeyUp = "D10.18=0" },
                    new RRLayoutItem("吹气") { Id = 12, LightOn = "D10.21=1", LightOff = "D10.21=0", KeyDown = "D10.20=1", KeyUp = "D10.20=0" },
                    new RRLayoutItem("后排冲水") { Id = 13, LightOn = "D10.23=1", LightOff = "D10.23=0", KeyDown = "D10.22=1", KeyUp = "D10.22=0" },
                    new RRLayoutItem("主轴正转") { Id = 14, LightOn = "D10.25=1", LightOff = "D10.25=0", KeyDown = "D10.24=1", KeyUp = "D10.24=0" },
                    new RRLayoutItem("主轴停止") { Id = 15, LightOn = "D10.27=1", LightOff = "D10.27=0", KeyDown = "D10.26=1", KeyUp = "D10.26=0" },
                    new RRLayoutItem("主轴反转") { Id = 16, LightOn = "D10.29=1", LightOff = "D10.29=0", KeyDown = "D10.28=1", KeyUp = "D10.28=0" },
                    new RRLayoutItem("主轴定向") { Id = 17, LightOn = "D10.31=1", LightOff = "D10.31=0", KeyDown = "D10.30=1", KeyUp = "D10.30=0" },
                    new RRLayoutItem("排屑正传") { Id = 18, LightOn = "D11.1=1", LightOff = "D11.1=0", KeyDown = "D11.0=1", KeyUp = "D11.0=0" },
                    new RRLayoutItem("刀库调试") { Id = 19, LightOn = "D11.3=1", LightOff = "D11.3=0", KeyDown = "D11.2=1", KeyUp = "D11.2=0" },
                    new RRLayoutItem("刀库正转") { Id = 20, LightOn = "D11.5=1", LightOff = "D11.5=0", KeyDown = "D11.4=1", KeyUp = "D11.4=0" },
                    new RRLayoutItem("工作灯") { Id = 21, LightOn = "D11.7=1", LightOff = "D11.7=0", KeyDown = "D11.6=1", KeyUp = "D11.6=0" },
                    new RRLayoutItem("排屑反转") { Id = 22, LightOn = "D11.9=1", LightOff = "D11.9=0", KeyDown = "D11.8=1", KeyUp = "D11.8=0" },
                    new RRLayoutItem("机手动作") { Id = 23, LightOn = "D11.11=1", LightOff = "D11.11=0", KeyDown = "D11.10=1", KeyUp = "D11.10=0" },
                    new RRLayoutItem("刀库反转") { Id = 24, LightOn = "D11.13=1", LightOff = "D11.13=0", KeyDown = "D11.12=1", KeyUp = "D11.12=0" },
                    new RRLayoutItem("润滑") { Id = 25, LightOn = "D11.15=1", LightOff = "D11.15=0", KeyDown = "D11.14=1", KeyUp = "D11.14=0" },
                };
                var text = JsonSerializer.Serialize(layoutItems, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true,
                });
                await System.IO.File.WriteAllTextAsync(Path.Combine(StoredFilesPath, "rr_layout.json"), text, Encoding.UTF8);
                return text;
            }
        }
    }
}
