using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hnc.iGC.Web
{
    public class Images_Mes
    {
        public string Id { get; set; }
        /// <summary>
        /// 上传的文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件名后缀
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; } 
        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime AddTime { get; set; } 
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTimeOffset ModifyTime { get; set; } 
    
        /// <summary>
        /// 上传的的文件流
        /// </summary>
        public byte[] FileCon { get; set; } 
    }
}
