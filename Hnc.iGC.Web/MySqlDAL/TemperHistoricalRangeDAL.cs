using MySql.Data.MySqlClient;
using NPOI.SS.Util;
using System.Data;
using System.Text;

namespace Hnc.iGC.Web
{
    public class TemperHistoricalRangeDAL
    {
        public TemperHistoricalRangeDAL() { }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="historicalRange"></param>
        /// <returns></returns>
        public bool Add(TemperHistoricalRange historicalRange)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into historical_range(");
            strSql.Append("id,device_id,make,State,start_time,create_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_id,@make,@State,@start_time,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@make",MySqlDbType.String),
                new MySqlParameter("@State",MySqlDbType.Int32),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = historicalRange.Id;
            parameters[1].Value = historicalRange.DeviceId;
            parameters[2].Value = historicalRange.Make;
            parameters[3].Value = historicalRange.State;
            parameters[4].Value = historicalRange.StartTime;
            parameters[5].Value = historicalRange.CreateTime;
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
        /// <param name="historicalRange"></param>
        /// <returns></returns>
        public bool Update(TemperHistoricalRange historicalRange)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update historical_range set ");
            strSql.Append("id=@id,");
            strSql.Append("device_id=@device_id,");
            strSql.Append("make=@make,");
            strSql.Append("state=@state,");
            strSql.Append("end_time=@end_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters =
            {
                 new MySqlParameter("@id",MySqlDbType.String),
                 new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@make",MySqlDbType.String),
                new MySqlParameter("@state",MySqlDbType.Int32),
                new MySqlParameter("@end_time",MySqlDbType.DateTime),
            };
            parameters[0].Value = historicalRange.Id;
            parameters[1].Value = historicalRange.DeviceId;
            parameters[2].Value = historicalRange.Make;
            parameters[3].Value = historicalRange.State;
            parameters[4].Value = historicalRange.EndTime;
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
        /// 转实体对象
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public TemperHistoricalRange DataRowToModel(DataRow dataRow)
        {
            TemperHistoricalRange model = new TemperHistoricalRange();
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
                if (dataRow.Table.Columns.Contains("make"))
                {
                    if (dataRow["make"] != null && dataRow["make"].ToString() != "")
                    {
                        model.Make = dataRow["make"].ToString();
                    }
                }
                if (dataRow.Table.Columns.Contains("state"))
                {
                    if (dataRow["state"] != null && dataRow["state"].ToString() != "")
                    {
                        model.State = int.Parse(dataRow["State"].ToString());
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
        /// 转list
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public List<TemperHistoricalRange> DataRowToModelList(DataSet dataSet)
        {
            List<TemperHistoricalRange> historicalRangeList = new List<TemperHistoricalRange>();
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    TemperHistoricalRange historicalRange = DataRowToModel(dataSet.Tables[0].Rows[i]);
                    historicalRangeList.Add(historicalRange);
                }
                return historicalRangeList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取结束时间为空的数据
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public TemperHistoricalRange GetByEndTimeIsNull(string deviceId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from historical_range where end_time is null and device_id = '" + deviceId + "'");
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

        public List<TemperHistoricalRange> GetListByDate(string deviceId, DateTime time)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM historical_range where device_id = '" + deviceId);
            strSql.Append("' and create_time = '" + time + "' order by start_time desc");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            return DataRowToModelList(dataSet);

        }

        /// <summary>
        /// 查询表示列表
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public List<TemperHistoricalRange> GetMakeListByDeviceId(string deviceId) 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM historical_range where device_id = '" + deviceId+"' order by start_time asc");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            return DataRowToModelList(dataSet);
        }

        /// <summary>
        /// 通过deviceId和标识查询查询的时间范围
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="make"></param>
        /// <returns></returns>
        public TemperHistoricalRange GetQueryTime(string deviceId, string make) 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM historical_range WHERE device_id = '" + deviceId + "' AND make = '" + make + "'");
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
        /// 组装model
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TemperHistoricalRange SetTemperHistoricalRange(TemperBoxDto dto)
        {
            TemperHistoricalRange historicalRange = new TemperHistoricalRange();
            historicalRange.Id = GetRandomString();
            historicalRange.DeviceId = dto.DeviceId;
            //生成一个固定规律的标识
            //historicalRange.make = dto.make;
            historicalRange.CreateTime = DateTime.Now;

            historicalRange.StartTime = DateTime.Now;
            return historicalRange;
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
