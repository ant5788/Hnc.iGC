using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class AlarmListDAL
    {
        public AlarmListDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="alarmList"></param>
        /// <returns></returns>
        public bool Add(AlarmList alarmList)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into alarm_list(").Append("id,device_name,device_Id,alarm_number,alarm_message,start_At,end_At,create_time)");
            strSql.Append(" values (@id,@device_name,@device_Id,@alarm_number,@alarm_message,@start_At,@end_At,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_Id",MySqlDbType.String),
                new MySqlParameter("@alarm_number",MySqlDbType.String),
                new MySqlParameter("@alarm_message",MySqlDbType.Int32),
                new MySqlParameter("@start_At",MySqlDbType.Int32),
                new MySqlParameter("@end_At",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = alarmList.Id;
            parameters[1].Value = alarmList.DeviceName;
            parameters[2].Value = alarmList.DeviceId;
            parameters[3].Value = alarmList.AlarmNumber;
            parameters[4].Value = alarmList.AlarmMessage;
            parameters[5].Value = alarmList.StartAt;
            parameters[6].Value = alarmList.EndAt;
            parameters[7].Value = alarmList.CreateTime;
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
        /// 查询列表
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<AlarmList> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from alarm_list ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DataRowToModelList(DbHelperMySQL.Query(strSql.ToString()));
        }



        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AlarmList GetById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM alarm_list where id = '" + id + "'");
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

        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) count FROM alarm_list ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            int count = 0;
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                count = int.Parse(dataSet.Tables[0].Rows[0]["count"].ToString());
            }
            return count;
        }



        /// <summary>
        /// 数据转为对象 toModel
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public AlarmList DataRowToModel(DataRow dataRow)
        {
            AlarmList model = new AlarmList();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = dataRow["ID"].ToString();
                }
                if (dataRow["device_name"] != null && dataRow["device_name"].ToString() != "")
                {
                    model.DeviceName = dataRow["device_name"].ToString();
                }

                if (dataRow["device_Id"] != null && dataRow["device_Id"].ToString() != "")
                {
                    model.DeviceId = dataRow["device_Id"].ToString();
                }
                if (dataRow["alarm_number"] != null && dataRow["alarm_number"].ToString() != "")
                {
                    model.AlarmNumber = dataRow["alarm_number"].ToString();
                }
                if (dataRow["alarm_message"] != null && dataRow["alarm_message"].ToString() != "")
                {
                    model.AlarmMessage = dataRow["alarm_message"].ToString();
                }


                if (dataRow["start_At"] != null && dataRow["start_At"].ToString() != "")
                {
                    model.StartAt = (DateTime)dataRow["start_At"];
                }

                if (dataRow["end_At"] != null && dataRow["end_At"].ToString() != "")
                {
                    model.EndAt = (DateTime)dataRow["end_At"];
                }

                if (dataRow["update_time"] != null && dataRow["update_time"].ToString() != "")
                {
                    model.CreateTime = (DateTime)dataRow["create_time"];
                }

            }
            return model;
        }

        /// <summary>
        /// 转list对象数据
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public List<AlarmList> DataRowToModelList(DataSet dataSet)
        {
            List<AlarmList> alarmList = new List<AlarmList>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                AlarmList alarm = DataRowToModel(dataSet.Tables[0].Rows[i]);
                alarmList.Add(alarm);
            }
            return alarmList;
        }



        /// <summary>
        /// 设置modl
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public AlarmList SetAlarmList(CNCDto dto)
        {
            AlarmList alarm = new AlarmList();
            alarm.Id = GetRandomString();
            alarm.DeviceName = dto.Name;
            alarm.DeviceId = dto.DeviceId;
            alarm.CreateTime = DateTime.Now;
            return alarm;
        }


        /// <summary>
        /// 生成32未随机数
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

        #endregion  BasicMethod
    }
}
