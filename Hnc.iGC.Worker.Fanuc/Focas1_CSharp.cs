using System;

using static Focas1;

namespace Hnc.iGC.Worker
{
    /// <summary>
    /// FOCAS1 应用于 0i 和 16i/18i/21i 系列 CNC
    /// </summary>
    public class Focas1_CSharp : Collector<CNCDto>
    {
        private ushort FlibHndl;

        public override string Protocal { get; } = "Focas1";

        public override bool Connect(string ip, ushort port, string type)
        {
            isConnected = EW_OK == cnc_allclibhndl3(ip, port, 10, out FlibHndl);
            return IsConnected;
        }

        public override bool Disconnect()
        {
            Console.WriteLine("cnc_freelibhndl(FlibHndl)================" + cnc_freelibhndl(FlibHndl));
            if (EW_OK == cnc_freelibhndl(FlibHndl))
            {
                isConnected = false;
            }
            isConnected = false;
            return !IsConnected;
        }
      
        /// <summary>
        /// 从API中读取数据。
        /// API会报错：EW_HANDLE = (-8) , /* Windows library handle error */
        /// </summary>
        /// <param name="model"></param>
        public override void SetDataTo(CNCDto model)
        {
            Console.WriteLine("FlibHndl======================"+ FlibHndl);

            int curr = -1;

            short number1 = Convert.ToInt16("4120");

            Focas1.ODBM odbm1 = new Focas1.ODBM();

            if (EW_OK == cnc_rdmacro(FlibHndl, number1, 10, odbm1))
            {
                int nowCurr = Convert.ToInt16(Math.Pow(10, -odbm1.dec_val) * odbm1.mcr_val);
                //此方式可以
                Console.WriteLine("odbm1========" + Math.Pow(10, -odbm1.dec_val) * odbm1.mcr_val);
                if (curr == -1)
                {
                    //todo 给model当前 nowCurr
                    //model.cuu
                    model.CurrentCutterNumber = nowCurr;
                    //将当前刀号设置全局变量
                    curr = nowCurr;
                }

                if (curr != nowCurr)
                {
                    short number2 = Convert.ToInt16("4113");
                    Focas1.ODBM odbm2 = new Focas1.ODBM();
                    if (EW_OK == cnc_rdmacro(FlibHndl, number2, 10, odbm2))
                    {
                        //判断4113 为6 标识换刀 在将新刀号赋予model
                        if (Math.Pow(10, -odbm2.dec_val) * odbm2.mcr_val == 6)
                        {
                            Console.WriteLine("odbm2========" + Math.Pow(10, -odbm2.dec_val) * odbm2.mcr_val);
                            Console.WriteLine("换刀成功");
                            model.CurrentCutterNumber = nowCurr;
                            curr = nowCurr;
                        }
                    }
                }
            }



            //已在0i-F验证
            IODBTIMER iodbtimer = new() { type = 0 };
            if (EW_OK == cnc_gettimer(FlibHndl, iodbtimer))
            {
                model.SystemTime = new DateTime(iodbtimer.date.year, iodbtimer.date.month, iodbtimer.date.date);
            }
            iodbtimer = new() { type = 1 };
            if (EW_OK == cnc_gettimer(FlibHndl, iodbtimer))
            {
                model.SystemTime = model.SystemTime?.Add(new TimeSpan(iodbtimer.time.hour, iodbtimer.time.minute, iodbtimer.time.second));
            }

            //已在0i-F验证
            var odbst = new ODBST();
            if (EW_OK == cnc_statinfo(FlibHndl, odbst))
            {
                model.RunState = Enum.GetName((RunStates)odbst.run);
                model.State = odbst.run;
                model.Alarm = odbst.alarm == 1;
                model.Emergency = odbst.emergency == 1;
                model.WorkMode = Enum.GetName((WorkModes)odbst.aut);
            }

            var iodbpsd_1 = new IODBPSD_1();

            //已在0i-F验证
            if (EW_OK == cnc_rdparam(FlibHndl, 6711, -1, 8, iodbpsd_1))
            {
                model.PartsCount = iodbpsd_1.ldata;
            }

            //已在0i-F验证
            if (EW_OK == cnc_rdparam(FlibHndl, 6712, -1, 8, iodbpsd_1))
            {
                model.PartsTotal = iodbpsd_1.ldata;
            }

            //已在0i-F验证
            if (EW_OK == cnc_rdparam(FlibHndl, 6713, -1, 8, iodbpsd_1))
            {
                model.PartsRequired = iodbpsd_1.ldata;
            }

            //已在0i-F验证
            if (EW_OK == cnc_rdparam(FlibHndl, 6750, 0, 8, iodbpsd_1))
            {
                int var = iodbpsd_1.ldata / 2;
                model.TimePowerOn = TimeSpan.FromMinutes(var).ToString();
            }

            //已在0i-F验证
            var timeOperating = TimeSpan.Zero;
            if (EW_OK == cnc_rdparam(FlibHndl, 6751, 0, 8, iodbpsd_1))
            {
                timeOperating = timeOperating.Add(TimeSpan.FromMilliseconds((iodbpsd_1.ldata/2)));
                model.TimeOperating = timeOperating.ToString();
            }
            if (EW_OK == cnc_rdparam(FlibHndl, 6752, 0, 8, iodbpsd_1))
            {
                //TODO 时间类型的改成时分秒形式
                timeOperating = timeOperating.Add(TimeSpan.FromMinutes((iodbpsd_1.ldata / 2)));
                timeOperating = timeOperating.Add(TimeSpan.FromMinutes((iodbpsd_1.ldata / 2)));
                model.TimeOperating = timeOperating.ToString();
            }

            //已在0i-F验证
            var timeCutting = TimeSpan.Zero;
            if (EW_OK == cnc_rdparam(FlibHndl, 6753, 0, 8, iodbpsd_1))
            {
                timeCutting = timeCutting.Add(TimeSpan.FromMilliseconds((iodbpsd_1.ldata / 2)));
                model.TimeCutting = timeCutting.ToString();
            }
            if (EW_OK == cnc_rdparam(FlibHndl, 6754, 0, 8, iodbpsd_1))
            {
                timeCutting = timeCutting.Add(TimeSpan.FromMinutes((iodbpsd_1.ldata / 2)));
                model.TimeCutting = timeCutting.ToString();
            }

            //已在0i-F验证
            var timeCycle = TimeSpan.Zero;
            if (EW_OK == cnc_rdparam(FlibHndl, 6757, 0, 8, iodbpsd_1))
            {
                timeCycle = timeCycle.Add(TimeSpan.FromMilliseconds((iodbpsd_1.ldata / 2)));
                model.TimeCycle = timeCycle.ToString();
            }
            if (EW_OK == cnc_rdparam(FlibHndl, 6758, 0, 8, iodbpsd_1))
            {
                timeCycle = timeCycle.Add(TimeSpan.FromMinutes((iodbpsd_1.ldata / 2)));
                model.TimeCycle = timeCycle.ToString();
            }


            //程序
            var odbexeprg = new ODBEXEPRG();
            //已在0i-F验证
            if (EW_OK == cnc_exeprgname(FlibHndl, odbexeprg))
            {
                model.CurrentProgramNumber = odbexeprg.o_num;
                model.CurrentProgramName = new string(odbexeprg.name).TrimEnd('\0');
            }
            if (EW_OK == cnc_rdblkcount(FlibHndl, out int lineNumber))
            {
                model.CurrentProgramLineNumber = lineNumber;
            }
            //var odbseq = new ODBSEQ();
            //if (EW_OK == cnc_rdseqnum(FlibHndl, odbseq))
            //{
            //    model.CurrentProgramLineNumber = odbseq.data;
            //}

            //正在运行的程序，报错
            ushort progLength = 1000;
            object progContent = 0;
            model.CurrentProgramContent =
    @"G18 G40 G71 G90
G54
LIMS=3500
G0 X250 Z250 D0
G0 X250 Z250 D0
M30";
            if (false &&
                EW_OK == cnc_rdexecprog(FlibHndl, ref progLength, out short blknum, progContent))
            {
                model.CurrentProgramLineNumber = blknum;
                //model.CurrentProgramContent = progContent?.ToString();
            }

            //报警
            short num = 10;
            var odbalmmsg2 = new ODBALMMSG2();
            //已在0i-F验证
            var r = cnc_rdalmmsg2(FlibHndl, -1, ref num, odbalmmsg2);
            if (EW_OK == r)
            {
                for (int i = 0; i < num; i++)
                {
                    if (typeof(ODBALMMSG2).GetField($"msg{i + 1}")?.GetValue(odbalmmsg2) is ODBALMMSG2_data p)
                    {
                        model.AlarmMessages.Add(new CNCDto.AlarmMessageDto
                        {
                            Number = p.alm_no.ToString(),
                            Message = p.alm_msg,
                            StartAt = p.alm_no == 0 ? null : DateTime.Now
                        });
                    }
                }
            }

            //进给、主轴
            var odbspeed = new ODBSPEED();
            //已在0i-F验证
            if (EW_OK == cnc_rdspeed(FlibHndl, -1, odbspeed))
            {
                model.FeedSpeed = odbspeed.actf.data * Math.Pow(10, -odbspeed.actf.dec);
                model.FeedSpeedUnit = odbspeed.actf.unit switch
                {
                    0 => "mm/min",
                    1 => "inch/min",
                    2 => "rpm",
                    3 => "mm/rev",
                    4 => "inch/rev",
                    _ => "",
                };
                model.SpindleSpeed = odbspeed.acts.data * Math.Pow(10, -odbspeed.acts.dec);
                model.SpindleSpeedUnit = odbspeed.acts.unit switch
                {
                    0 => "mm",
                    1 => "inch",
                    2 => "degree",
                    3 => "mm/rev",
                    4 => "inch/rev",
                    _ => "",
                };
            }
            short a = 6;
            var odbspn = new ODBSPLOAD();
            //已在0i-F验证
            if (EW_OK == cnc_rdspmeter(FlibHndl, 0, ref a, odbspn))
            {
                model.SpindleLoad = odbspn.spload1.spload.data;
            }

            var iodbpmc0 = new IODBPMC0();
            //已在0i-F验证
            if (EW_OK == pmc_rdpmcrng(FlibHndl, 0, 0, 12, 12, 9, iodbpmc0))
            {
                model.FeedOverride = 255 - iodbpmc0.cdata[0];
            }

            //已在0i-F验证
            if (EW_OK == pmc_rdpmcrng(FlibHndl, 0, 0, 30, 30, 9, iodbpmc0))
            {
                model.SpindleOverride = iodbpmc0.cdata[0];
            }

            //已在0i-F验证
            if (EW_OK == pmc_rdpmcrng(FlibHndl, 0, 0, 14, 14, 9, iodbpmc0))
            {
                model.RapidOverride = (iodbpmc0.cdata[0] & 0x03) switch
                {
                    0 => 100,
                    1 => 50,
                    2 => 25,
                    3 => 0,
                    _ => 0,
                };
            }

            //主轴负载
            num = MAX_AXIS;
            var spindleLoad = new ODBSPLOAD();
            //已在0i-F验证
            if (EW_OK == cnc_rdspmeter(FlibHndl, 0, ref num, spindleLoad))
            {
                //sp.spload1.spload.data
                for (int i = 0; i < num; i++)
                {
                    if (typeof(ODBSPLOAD).GetField($"spload{i + 1}")?.GetValue(spindleLoad) is ODBSPLOAD_data p)
                    {
                        model.Spindles.Add(new CNCDto.SpindleDto
                        {
                            Speed = p.spspeed.data,
                            Load = p.spload.data
                        });
                    }
                }
            }

            //伺服轴位置
            num = MAX_AXIS;
            var pos = new ODBPOS();
            //已在0i-F验证
            if (EW_OK == cnc_rdposition(FlibHndl, -1, ref num, pos))
            {
                for (int i = 0; i < num; i++)
                {
                    if (typeof(ODBPOS).GetField($"p{i + 1}")?.GetValue(pos) is POSELMALL p)
                    {
                        model.Axes.Add(new CNCDto.AxisDto
                        {
                            Name = p.abs.name.ToString(),
                            Absolute = p.abs.data * Math.Pow(10, -p.abs.dec),
                            Relative = p.rel.data * Math.Pow(10, -p.rel.dec),
                            Machine = p.mach.data * Math.Pow(10, -p.mach.dec),
                            Distance = p.dist.data * Math.Pow(10, -p.dist.dec)
                        });
                    }
                }
            }

            //伺服轴负载
            num = MAX_AXIS;
            var svload = new ODBSVLOAD();
            //已在0i-F验证
            if (EW_OK == cnc_rdsvmeter(FlibHndl, ref num, svload))
            {
                for (int i = 0; i < num; i++)
                {
                    if (typeof(ODBSVLOAD).GetField($"svload{i + 1}")?.GetValue(svload) is LOADELM p)
                    {
                        model.Axes[i].Load = p.data;
                    }
                }
            }

            //刀偏
            var odbtlinf = new ODBTLINF();
            //已在0i-F验证
            if (EW_OK == cnc_rdtofsinfo(FlibHndl, odbtlinf))
            {
                //有效工件偏置个数为：odbtlinf.use_no
                for (short no = 1; no <= Math.Min((short)10, odbtlinf.use_no); no++)
                {
                    model.CutterInfos.Add(new CNCDto.CutterInfoDto() { Number = no });
                    var odbtofs = new ODBTOFS();
                    //已在0i-F验证
                    if (EW_OK == cnc_rdtofs(FlibHndl, no, 0, 8, odbtofs))
                    {
                        model.CutterInfos[no - 1].RadiusWearOffset = odbtofs.data / 1000.0;
                    }
                    if (EW_OK == cnc_rdtofs(FlibHndl, no, 1, 8, odbtofs))
                    {
                        model.CutterInfos[no - 1].RadiusSharpOffset = odbtofs.data / 1000.0;
                    }
                    if (EW_OK == cnc_rdtofs(FlibHndl, no, 2, 8, odbtofs))
                    {
                        model.CutterInfos[no - 1].LengthWearOffset = odbtofs.data / 1000.0;
                    }
                    if (EW_OK == cnc_rdtofs(FlibHndl, no, 3, 8, odbtofs))
                    {
                        model.CutterInfos[no - 1].LengthSharpOffset = odbtofs.data / 1000.0;
                    }
                }
            }


        }
    }
}
