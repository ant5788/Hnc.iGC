using MySql.Data.MySqlClient;
using NPOI.SS.Util;
using System.Data;
using System.Text;

namespace Hnc.iGC.Web
{
    public class TemperStateTotalDAL
    {
        public TemperStateTotalDAL() { }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="temperStateTotal"></param>
        /// <returns></returns>
        public bool Add(TemperStateTotal temperStateTotal)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into temper_state_total(");
            strSql.Append("id,device_id,device_name,runstate,state,start_time,create_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_id,@device_name,@runstate,@state,@start_time,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@runstate",MySqlDbType.String),
                new MySqlParameter("@state",MySqlDbType.Int32),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@create_time", MySqlDbType.DateTime)};
            parameters[0].Value = temperStateTotal.Id;
            parameters[1].Value = temperStateTotal.DeviceId;
            parameters[2].Value = temperStateTotal.DeviceName;
            parameters[3].Value = temperStateTotal.RunState;
            parameters[4].Value = temperStateTotal.State;
            parameters[5].Value = temperStateTotal.StartTime;
            parameters[6].Value = temperStateTotal.CreateTime;
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
        /// <param name="temperStateTotal"></param>
        /// <returns></returns>
        public bool Update(TemperStateTotal temperStateTotal)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update temper_state_total set ");
            strSql.Append("id=@id,");
            strSql.Append("device_id=@device_id,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("runstate=@runstate,");
            strSql.Append("state=@state,");
            strSql.Append("start_time=@start_time,");
            strSql.Append("end_time=@end_time,");
            strSql.Append("duration=@duration,");
            strSql.Append("create_time=@create_time,");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters =
            {
                 new MySqlParameter("@id",MySqlDbType.String),
                 new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@runstate",MySqlDbType.String),
                new MySqlParameter("@state",MySqlDbType.String),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@end_time",MySqlDbType.DateTime),
                new MySqlParameter("@duration",MySqlDbType.Double),
                new MySqlParameter("@update_time",MySqlDbType.DateTime),
                new MySqlParameter("@create_time",MySqlDbType.DateTime) };
            parameters[0].Value = temperStateTotal.Id;
            parameters[1].Value = temperStateTotal.DeviceId;
            parameters[2].Value = temperStateTotal.DeviceName;
            parameters[3].Value = temperStateTotal.RunState;
            parameters[4].Value = temperStateTotal.State;
            parameters[5].Value = temperStateTotal.StartTime;
            parameters[6].Value = temperStateTotal.EndTime;
            parameters[7].Value = temperStateTotal.Duration;
            parameters[8].Value = temperStateTotal.CreateTime;
            parameters[9].Value = temperStateTotal.UpdateTime;
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
        public TemperStateTotal GetModelByParameters(string devicId, int deviceStatus)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,device_id,device_name,runstate,state,start_time,end_time,duration,create_time from temper_state_total");
            strSql.Append(" where device_id = ?");
            strSql.Append(" and state = ?");
            strSql.Append(" and end_time is null");
            strSql.Append(" and duration is null");

            MySqlParameter[] parameters =
            {
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@state",MySqlDbType.Int32)
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
        public TemperStateTotal DataRowToModel(DataRow dataRow)
        {
            TemperStateTotal model = new TemperStateTotal();
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

                if (dataRow.Table.Columns.Contains("runstate"))
                {
                    if (dataRow["runstate"] != null && dataRow["runstate"].ToString() != "")
                    {
                        model.RunState = dataRow["runstate"].ToString();
                    }
                }  
                if (dataRow.Table.Columns.Contains("state"))
                {
                    if (dataRow["state"] != null && dataRow["state"].ToString() != "")
                    {
                        model.State = int.Parse(dataRow["state"].ToString());
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
            }
            return model;
        }

        /// <summary>
        /// 查詢列表
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<TemperStateTotal> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,device_id,device_status,start_time,end_time,duration,create_time from `temper_state_total` ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DataRowToModelList(DbHelperMySQL.Query(strSql.ToString()));
        }



        public List<TemperStateTotal> DataRowToModelList(DataSet dataSet)
        {
            List<TemperStateTotal> temperStateTotalList = new List<TemperStateTotal>();
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    TemperStateTotal temperStateTotal = DataRowToModel(dataSet.Tables[0].Rows[i]);
                    temperStateTotalList.Add(temperStateTotal);
                }
                return temperStateTotalList;
            }
            else
            {
                return null;
            }
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
        /// 组装model
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TemperStateTotal SetTemperStateTotal(TemperBoxDto dto)
        {
            TemperStateTotal temperStateTotal = new TemperStateTotal();
            temperStateTotal.Id = GetRandomString();
            temperStateTotal.DeviceId = dto.DeviceId;
            temperStateTotal.DeviceName = dto.Name;
            temperStateTotal.RunState = dto.RunState;
            temperStateTotal.State = dto.State;
            string dateTime = DateTime.Now.ToString();
            string createddate = Convert.ToDateTime(dateTime).ToString("yyyy-MM-dd HH:mm:dd");
            //DateTime dt = DateTime.Parse(createddate);
            temperStateTotal.StartTime = DateTime.Parse(createddate);
            temperStateTotal.CreateTime = DateTime.Parse(createddate);
            temperStateTotal.Duration = 0.00;
            return temperStateTotal;
        }


        /// <summary>
        /// 生成321未随机数
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
