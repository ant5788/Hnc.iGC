using AutoMapper;

namespace Hnc.iGC.Web.Models
{
    public class StringToTimeSpanConverter : ITypeConverter<string?, TimeSpan?>
    {
        public TimeSpan? Convert(string? source, TimeSpan? destination, ResolutionContext context)
        {
            return TimeSpan.TryParse(source, out var r) ? r : null;
        }
    }
}
