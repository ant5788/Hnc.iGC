using FluentFTP;

using Hnc.iGC.Models;

using Microsoft.EntityFrameworkCore;

using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace Hnc.iGC.Web.Worker
{
    public class CNCProgWorker : BackgroundService
    {
        public CNCProgWorker(ILogger<CNCProgWorker> logger,
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            IConfiguration configuration)
        {
            Logger = logger;
            DbContextFactory = dbContextFactory;
            Configuration = configuration;
            StoredFilesPath = Configuration[nameof(StoredFilesPath)];
        }

        public ILogger<CNCProgWorker> Logger { get; }
        public IDbContextFactory<ApplicationDbContext> DbContextFactory { get; }
        public IConfiguration Configuration { get; }
        public string StoredFilesPath { get; }

        private List<HnciGSRequest> HnciGSRequests { get; set; } = new();
        private List<HnciGSResponse> HnciGSResponses { get; set; } = new();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var dbContext = DbContextFactory.CreateDbContext();

                var cncs = await dbContext.CNCs.Select(s => s.DeviceId).Distinct().Select(deviceId =>
                     dbContext.CNCs.Where(p => p.DeviceId == deviceId).OrderByDescending(p => p.CreationTime).First())
                    .OrderBy(o => o.Name).AsNoTracking().ToListAsync(stoppingToken);

                foreach (var cncDest in cncs)
                {
                    var cncPath = Path.Combine(StoredFilesPath, $"{cncDest.Name}__{cncDest.IP}");
                    var cncRequestFilePath = Path.Combine(cncPath, "iGSRequest.SV");
                    var cncResponseFilePath = Path.Combine(cncPath, "iGSResponse.SV");
                    var cncProgPath = Path.Combine(cncPath, "prog");
                    var progmapFilePath = Path.Combine(cncProgPath, "progmap.xml");

                    try
                    {
                        //FtpClient client = new(cncDest.IP, 10021, "root", "111111");
                        using FtpClient client = new("192.168.30.30", 10021, "root", "111111");
                        await client.ConnectAsync(stoppingToken);

                        await GetGprogAsync(dbContext, cncs, cncDest, cncProgPath, progmapFilePath, client, stoppingToken);

                        await ProcessRequest(dbContext, cncs, cncRequestFilePath, cncResponseFilePath, client, stoppingToken);

                        await client.DisconnectAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "FTP操作失败！|设备名称：{name}|设备IP：{ip}", cncDest.Name, cncDest.IP);
                    }

                }
                await Task.Delay(2000, stoppingToken);
            }
        }

        /// <summary>
        /// 处理G代码传输请求
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="cncs"></param>
        /// <param name="cncRequestFilePath"></param>
        /// <param name="cncResponseFilePath"></param>
        /// <param name="client"></param>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task ProcessRequest(ApplicationDbContext dbContext, List<CNC> cncs, string cncRequestFilePath, string cncResponseFilePath, FtpClient client, CancellationToken stoppingToken)
        {
            var remoteRequestFilePath = "/h/lnc8/tmp/iGS/iGSRequest.SV";
            var ftpStatus = await client.DownloadFileAsync(cncRequestFilePath, remoteRequestFilePath, FtpLocalExists.Overwrite, FtpVerify.Retry, token: stoppingToken);
            if (ftpStatus == FtpStatus.Success)
            {
                Logger.LogInformation("读取[{remoteRequestFilePath}]成功:[{ftpStatus}]！", remoteRequestFilePath, ftpStatus);
                HnciGSRequests = GetiGSRequests(cncRequestFilePath);
                for (int i = 0; i < HnciGSRequests.Count; i++)
                {
                    var request = HnciGSRequests[i];
                    if (HnciGSResponses.Any(p => p.No == request.No && p.Type == request.Type))
                    {
                        Logger.LogDebug("请求编号[{no}]，请求类型[{type}]，已经在处理", request.No, request.Type);
                    }
                    else
                    {
                        iGC.Models.File? file = await dbContext.Files.FindAsync(new object?[] { request.Id }, cancellationToken: stoppingToken);
                        if (file == null)
                        {
                            Logger.LogError("文件[{id}]不存在！", request.Id);
                        }
                        else
                        {
                            var destCNC = cncs.FirstOrDefault(p => p.Name == request.Dest);
                            if (destCNC == null)
                            {
                                Logger.LogError("目标机床[{dest}]不存在！", request.Dest);
                            }
                            else
                            {
                                var response = new HnciGSResponse(request);
                                using FtpClient sClient = new(destCNC.IP, 10021, "root", "111111");
                                await sClient.ConnectAsync(stoppingToken);
                                var remoteFilePath = Path.Combine("/h/lnc8/tmp/iGS/prog", file.RemoteFileName);
                                _ = sClient.UploadFileAsync(
                                    file.LocalFullPath,
                                    remoteFilePath,
                                    FtpRemoteExists.Overwrite,
                                    true,
                                    FtpVerify.Retry,
                                    //new HnciGSProgress(response),
                                    new Progress<FtpProgress>(value =>
                                    {
                                        if (value.Progress == -1)
                                        {
                                            response.Progress = byte.MaxValue;
                                        }
                                        else
                                        {
                                            response.Progress = (byte)Math.Floor(value.Progress);
                                        }
                                        response.SendTime = (uint)CNC_Statistic.GetUnixTimestamp(DateTime.Now);
                                    }),
                                    stoppingToken);
                                HnciGSResponses.Add(response);
                            }
                        }
                    }
                }
            }
            else
            {
                Logger.LogError("读取[{remoteRequestFilePath}]失败:[{ftpStatus}]！", remoteRequestFilePath, ftpStatus);
            }

            var data = new List<byte>();
            foreach (var item in HnciGSResponses)
            {
                var bytes = item.GetDataBytes();
                for (int i = 0; i < bytes.Length; i++)
                {
                    data.Add(bytes[i]);
                }
            }
            await System.IO.File.WriteAllBytesAsync(cncResponseFilePath, data.ToArray(), stoppingToken);

            var remoteResponseFilePath = "/h/lnc8/tmp/iGS/iGSResponse.SV";
            ftpStatus = await client.UploadFileAsync(cncResponseFilePath, remoteResponseFilePath, token: stoppingToken);
            if (ftpStatus == FtpStatus.Success)
            {
                Logger.LogInformation("发送[{remoteResponseFilePath}]成功:[{ftpStatus}]！", remoteResponseFilePath, ftpStatus);
            }
            else
            {
                Logger.LogError("发送[{remoteResponseFilePath}]失败:[{ftpStatus}]！", remoteResponseFilePath, ftpStatus);
            }
        }

        public class HnciGSRequest
        {
            public HnciGSRequest(byte[] bytes)
            {
                No = BitConverter.ToInt32(bytes, 0);
                Type = BitConverter.ToUInt32(bytes, 4);
                Origin = BitConverter.ToString(bytes, 8);
                Id = BitConverter.ToString(bytes, 18);
                Dest = BitConverter.ToString(bytes, 54);
                SendTime = BitConverter.ToUInt32(bytes, 64);
            }

            public int No { get; set; }

            public uint Type { get; set; }

            public string Origin { get; set; }

            public string Id { get; set; }

            public string Dest { get; set; }

            public uint SendTime { get; set; }
        }

        public class HnciGSResponse
        {
            public HnciGSResponse(HnciGSRequest request)
            {
                No = request.No;
                Type = request.Type;
                Origin = request.Origin;
                Id = request.Id;
                Dest = request.Dest;
                SendTime = request.SendTime;
                Progress = 0;
            }

            public int No { get; set; }

            public uint Type { get; set; }

            public string Origin { get; set; }

            public string Id { get; set; }

            public string Dest { get; set; }

            public uint SendTime { get; set; }

            public byte Progress { get; set; }

            public byte[] GetDataBytes()
            {
                byte[] data = new byte[69];

                var bs1 = BitConverter.GetBytes(No);
                Array.Reverse(bs1);
                Array.Copy(bs1, 0, data, 0, bs1.Length);

                var bs2 = BitConverter.GetBytes(Type);
                Array.Reverse(bs2);
                Array.Copy(bs2, 0, data, 4, bs2.Length);

                Array.Copy(Encoding.ASCII.GetBytes(Origin.PadRight(10)), 0, data, 8, 10);

                Array.Copy(Encoding.ASCII.GetBytes(Id.PadRight(36)), 0, data, 18, 36);

                Array.Copy(Encoding.ASCII.GetBytes(Dest.PadRight(10)), 0, data, 54, 10);

                var bs3 = BitConverter.GetBytes(SendTime);
                Array.Reverse(bs3);
                Array.Copy(bs2, 0, data, 64, bs3.Length);

                data[68] = Progress;

                return data;
            }
        }

        public class HnciGSProgress : IProgress<FtpProgress>
        {
            public HnciGSProgress(HnciGSResponse response)
            {
                Response = response;
            }

            public HnciGSResponse Response { get; set; }

            public void Report(FtpProgress value)
            {
                if (value.Progress == -1)
                {
                    Response.Progress = byte.MaxValue;
                }
                else
                {
                    Response.Progress = (byte)Math.Floor(value.Progress);
                }
                Response.SendTime = (uint)CNC_Statistic.GetUnixTimestamp(DateTime.Now);
            }
        }

        private List<HnciGSRequest> GetiGSRequests(string cncRequestFilePath)
        {
            List<HnciGSRequest> requests = new();
            try
            {
                using var sr = new BinaryReader(new FileStream(cncRequestFilePath, FileMode.OpenOrCreate));
                byte[] buffer = new byte[68];
                int len = 0;
                while ((len = sr.Read(buffer)) != 0)
                {
                    var ptr = Marshal.AllocHGlobal(buffer.Length);
                    try
                    {
                        Marshal.Copy(buffer, 0, ptr, buffer.Length);
                        HnciGSRequest request = Marshal.PtrToStructure<HnciGSRequest>(ptr);
                        requests.Add(request);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "解析请求文件[{cncRequestFilePath}]失败", cncRequestFilePath);
            }
            return requests;
        }

        /// <summary>
        /// 读取G代码
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="cncs"></param>
        /// <param name="cncDest"></param>
        /// <param name="cncProgPath"></param>
        /// <param name="progmapFilePath"></param>
        /// <param name="client"></param>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task GetGprogAsync(ApplicationDbContext dbContext, List<CNC> cncs, CNC cncDest, string cncProgPath, string progmapFilePath, FtpClient client, CancellationToken stoppingToken)
        {
            var ftpListItem = await client.GetObjectInfoAsync("/h/lnc8/prog", token: stoppingToken);
            await ProcessFtpListItemsAsync(dbContext, cncDest.DeviceId, client, cncProgPath, stoppingToken, ftpListItem);
            await dbContext.SaveChangesAsync(stoppingToken);

            if (await UpdateProgMapAsync(dbContext, cncs, progmapFilePath, stoppingToken))
            {
                var remoteProgmapFilePath = "/h/lnc8/tmp/iGS/progmap.xml";
                var ftpStatus = await client.UploadFileAsync(progmapFilePath, remoteProgmapFilePath, createRemoteDir: true, token: stoppingToken);
                if (ftpStatus == FtpStatus.Success)
                {
                    Logger.LogInformation("上传[{localPath}]到远程[{remotePath}]成功:[{ftpStatus}]！", progmapFilePath, remoteProgmapFilePath, ftpStatus);
                }
                else
                {
                    Logger.LogError("上传[{localPath}]到远程[{remotePath}]失败:[{ftpStatus}]！", progmapFilePath, remoteProgmapFilePath, ftpStatus);
                }
            }
        }

        private async Task ProcessFtpListItemsAsync(ApplicationDbContext dbContext, string deviceId, FtpClient client, string localDirectoryName, CancellationToken stoppingToken, params FtpListItem[] ftpListItems)
        {
            foreach (var item in ftpListItems)
            {
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    //原始的 Modified 读不到秒
                    item.Modified = await client.GetModifiedTimeAsync(item.FullName, stoppingToken);
                }
                var fileMeta = await dbContext.Files.FirstOrDefaultAsync(p => p.DeviceId == deviceId && p.RemoteFullPath == item.FullName && p.Size == item.Size && p.Modified == item.Modified, stoppingToken);

                switch (item.Type)
                {
                    case FtpFileSystemObjectType.File:
                        if (fileMeta == null)
                        {
                            var saveFilename = Guid.NewGuid().ToString("N");//N	32位数：00000000000000000000000000000000
                            var saveFilePath = Path.Combine(localDirectoryName, saveFilename);
                            fileMeta = new iGC.Models.File
                            {
                                DeviceId = deviceId,
                                ObjectType = FileSystemObjectType.File,
                                RemoteFileName = item.Name,
                                RemoteFullPath = item.FullName,
                                LocalDirectoryName = localDirectoryName,
                                LocalFilename = saveFilename,
                                LocalFullPath = saveFilePath,
                                Size = item.Size,
                                Created = item.Created,
                                Modified = item.Modified,
                                DownloadAt = DateTime.Now,
                            };

                            var ftpStatus = await client.DownloadFileAsync(saveFilePath, item.FullName, FtpLocalExists.Overwrite, FtpVerify.Retry, token: stoppingToken);
                            if (FtpStatus.Success == ftpStatus)
                            {
                                await dbContext.Files.AddAsync(fileMeta, stoppingToken);

                                System.IO.File.SetCreationTime(saveFilePath, item.Modified);
                                System.IO.File.SetLastWriteTime(saveFilePath, item.Modified);

                                FileAttributes fileAttributes = System.IO.File.GetAttributes(saveFilePath);
                                if (!fileAttributes.HasFlag(FileAttributes.ReadOnly))
                                {
                                    System.IO.File.SetAttributes(saveFilePath, fileAttributes | FileAttributes.ReadOnly);
                                }
                            }
                            else
                            {
                                Logger.LogError("下载[{remotePath}]到本地[{localPath}]失败:[{ftpStatus}]！", item.FullName, saveFilePath, ftpStatus);
                            }
                        }
                        else if (!System.IO.File.Exists(fileMeta.LocalFullPath))
                        {
                            dbContext.Files.Remove(fileMeta);
                            Logger.LogWarning("远程文件[{remotePath}]对应的本地文件[{localPath}]不存在，删除此项记录！", fileMeta.RemoteFullPath, fileMeta.LocalFullPath);
                        }
                        break;
                    case FtpFileSystemObjectType.Directory:
                        if (fileMeta == null)
                        {
                            fileMeta = new iGC.Models.File
                            {
                                DeviceId = deviceId,
                                ObjectType = FileSystemObjectType.Directory,
                                RemoteFullPath = item.FullName,
                                Size = item.Size,
                                Created = item.Created,
                                Modified = item.Modified
                            };
                            await dbContext.Files.AddAsync(fileMeta, stoppingToken);
                        }

                        var subFtpListItems = await client.GetListingAsync(item.FullName, stoppingToken);
                        await ProcessFtpListItemsAsync(dbContext, deviceId, client, localDirectoryName, stoppingToken, subFtpListItems);
                        break;
                    case FtpFileSystemObjectType.Link:
                        if (fileMeta == null)
                        {
                            fileMeta = new iGC.Models.File
                            {
                                DeviceId = deviceId,
                                ObjectType = FileSystemObjectType.Link,
                                RemoteFullPath = item.FullName,
                                Size = item.Size,
                                Created = item.Created,
                                Modified = item.Modified
                            };
                            await dbContext.Files.AddAsync(fileMeta, stoppingToken);
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        private async Task<bool> UpdateProgMapAsync(ApplicationDbContext dbContext, List<CNC> cncs, string saveFilePath, CancellationToken stoppingToken)
        {
            try
            {
                XmlDocument xmlDocument = new();
                XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", "");
                xmlDocument.AppendChild(xmlDeclaration);

                XmlElement iGSProglist = xmlDocument.CreateElement("iGSProglist");
                iGSProglist.SetAttribute("version", "1.0");
                xmlDocument.AppendChild(iGSProglist);

                foreach (var cnc in cncs)
                {
                    //所有文件，不包括历史版本
                    var files = await dbContext.Files.Where(p => p.DeviceId == cnc.DeviceId)
                                                     .OrderByDescending(p => (int)p.ObjectType)
                                                     .ThenBy(p => p.RemoteFullPath)
                                                     .Select(p => p.RemoteFullPath).Distinct().Select(remoteFullPath =>
                             dbContext.Files.Where(p => p.DeviceId == cnc.DeviceId && p.RemoteFullPath == remoteFullPath)
                             .OrderByDescending(p => p.Modified).First())
                           .AsNoTracking()
                           .ToListAsync(stoppingToken);

                    //所有文件，包括历史版本
                    //var files = await dbContext.Files.Where(p => p.DeviceId == cnc.DeviceId)
                    //                                 .OrderByDescending(p => (int)p.ObjectType)
                    //                                 .ThenBy(p => p.RemoteFullPath)
                    //                                 .ThenByDescending(p => p.Modified)
                    //                                 .AsNoTracking()
                    //                                 .ToListAsync(stoppingToken);

                    XmlElement mac = xmlDocument.CreateElement("Mac");
                    mac.SetAttribute("name", cnc.Name);
                    iGSProglist.AppendChild(mac);

                    foreach (iGC.Models.File file in files)
                    {
                        var filename = Path.GetFileName(file.RemoteFullPath);
                        var filePath = file.RemoteFullPath[..(file.RemoteFullPath.Length - filename.Length - 1)];
                        var lastModified = new DateTimeOffset(DateTime.SpecifyKind(file.Modified, DateTimeKind.Utc)).ToUnixTimeSeconds();

                        XmlElement menu = xmlDocument.CreateElement("menu");
                        menu.SetAttribute("filesystemobjecttype", $"{(int)file.ObjectType}");
                        menu.SetAttribute("id", $"{file.Id}");
                        menu.SetAttribute("size", $"{file.Size}");
                        menu.SetAttribute("filename", filename);
                        menu.SetAttribute("filepath", filePath);
                        menu.SetAttribute("lastModified", $"{lastModified}");
                        menu.InnerText = string.Empty;

                        mac.AppendChild(menu);
                    }
                }

                xmlDocument.Save(saveFilePath);

                return System.IO.File.Exists(saveFilePath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "创建失败！|{progmap}", saveFilePath);
                //throw;
                return false;
            }
        }


    }
}
