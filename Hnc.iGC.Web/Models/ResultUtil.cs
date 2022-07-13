using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
namespace Hnc.iGC.Web
{
    public class ResultUtil
    {
        public static string Success(object obj,string message) 
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("state", 1);
            map.Add("message", message);
            map.Add("data", obj);
            return ToSuccess(map);

        }
        public static string Success(string message)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("state", 1);
            map.Add("message", message);
            return ToSuccess(map);

        }

        public static string Fail(string message) 
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("state", 0);
            map.Add("message", message);
            return ToSuccess(map);
        }


        public static string ToSuccess(Dictionary<string, object> map) 
        {
            var text = JsonSerializer.Serialize(map, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
            });
            return text;
        }

    }
}
