﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Hnc.iGC.Web
{

    public class ImageHelper
    {
        /// <summary> 
        /// 根据对应宽高生成缩略图
        /// </summary> 
        /// <param name="ResourceImage">图片流</param> 
        /// <param name="Width">缩略图的宽度</param> 
        /// <param name="Height">缩略图的高度</param> 
        /// <returns>缩略图的Image对象</returns> 
        public static byte[] GetReducedImage(byte[] ResourceImage, int Width, int Height)
        {
            try
            {
                using (var outputStream = new MemoryStream())
                {
                    //缩略图
                    using (Image<Rgba32> image = Image.Load(ResourceImage))
                    {
                        image.Mutate(x => x
                             .Resize(Width, Height)
                             );


                        image.SaveAsJpeg(outputStream);
                        return outputStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }

        }
    }
}
