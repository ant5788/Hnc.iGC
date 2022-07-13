using AutoMapper;

using Hnc.iGC.Models;

namespace Hnc.iGC.Web.Models
{
    public partial class MyProfile : Profile
    {
        public MyProfile()
        {
            CreateMap<TimeSpan?, string?>().ConvertUsing<TimeSpanToStringConverter>();
            CreateMap<string?, TimeSpan?>().ConvertUsing<StringToTimeSpanConverter>();

            CreateMap<CNC, CNCDto>();
            CreateMap<CNCDto, CNC>();

            CreateMap<CNC.AlarmMessage, CNCDto.AlarmMessageDto>();
            CreateMap<CNCDto.AlarmMessageDto, CNC.AlarmMessage>();

            CreateMap<CNC.Spindle, CNCDto.SpindleDto>();
            CreateMap<CNCDto.SpindleDto, CNC.Spindle>();

            CreateMap<CNC.Axis, CNCDto.AxisDto>();
            CreateMap<CNCDto.AxisDto, CNC.Axis>();

            CreateMap<CNC.CutterInfo, CNCDto.CutterInfoDto>();
            CreateMap<CNCDto.CutterInfoDto, CNC.CutterInfo>();

            CreateMap<Balancer, BalancerDto>();
            CreateMap<BalancerDto, Balancer>();
        }
    }
}
