using Fleck;
using Quartz;

namespace Hnc.iGC.Web
{
    public class WorkJob : IJob
    {

        public CalibrationPlanDAL calibrationPlanDAL = new CalibrationPlanDAL();
        public DeviceMaintainDAL deviceMaintainDAL = new DeviceMaintainDAL();

        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            List<IWebSocketConnection> allSockets = (List<IWebSocketConnection>)dataMap.Get("allSockets");
            //实验室 设备校准计划提醒
            List<CalibrationPlan> startPlan = calibrationPlanDAL.GetList(" plan_state = 0 and start_time between now() and (NOW() + interval 24 hour)");
            List<CalibrationPlan> endPlan = calibrationPlanDAL.GetList(" plan_state = 1 and end_time between now() and (NOW() + interval 24 hour)");
            //await Console.Out.WriteLineAsync($"{DateTime.Now}:Hello!");
            if (startPlan.Count > 0 || endPlan.Count > 0)
            {
                string message = "设备校准计划将在未来24小时开始请做好准备";
                string nameMessage = "";
                foreach (CalibrationPlan calibrationPlan in startPlan)
                {
                    nameMessage += calibrationPlan.PlanName + " ";
                }

                string messageEnd = "设备校准计划将在未来24小时结束请尽快执行操作";
                string endMessage = "";

                foreach (CalibrationPlan calibrationPlan in endPlan)
                {
                    endMessage += calibrationPlan.PlanName + " ";
                }


                foreach (var socket in allSockets.ToList())
                {
                    if (endMessage.Length > 0)
                    {
                        await socket.Send(nameMessage + message + "<BR/>" + endMessage + messageEnd);
                    }
                    else
                    {
                        await socket.Send(nameMessage + message);
                    }

                }
            }

            //设备保养提醒
            string msg = "";
            List<DeviceMaintain> maintainList = deviceMaintainDAL.GetList("");
            if (maintainList.Count > 0)
            {
                for (int i = 0; i < maintainList.Count; i++)
                {
                    DateTime dateTime = DateTime.Now;
                    DateTime plannedTime = maintainList[i].PlannedTime;
                    int val = DateDiff(dateTime, plannedTime);
                    /*Console.WriteLine("val======="+val);
                    Console.WriteLine("maintainList[i].EarlyWarningTime=======" + maintainList[i].EarlyWarningTime);*/

                    if (val <= maintainList[i].EarlyWarningTime && val > 0)
                    {
                        msg += maintainList[i].DeviceName + "将在未来" + val + "天内开始保养请做好准备";
                    }
                    if (i <= maintainList.Count)
                    {
                        msg += "</br>";
                    }
                }
            }
            foreach (var socket in allSockets.ToList())
            {
                await socket.Send(msg);
            }


            //设备检点计划提醒
            /* List<CheckPoint> startPointList = checkPointDAL.GetList(" State = 0 and start_time between now() and (NOW() + interval 24 hour)");
             List<CheckPoint> endPointList = checkPointDAL.GetList(" State = 1 and end_time between now() and (NOW() + interval 24 hour)");
             string startMsg = "等设备将在24内开始进行检点请做好准备";
             string endMsg = "等设备将在24内结束检点请尽快完成";
             string startName = "";
             string endName = "";
             if (startPointList.Count > 0 || endPointList.Count > 0)
             {

                 foreach (CheckPoint point in startPointList)
                 {
                     startName += point.DeviceName + " ";
                 }
                 foreach (CheckPoint point in endPointList)
                 {
                     endName += point.DeviceName + " ";
                 }

                 foreach (var socket in allSockets.ToList())
                 {
                     if (endName.Length > 0)
                     {
                         await socket.Send(startName + startMsg + "<BR/>" + endName + endMsg);
                     }
                     else
                     {
                         await socket.Send(startName + startMsg);
                     }
                 }
             }*/




        }


        public int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());

            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());

            TimeSpan sp = end.Subtract(start);

            return sp.Days;
        }
    }
}