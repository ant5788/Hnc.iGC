using System;

namespace Hnc.iGC.Models
{
    public enum FileSystemObjectType
    {
        File = 0,
        Directory = 1,
        Link = 2
    }

    public class File
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string DeviceId { get; set; }

        public byte[] Content { get; set; }

        public FileSystemObjectType ObjectType { get; set; }

        public string RemoteFileName { get; set; }

        public string RemoteFullPath { get; set; }

        public string LocalDirectoryName { get; set; }

        public string LocalFilename { get; set; }

        public string LocalFullPath { get; set; }

        public string Description { get; set; }

        public long Size { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime DownloadAt { get; set; }
    }
}
