using MySql.Data.MySqlClient;
using NPOI.SS.Util;
using System.Data;
using System.Text;

namespace Hnc.iGC.Web
{
    public class TemperBoxDtoHisDAL
    {
        public TemperBoxDtoHisDAL() { }

        //TODO  需要修改属性 
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="temperBox"></param>
        /// <returns></returns>
        public bool Add(TemperBoxDtoHis temperBox)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into temper_box_his(");
            strSql.Append("id,device_Id,runState,state,PV_TMP,PV_HUM,SV_TMP,SV_HUM,alarmstate,alarmData,create_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_Id,@runState,@state,@PV_TMP,@PV_HUM,@SV_TMP,@SV_HUM,@alarmstate,@alarmData,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_Id",MySqlDbType.String),
                new MySqlParameter("@runState",MySqlDbType.String),
                new MySqlParameter("@state",MySqlDbType.Int32),
                new MySqlParameter("@PV_TMP",MySqlDbType.Double),
                new MySqlParameter("@PV_HUM",MySqlDbType.Double),
                new MySqlParameter("@SV_TMP",MySqlDbType.Double),
                new MySqlParameter("@SV_HUM", MySqlDbType.Double),
                new MySqlParameter("@alarmstate", MySqlDbType.Bit),
                new MySqlParameter("@alarmData", MySqlDbType.String),
                new MySqlParameter("@create_time", MySqlDbType.DateTime)
            };
            parameters[0].Value = temperBox.Id;
            parameters[1].Value = temperBox.DeviceId;
            parameters[2].Value = temperBox.RunState;
            parameters[3].Value = temperBox.State;
            parameters[4].Value = temperBox.PV_TMP;
            parameters[5].Value = temperBox.PV_HUM;
            parameters[6].Value = temperBox.SV_TMP;
            parameters[7].Value = temperBox.SV_HUM;
            parameters[8].Value = temperBox.Alarmstate;
            parameters[9].Value = temperBox.AlarmData;
            parameters[10].Value = temperBox.CreateTime;
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
        /// 删除24个月之前的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean DeleteBy24MonthsAgo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("DELETE FROM temper_box_his WHERE DATE(create_time) <= DATE(DATE_SUB(NOW(),INTERVAL 24 MONTH))");
            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString());
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
        /// 按照时间范围和设备ID查询历史曲线数据
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<TemperBoxDtoHis> HistoricalCurve(string deviceId, string startTime, string endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from temper_box_his where device_Id = '" + deviceId);
            strSql.Append("' and create_time BETWEEN '"+startTime);
            strSql.Append("' AND '"+endTime+ "' ORDER BY create_time asc");
            Console.WriteLine(strSql.ToString());
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            return DataRowToModelList(dataSet);
        }




        public TemperBoxDtoHis DataRowToModel(DataRow dataRow)
        {
            TemperBoxDtoHis model = new TemperBoxDtoHis();
            if (null != dataRow)
            {
                if (dataRow.Table.Columns.Contains("Id"))
                {
                    if (null != dataRow["Id"] && dataRow["Id"].ToString() != "")
                    {
                        model.Id = dataRow["Id"].ToString();
                    }
                }
                if (dataRow.Table.Columns.Contains("device_id"))
                {
                    if (dataRow["device_id"] != null && dataRow["device_id"].ToString() != "")
                    {
                        model.DeviceId = dataRow["device_id"].ToString();
                    }
                }
                if (dataRow.Table.Columns.Contains("runState"))
                {
                    if (dataRow["runState"] != null && dataRow["runState"].ToString() != "")
                    {
                        model.RunState = dataRow["runState"].ToString();
                    }
                }

                if (dataRow.Table.Columns.Contains("state"))
                {
                    if (dataRow["state"] != null && dataRow["state"].ToString() != "")
                    {
                        model.State = int.Parse(dataRow["state"].ToString());
                    }
                }
                if (dataRow.Table.Columns.Contains("PV_TMP"))
                {
                    if (dataRow["PV_TMP"] != null && dataRow["PV_TMP"].ToString() != "")
                    {
                        model.PV_TMP = float.Parse(dataRow["PV_TMP"].ToString());
                    }
                }

                if (dataRow.Table.Columns.Contains("PV_HUM"))
                {
                    if (dataRow["PV_HUM"] != null && dataRow["PV_HUM"].ToString() != "")
                    {
                        model.PV_HUM = float.Parse(dataRow["PV_HUM"].ToString());
                    }
                }

                if (dataRow.Table.Columns.Contains("SV_TMP"))
                {
                    if (dataRow["SV_TMP"] != null && dataRow["SV_TMP"].ToString() != "")
                    {
                        model.SV_TMP = float.Parse(dataRow["SV_TMP"].ToString());
                    }
                }

                if (dataRow.Table.Columns.Contains("SV_HUM"))
                {
                    if (dataRow["SV_HUM"] != null && dataRow["SV_HUM"].ToString() != "")
                    {
                        model.SV_HUM = float.Parse(dataRow["SV_HUM"].ToString());
                    }
                }

                if (dataRow.Table.Columns.Contains("alarmstate"))
                {
                    if (dataRow["alarmstate"] != null && dataRow["alarmstate"].ToString() != "")
                    {
                        model.Alarmstate = int.Parse(dataRow["alarmstate"].ToString());
                    }
                }

                if (dataRow.Table.Columns.Contains("alarmData"))
                {
                    if (dataRow["alarmData"] != null && dataRow["alarmData"].ToString() != "")
                    {
                        model.AlarmData = dataRow["alarmData"].ToString();
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



        public List<TemperBoxDtoHis> DataRowToModelList(DataSet dataSet)
        {
            List<TemperBoxDtoHis> temperBoxDtoHisList = new List<TemperBoxDtoHis>();
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    TemperBoxDtoHis temperBoxDtoHis = DataRowToModel(dataSet.Tables[0].Rows[i]);
                    temperBoxDtoHisList.Add(temperBoxDtoHis);
                }
                return temperBoxDtoHisList;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 组装model
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TemperBoxDtoHis SetTemperBoxHis(TemperBoxDto dto)
        {
            TemperBoxDtoHis his = new TemperBoxDtoHis();
            his.Id = GetRandomString();
            his.RunState = dto.RunState;
            his.State = dto.State;
            his.PV_TMP = dto.PV_TMP;
            his.PV_HUM = dto.PV_HUM;
            his.SV_TMP = dto.SV_TMP;
            his.SV_HUM = dto.SV_HUM;
            his.Alarmstate = dto.Alarmstate?0:1;
            his.AlarmData = dto.AlarmData;
            his.CreateTime = DateTime.Now;
            return his;
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




    }
}
