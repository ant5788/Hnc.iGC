using MySql.Data.MySqlClient;
using NPOI.SS.Util;
using System.Data;
using System.Text;

namespace Hnc.iGC.Web
{
    public class StatusTotalDAL
    {
        public StatusTotalDAL() { }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="statusTotal"></param>
        /// <returns></returns>
        public bool Add(StatusTotal statusTotal)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into status_total(");
            strSql.Append("id,device_id,device_name,device_status,device_status_name,start_time,create_time,current_program_number,current_program_name )");
            strSql.Append(" values (");
            strSql.Append("@id,@device_id,@device_name,@device_status,@device_status_name,@start_time,@create_time,@current_program_number,@current_program_name)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_status",MySqlDbType.Int32),
                new MySqlParameter("@device_status_name",MySqlDbType.String),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@current_program_number", MySqlDbType.Int32),
                new MySqlParameter("@current_program_name", MySqlDbType.String)};
            parameters[0].Value = statusTotal.Id;
            parameters[1].Value = statusTotal.DeviceId;
            parameters[2].Value = statusTotal.DeviceName;
            parameters[3].Value = statusTotal.DeviceStatus;
            parameters[4].Value = statusTotal.DeviceStatusName;
            parameters[5].Value = statusTotal.StartTime;
            parameters[6].Value = statusTotal.CreateTime;
            parameters[7].Value = statusTotal.CurrentProgramNumber;
            parameters[8].Value = statusTotal.CurrentProgramName;
            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="statusTotal"></param>
        /// <returns></returns>
        public bool Update(StatusTotal statusTotal)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update status_total set ");
            strSql.Append("id=@id,");
            strSql.Append("device_id=@device_id,");
            strSql.Append("device_status=@device_status,");
            strSql.Append("device_status_name=@device_status_name,");
            strSql.Append("start_time=@start_time,");
            strSql.Append("end_time=@end_time,");
            strSql.Append("duration=@duration,");
            strSql.Append("create_time=@create_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters =
            {
                 new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_status",MySqlDbType.Int32),
                new MySqlParameter("@device_status_name",MySqlDbType.String),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@end_time",MySqlDbType.DateTime),
                new MySqlParameter("@duration",MySqlDbType.Double),
                new MySqlParameter("@create_time",MySqlDbType.DateTime) };
            parameters[0].Value = statusTotal.DeviceId;
            parameters[1].Value = statusTotal.DeviceStatus;
            parameters[2].Value = statusTotal.DeviceStatusName;
            parameters[3].Value = statusTotal.StartTime;
            parameters[4].Value = statusTotal.EndTime;
            parameters[5].Value = statusTotal.Duration;
            parameters[6].Value = statusTotal.CreateTime;
            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devicId"></param>
        /// <param name="deviceStatus"></param>
        /// <returns></returns>
        public StatusTotal GetModelByParameters(string devicId, int deviceStatus)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from status_total");
            strSql.Append(" where device_id = ?");
            strSql.Append(" and device_status = ?");
            strSql.Append(" and end_time is null");
            strSql.Append(" and duration is null order by create_time DESC");
            MySqlParameter[] parameters =
            {
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_status",MySqlDbType.Int32)
            };
            parameters[0].Value = devicId;
            parameters[1].Value = deviceStatus;
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString(), parameters);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(dataSet.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devicId"></param>
        /// <param name="deviceStatus"></param>
        /// <returns></returns>
        public StatusTotal GetModelByParameters(string devicId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from status_total where device_id = '"+devicId+"' and end_time is null and duration is null order by create_time DESC");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(dataSet.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }
        public StatusTotal DataRowToModel(DataRow dataRow)
        {
            StatusTotal model = new StatusTotal();
            if (null != dataRow)
            {
                if (dataRow.Table.Columns.Contains("id"))
                {
                    if (null != dataRow["id"] && dataRow["id"].ToString() != "")
                    {
                        model.Id = dataRow["id"].ToString();
                    }
                }
                if (dataRow.Table.Columns.Contains("device_id"))
                {
                    if (dataRow["device_id"] != null && dataRow["device_id"].ToString() != "")
                    {
                        model.DeviceId = dataRow["device_id"].ToString();
                    }
                }
                if (dataRow.Table.Columns.Contains("device_name"))
                {
                    if (dataRow["device_name"] != null && dataRow["device_name"].ToString() != "")
                    {
                        model.DeviceName = dataRow["device_name"].ToString();
                    }
                }

                if (dataRow.Table.Columns.Contains("device_status"))
                {
                    if (dataRow["device_status"] != null && dataRow["device_status"].ToString() != "")
                    {
                        model.DeviceStatus = int.Parse(dataRow["device_status"].ToString());
                    }
                }


                if (dataRow.Table.Columns.Contains("start_time"))
                {
                    if (dataRow["start_time"] != null && dataRow["start_time"].ToString() != "")
                    {
                        model.StartTime = DateTime.Parse(dataRow["start_time"].ToString());
                    }
                }

                if (dataRow.Table.Columns.Contains("end_time"))
                {
                    if (dataRow["end_time"] != null && dataRow["end_time"].ToString() != "")
                    {
                        model.EndTime = DateTime.Parse(dataRow["end_time"].ToString());
                    }
                }

                if (dataRow.Table.Columns.Contains("duration"))
                {
                    if (dataRow["duration"] != null && dataRow["duration"].ToString() != "")
                    {
                        model.Duration = double.Parse(dataRow["duration"].ToString());
                    }
                }

                if (dataRow.Table.Columns.Contains("create_time"))
                {
                    if (dataRow["create_time"] != null && dataRow["create_time"].ToString() != "")
                    {
                        model.CreateTime = DateTime.Parse(dataRow["create_time"].ToString());
                    }
                }
                if (dataRow.Table.Columns.Contains("current_program_number"))
                {
                    if (dataRow["current_program_number"] != null && dataRow["current_program_number"].ToString() != "")
                    {

                        model.CurrentProgramNumber = Convert.ToInt32(dataRow["current_program_number"].ToString());
                    }
                }
                if (dataRow.Table.Columns.Contains("current_program_name"))
                {
                    if (dataRow["current_program_name"] != null && dataRow["current_program_name"].ToString() != "")
                    {

                        model.CurrentProgramName = dataRow["current_program_name"].ToString();
                    }
                }


            }
            return model;
        }

        /// <summary>
        /// 查詢列表
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<StatusTotal> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,device_id,device_status,start_time,end_time,duration,create_time from `status_total` ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DataRowToModelList(DbHelperMySQL.Query(strSql.ToString()));
        }



        public List<StatusTotal> DataRowToModelList(DataSet dataSet)
        {
            List<StatusTotal> statusTotalList = new List<StatusTotal>();
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    StatusTotal statusTotal = DataRowToModel(dataSet.Tables[0].Rows[i]);
                    statusTotalList.Add(statusTotal);
                }
                return statusTotalList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> FaultDuration(string deviceId, string time)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select a.order_hour as startTime, ifnull(b.duration,0) as duration ");
            strSql.Append("from (select 0 as order_hour union all select 1 as order_hour union all ");
            strSql.Append("select 2 as order_hour union all select 3 as order_hour union all ");
            strSql.Append("select 4 as order_hour union all select 5 as order_hour union all ");
            strSql.Append("select 6 as order_hour union all select 7 as order_hour union all ");
            strSql.Append("select 8 as order_hour union all select 9 as order_hour union all ");
            strSql.Append("select 10 as order_hour union all select 11 as order_hour union all ");
            strSql.Append("select 12 as order_hour union all select 13 as order_hour union all ");
            strSql.Append("select 14 as order_hour union all select 15 as order_hour union all ");
            strSql.Append("select 16 as order_hour union all select 17 as order_hour union all ");
            strSql.Append("select 18 as order_hour union all select 19 as order_hour union all ");
            strSql.Append("select 20 as order_hour union all select 21 as order_hour union all ");
            strSql.Append("select 22 as order_hour union all select 23 as order_hour union all ");
            strSql.Append("select 24 as order_hour) as a left join (");
            strSql.Append("select HOUR (end_time) AS saleHour, sum(duration) AS duration  FROM status_total  ");
            strSql.Append("where date_format( create_time, '%Y-%m-%d' ) = " + "'" + time + "'" + "and device_id = '" + deviceId + "' ");
            strSql.Append("and device_status = '99'");
            strSql.Append(" GROUP BY HOUR (create_time) ORDER BY HOUR (end_time) ) b on a.order_hour = b.saleHour order by order_hour");
            Console.WriteLine(strSql.ToString());
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<Dictionary<string, object>> mapList = DataSetToList(dataSet);
            return mapList;
        }

        private List<Dictionary<string, object>> DataSetToList(DataSet dataSet)
        {
            List<Dictionary<string, object>> mapList = new List<Dictionary<string, object>>();
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();

                    string MakeTime = dataSet.Tables[0].Rows[i]["startTime"].ToString();
                    string time = "";
                    if (MakeTime.Length == 1)
                    {
                        time = "0" + MakeTime + ":00";
                    }
                    else
                    {
                        time = MakeTime + ":00";
                    }
                    double duration = Convert.ToDouble(dataSet.Tables[0].Rows[i]["duration"].ToString());
                    map.Add("time", time);
                    map.Add("duration", Math.Round((duration * 60), 2));
                    mapList.Add(map);
                }
            }
            return mapList;
        }


        /// <summary>
        /// 统计所有设备全部状态用时
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<StatusTotal> EfficiencyAnalysis(string startTime, string endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT A.device_id, a.device_name,A.device_status, A.start_time, A.end_time, A.duration ");
            strSql.Append("FROM status_total A JOIN (SELECT device_id, device_status, start_time, end_time, duration FROM status_total GROUP BY device_id HAVING count( * ) > 1 ) AS temp ");
            strSql.Append("ON a.device_id = temp.device_id ");
            strSql.Append("AND a.start_time >= '" + startTime + "' AND a.end_time <= '" + endTime + "' ORDER BY device_id");
            Console.WriteLine(strSql.ToString());
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<StatusTotal> statusTotalList = DataRowToModelList(dataSet);
            return statusTotalList;
        }

        private static List<Dictionary<string, object>> EfficiencyDataSetToList(DataSet dataSet)
        {
            List<Dictionary<string, object>> mapList = new List<Dictionary<string, object>>();
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    string deviceId = dataSet.Tables[0].Rows[i]["device_id"].ToString();
                    string deviceStstus = dataSet.Tables[0].Rows[i]["device_status"].ToString();
                    string startTime = dataSet.Tables[0].Rows[i]["start_time"].ToString();
                    string endTime = dataSet.Tables[0].Rows[i]["end_time"].ToString();
                    string duration = dataSet.Tables[0].Rows[i]["duration"].ToString();
                    map.Add("deviceId", deviceId);
                    map.Add("deviceStstus", deviceStstus);
                    map.Add("startTime", startTime);
                    map.Add("endTime", endTime);
                    map.Add("duration", duration);
                    mapList.Add(map);
                }
            }
            return mapList;
        }

        /// <summary>
        /// 单台设备某段时间状态统计
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<StatusTotal> EfficiencyAnalysisById(string deviceId, string startTime, string endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM status_total where device_id = ");
            strSql.Append("'" + deviceId + "'and start_time >= '" + startTime + "'");
            strSql.Append(" and end_time <= '" + endTime + "'");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<StatusTotal> mapList = DataRowToModelList(dataSet);
            return mapList;
        }




        /// <summary>
        /// 时间段内每个状态时长统计。
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, object>> StatusTimeStatistics(string deviceId, string startTime, string endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT DATE_FORMAT(create_time, '%Y-%m-%d' ) time, sum(duration) duration,device_status FROM status_total ");
            strSql.Append("WHERE create_time BETWEEN '" + startTime + "' AND '" + endTime + "' and device_id = '" + deviceId + "'");
            strSql.Append(" and device_status = '3' GROUP BY time  ORDER BY time ASC");
            Console.WriteLine(strSql.ToString());
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());

            List<Dictionary<string, object>> mapList = new List<Dictionary<string, object>>();

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    string Time = dataSet.Tables[0].Rows[i]["time"].ToString();
                    double Duration = double.Parse(dataSet.Tables[0].Rows[i]["duration"].ToString());
                    double DeviceStatus = double.Parse(dataSet.Tables[0].Rows[i]["device_status"].ToString());
                    map.Add("Time", Time);
                    map.Add("duration", Duration);
                    map.Add("DeviceStatus", DeviceStatus);
                    mapList.Add(map);
                }
                return mapList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取程序名称
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public string getCurrentName(string deviceId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT distinct current_program_name FROM status_total WHERE device_id =  '" + deviceId + "' ORDER BY create_time desc");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<string> nameList = new List<string>();
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    string name = dataSet.Tables[0].Rows[i]["current_program_name"].ToString();
                    nameList.Add(name);
                }
            }
            return nameList[0];
        }


        /// <summary>
        /// 查询程序总时长
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public List<StatusTotal> TotalProgramDuration(string deviceId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT sum(duration) duration, current_program_number, current_program_name FROM status_total ");
            strSql.Append("WHERE device_id = '" + deviceId + "' ");
            strSql.Append("GROUP BY current_program_number ORDER BY current_program_number ASC");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<StatusTotal> mapList = DataRowToModelList(dataSet);
            return mapList;
        }

        /// <summary>
        /// 查询某个程序循环启动的时长
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public List<StatusTotal> CycleStartDurtion(string deviceId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT sum(duration) duration, current_program_number, current_program_name FROM status_total ");
            strSql.Append("WHERE device_id = '" + deviceId + "' and device_status = '3' ");
            strSql.Append("GROUP BY current_program_number ORDER BY current_program_number ASC");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<StatusTotal> mapList = DataRowToModelList(dataSet);
            return mapList;
        }

        /// <summary>
        /// 报警时长TOP5
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetTop5() 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select sum(duration) duration,device_name from status_total where device_status = '99'");
            strSql.Append(" GROUP BY device_id ORDER BY duration desc");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
            if (dataSet.Tables[0].Rows.Count > 0) 
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    string duration = dataSet.Tables[0].Rows[i]["duration"].ToString();
                    string deviceName = dataSet.Tables[0].Rows[i]["device_name"].ToString();
                    map.Add("duration", duration);
                    map.Add("deviceName", deviceName);
                    listMap.Add(map);
                }
            }
            return listMap;
        }


        /// <summary>
        /// 组装model
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public StatusTotal SetStatusTotal(CNCDto dto)
        {
            StatusTotal statusTotal = new StatusTotal();
            statusTotal.Id = GetRandomString();
            statusTotal.DeviceId = dto.DeviceId;
            statusTotal.DeviceStatus = dto.State;
            string dateTime = DateTime.Now.ToString();
            string createddate = Convert.ToDateTime(dateTime).ToString("yyyy-MM-dd HH:mm:dd");
            //DateTime dt = DateTime.Parse(createddate);
            statusTotal.StartTime = DateTime.Parse(createddate);
            statusTotal.CreateTime = DateTime.Parse(createddate);
            //当前程序号
            statusTotal.CurrentProgramNumber = dto.CurrentProgramNumber;
            //当前程序名称
            statusTotal.CurrentProgramName = dto.CurrentProgramName;
            statusTotal.Duration = 0.00;
            statusTotal.DeviceName = dto.Name;
            statusTotal.DeviceStatusName = dto.RunState;
            return statusTotal;
        }


        /// <summary>
        /// 生成11未随机数
        /// </summary>
        /// <returns></returns>
        public static string GetRandomString()
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = "";
            if (true) { str += "0123456789"; }
            if (true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            for (int i = 0; i < 32; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }




    }
}
