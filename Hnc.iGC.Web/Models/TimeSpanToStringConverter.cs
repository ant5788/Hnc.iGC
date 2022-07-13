using AutoMapper;

namespace Hnc.iGC.Web.Models
{
    public class TimeSpanToStringConverter : ITypeConverter<TimeSpan?, string?>
    {
        public string? Convert(TimeSpan? source, string? destination, ResolutionContext context)
        {
            return source?.ToString();
        }
    }
}
