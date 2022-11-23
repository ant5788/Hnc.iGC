
namespace Hnc.iGC.Web
{
 
    public class FileDtos
    {
        /// <summary>
        /// 文件类型
        /// image/file/video
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// 文件名称，包含扩展名
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// base64String
        /// </summary>
        public string Base64String { get; set; }

        //业务名称
        public string? BusinessName { get; set; }
    }
}
