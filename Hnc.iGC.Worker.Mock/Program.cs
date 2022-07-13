using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace Hnc.iGC.Worker
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", true, true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();

            try
            {
                Log.Information("采集程序启动");
                await Collector.CreateHostBuilder<MockCollector, CNCDto>(args).RunConsoleAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "采集程序中断");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    internal class MockCollector : Collector<CNCDto>
    {
        public override string Protocal { get; } = "Mock";

        public override void SetDataTo(CNCDto dto)
        {
            Random random = new();
            dto.RunState = random.Next(0, 3) switch
            {
                0 => "循环启动",
                1 => "进给保持",
                2 => "急停",
                3 => "待机",
                _ => "",
            };
            dto.WorkMode = random.Next(0, 6) switch
            {
                0 => "手动",
                1 => "自动",
                2 => "增量",
                3 => "单段",
                4 => "回零",
                5 => "MDI",
                _ => "",
            };
            dto.Emergency = random.Next() % 2 == 0;
            dto.Alarm = random.Next() % 2 == 0;
            if (dto.Alarm)
            {
                var length = random.Next(3);
                for (int i = 0; i < length; i++)
                {
                    dto.AlarmMessages.Add(new CNCDto.AlarmMessageDto
                    {
                        Number = $"CA{i:D4}",
                        Message = Guid.NewGuid().ToString(),
                        StartAt = DateTime.Now
                    });
                }
            }
            dto.CurrentProgramName = $"O{random.Next(1000, 10000)}";
            dto.CurrentProgramNumber = random.Next(1000, 10000);
            dto.CurrentProgramLineNumber = random.Next(1000);
            dto.CurrentProgramContent =
@"G18 G40 G71 G90
G54
LIMS=3500
G0 X250 Z250 D0
G0 X250 Z250 D0
M30";
            dto.CurrentCutterNumber = random.Next(10);

            dto.FeedSpeed = random.NextDouble() * random.Next(100);
            dto.FeedSpeedUnit = "mm/min";
            dto.FeedOverride = random.Next(13) * 10;
            dto.SpindleSpeed = (random.Next() % 2 == 0 ? 1 : -1) * random.NextDouble() * random.Next(100);
            dto.SpindleSpeedUnit = "r/min";
            dto.SpindleOverride = random.Next(13) * 10;
            dto.SpindleLoad = random.Next(20);
            dto.Spindles.Add(new CNCDto.SpindleDto
            {
                Load = dto.SpindleLoad,
                Speed = dto.SpindleSpeed,
            });

            var axisNames = new string[] { "X", "Y", "Z", "A", "B", "C", "U", "V", "W" };
            for (int i = 0; i < 3; i++)
            {
                dto.Axes.Add(new CNCDto.AxisDto
                {
                    Name = axisNames[i],
                    Absolute = random.NextDouble() * random.Next(1000),
                    Relative = random.NextDouble() * random.Next(1000),
                    Machine = random.NextDouble() * random.Next(1000),
                    Distance = random.NextDouble() * random.Next(1000),
                    Load = random.Next(20)
                });
            }

            for (int i = 0; i < 10; i++)
            {
                dto.CutterInfos.Add(new CNCDto.CutterInfoDto
                {
                    Number = i + 1,
                    LengthSharpOffset = random.NextDouble() * random.Next(10),
                    LengthWearOffset = random.NextDouble() * random.Next(10),
                    RadiusSharpOffset = random.NextDouble() * random.Next(10),
                    RadiusWearOffset = random.NextDouble() * random.Next(10),
                });
            }

            dto.RapidOverride = random.Next(13) * 10;

            dto.SystemTime = DateTime.Now;
            dto.PartsCount = random.Next(1000, 10000);
            dto.PartsTotal = dto.PartsCount * random.Next(2, 5);
            dto.PartsRequired = dto.PartsCount * random.Next(1, 4);

            dto.TimePowerOn = TimeSpan.FromSeconds(random.Next(86400)).ToString();
            dto.TimeOperating = TimeSpan.FromSeconds(random.Next(86400)).ToString();
            dto.TimeCutting = TimeSpan.FromSeconds(random.Next(86400)).ToString();
            dto.TimeCycle = TimeSpan.FromSeconds(random.Next(86400)).ToString();
        }
    }
}
