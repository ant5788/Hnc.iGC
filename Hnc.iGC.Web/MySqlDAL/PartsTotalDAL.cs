using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class PartsTotalDAL
    {
        public PartsTotalDAL() { }

        #region  BasicMethod

        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="partsTotal"></param>
        /// <returns></returns>
        public bool Add(PartsTotal partsTotal)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into parts_total(");
            strSql.Append("id,device_id,make_time,magnesium_total,aluminum_total,create_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_id,@make_time,@magnesium_total,@aluminum_total,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@make_time",MySqlDbType.DateTime),
                new MySqlParameter("@magnesium_total",MySqlDbType.Int32),
                new MySqlParameter("@aluminum_total",MySqlDbType.Int32),
                new MySqlParameter("@create_time",MySqlDbType.Datetime)};
            parameters[0].Value = partsTotal.Id;
            parameters[1].Value = partsTotal.DeviceId;
            parameters[2].Value = partsTotal.MakeTime;
            parameters[3].Value = partsTotal.MagnesiumTotal;
            parameters[4].Value = partsTotal.AluminumTotal;
            parameters[5].Value = partsTotal.CreateTime;
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
        /// <param name="partsTotal"></param>
        /// <returns></returns>
        public bool Update(PartsTotal partsTotal)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update parts_total set ");
            strSql.Append("device_id=@device_id,");
            strSql.Append("make_time=@make_time,");
            strSql.Append("magnesium_total=@magnesium_total,");
            strSql.Append("aluminum_total=@aluminum_total");
            strSql.Append(" where id = @id");
            MySqlParameter[] parameters =
            {
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@make_time",MySqlDbType.DateTime),
                new MySqlParameter("@magnesium_total",MySqlDbType.Int32),
                new MySqlParameter("@aluminum_total",MySqlDbType.Int32),
                new MySqlParameter("@id",MySqlDbType.String)
            };
            parameters[0].Value = partsTotal.DeviceId;
            parameters[1].Value = partsTotal.MakeTime;
            parameters[2].Value = partsTotal.MagnesiumTotal;
            parameters[3].Value = partsTotal.AluminumTotal;
            parameters[4].Value = partsTotal.Id;
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
        /// 通过设备ID和加工时间查询一条数据
        /// </summary>
        /// <param name="devicId"></param>
        /// <param name="hoursStart"></param>
        /// <param name="hoursEnd"></param>
        /// <returns></returns>
        public PartsTotal GetModelByParameters(string devicId, string hoursStart, string hoursEnd)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,device_id,make_time,magnesium_total,aluminum_total from parts_total ");
            strSql.Append("where make_time between " + "'" + hoursStart + "'");
            strSql.Append(" and '" + hoursEnd + "'");
            strSql.Append(" and device_id = ? ");
            MySqlParameter[] parameters =
            {
                new MySqlParameter("@device_id",MySqlDbType.String),
            };
            parameters[0].Value = devicId;
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

        public PartsTotal DataRowToModel(DataRow dataRow)
        {
            PartsTotal model = new PartsTotal();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = dataRow["id"].ToString();
                }
                //id,device_id,make_time,magnesium_total,aluminum_total
                if (dataRow["device_id"] != null && dataRow["device_id"].ToString() != "")
                {
                    model.DeviceId = dataRow["device_id"].ToString();
                }
                if (dataRow["make_time"] != null && dataRow["make_time"].ToString() != "")
                {
                    model.MakeTime = (DateTime)dataRow["make_time"];
                }
                if (dataRow["magnesium_total"] != null && dataRow["magnesium_total"].ToString() != "")
                {
                    model.MagnesiumTotal = int.Parse(dataRow["magnesium_total"].ToString());
                }
                if (dataRow["aluminum_total"] != null && dataRow["aluminum_total"].ToString() != "")
                {
                    model.AluminumTotal = int.Parse(dataRow["aluminum_total"].ToString());
                }
            }
            return model;
        }


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<PartsTotal> GetList(string strWhere)
        {
            List<PartsTotal> partsTotals = new List<PartsTotal>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,device_id,make_time,magnesium_total,aluminum_total from `parts_total` ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            DataSet dataaSet = DbHelperMySQL.Query(strSql.ToString());
            return DataRowToModelList(dataaSet);

        }






        /// <summary>
        /// 转换列表数据
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public List<PartsTotal> DataRowToModelList(DataSet dataSet)
        {
            List<PartsTotal> partsTotalList = new List<PartsTotal>();
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    PartsTotal partsTotal = DataRowToModel(dataSet.Tables[0].Rows[i]);
                    partsTotalList.Add(partsTotal);
                }
                return partsTotalList;
            }
            else
            {
                return null;
            }
        }

        public List<Dictionary<string, object>> PartsDailyOutput(string deviceId, string time)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT a.order_hour AS make_time,ifnull( b.magnesiumCount, 0 ) AS magnesium_total,IFNULL( b.aluminumCount, 0 ) AS aluminum_total ");
            strSql.Append("FROM (SELECT 0 AS order_hour UNION ALL SELECT 1 AS order_hour UNION ALL ");
            strSql.Append("SELECT 2 AS order_hour UNION ALL SELECT 3 AS order_hour UNION ALL ");
            strSql.Append("SELECT 4 AS order_hour UNION ALL SELECT 5 AS order_hour UNION ALL ");
            strSql.Append("SELECT 6 AS order_hour UNION ALL SELECT 7 AS order_hour UNION ALL ");
            strSql.Append("SELECT 8 AS order_hour UNION ALL SELECT 9 AS order_hour UNION ALL ");
            strSql.Append("SELECT 10 AS order_hour UNION ALL SELECT 11 AS order_hour UNION ALL ");
            strSql.Append("SELECT 12 AS order_hour UNION ALL SELECT 13 AS order_hour UNION ALL ");
            strSql.Append("SELECT 14 AS order_hour UNION ALL SELECT 15 AS order_hour UNION ALL ");
            strSql.Append("SELECT 16 AS order_hour UNION ALL SELECT 17 AS order_hour UNION ALL ");
            strSql.Append("SELECT 18 AS order_hour UNION ALL SELECT 19 AS order_hour UNION ALL ");
            strSql.Append("SELECT 20 AS order_hour UNION ALL SELECT 21 AS order_hour UNION ALL ");
            strSql.Append("SELECT 22 AS order_hour UNION ALL SELECT 23 AS order_hour ) AS a ");
            strSql.Append("LEFT JOIN (SELECT HOUR(make_time) AS saleHour, sum(magnesium_total) AS magnesiumCount,");
            strSql.Append("SUM(aluminum_total) AS aluminumCount FROM parts_total WHERE ");
            strSql.Append("date_format(make_time, '%Y-%m-%d') = ");
            strSql.Append("'" + time + "'" + "and device_id = '" + deviceId + "' ");
            strSql.Append("GROUP BY HOUR (make_time) ORDER BY HOUR (make_time)) b ");
            strSql.Append("ON a.order_hour = b.saleHour ORDER BY order_hour");
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
                    string MakeTime = dataSet.Tables[0].Rows[i]["make_time"].ToString();
                    string time = "";
                    if (MakeTime.Length == 1)
                    {
                        time = "0" + MakeTime + ":00";
                    }
                    else
                    {
                        time = MakeTime + ":00";
                    }
                    int MagnesiumTotal = int.Parse(dataSet.Tables[0].Rows[i]["magnesium_total"].ToString());
                    int AluminumTotal = int.Parse(dataSet.Tables[0].Rows[i]["aluminum_total"].ToString());
                    map.Add("time", time);
                    map.Add("MagnesiumTotal", MagnesiumTotal);
                    map.Add("AluminumTotal", AluminumTotal);
                    mapList.Add(map);
                }
                return mapList;
            }
            else
            {
                return null;
            }
        }

        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM parts_total ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            object obj = DbHelperSQL.GetSingle(strSql.ToString());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }


        /// <summary>
        /// 实时产量统计 某台设备
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> RealTimeOutPut(string deviceId, string time)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT a.order_hour AS time, ifnull( b.aluminum_total, 0 ) AS aluminum,");
            strSql.Append("ifnull(b.magnesium_total,0) as magnesium FROM (");
            strSql.Append("SELECT 0 AS order_hour UNION ALL SELECT 1 AS order_hour UNION ALL ");
            strSql.Append("SELECT 2 AS order_hour UNION ALL SELECT 3 AS order_hour UNION ALL ");
            strSql.Append("SELECT 4 AS order_hour UNION ALL SELECT 5 AS order_hour UNION ALL ");
            strSql.Append("SELECT 6 AS order_hour UNION ALL SELECT 7 AS order_hour UNION ALL ");
            strSql.Append("SELECT 8 AS order_hour UNION ALL SELECT 9 AS order_hour UNION ALL ");
            strSql.Append("SELECT 10 AS order_hour UNION ALL SELECT 11 AS order_hour UNION ALL ");
            strSql.Append("SELECT 12 AS order_hour UNION ALL SELECT 13 AS order_hour UNION ALL ");
            strSql.Append("SELECT 14 AS order_hour UNION ALL SELECT 15 AS order_hour UNION ALL ");
            strSql.Append("SELECT 16 AS order_hour UNION ALL SELECT 17 AS order_hour UNION ALL ");
            strSql.Append("SELECT 18 AS order_hour UNION ALL SELECT 19 AS order_hour UNION ALL ");
            strSql.Append("SELECT 20 AS order_hour UNION ALL SELECT 21 AS order_hour UNION ALL ");
            strSql.Append("SELECT 22 AS order_hour UNION ALL SELECT 23 AS order_hour UNION ALL ");
            strSql.Append("SELECT 24 AS order_hour) a LEFT JOIN ( ");
            strSql.Append("SELECT HOUR (make_time) AS saleHour, sum(aluminum_total) AS aluminum_total,");
            strSql.Append("sum(magnesium_total) as magnesium_total FROM parts_total ");
            strSql.Append("WHERE date_format( make_time, '%Y-%m-%d' ) = '" + time + "' ");
            if (!string.IsNullOrEmpty(deviceId) && null != deviceId)
            {
                strSql.Append("AND device_id = '" + deviceId + "' ");
            }
            
            strSql.Append("GROUP BY (make_time) ORDER BY HOUR (make_time) ) b ON a.order_hour = b.saleHour ORDER BY order_hour");
            Console.WriteLine(strSql.ToString());
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<Dictionary<string, object>> mapList = dataSetToMapList(dataSet);
            return mapList;
        }

        /// <summary>
        /// 累计产量 输出每天的产量
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> CumulativeProduction(string deviceId, string startTime, string endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT DATE_FORMAT(make_time, '%Y-%m-%d' ) time,sum(magnesium_total ) magnesium,sum(aluminum_total ) aluminum ");
            strSql.Append("from parts_total WHERE make_time BETWEEN '" + startTime + "' and '" + endTime + "'");
            if (!string.IsNullOrEmpty(deviceId) && null != deviceId)
            {
                strSql.Append(" and device_id = '" + deviceId + "' ");
            }
            strSql.Append(" GROUP BY time ORDER BY time ASC");
            Console.WriteLine(strSql.ToString());
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<Dictionary<string, object>> mapList = dataSetToMapList(dataSet);
            return mapList;
        }

        /// <summary>
        /// 时段产量 输出每小时的产量
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> TimeinTervalProduction(string deviceId, string startTime, string endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT HOUR ( make_time ) order_hour, sum( magnesium_total ) magnesium, sum( aluminum_total ) aluminum ");
            strSql.Append("from parts_total WHERE make_time BETWEEN '" + startTime + "' and '" + endTime + "'");
            if (!string.IsNullOrEmpty(deviceId) && null != deviceId)
            {
                strSql.Append(" and device_id = '" + deviceId + "' ");
            }
            strSql.Append(" GROUP BY order_hour ORDER BY order_hour ASC");
            Console.WriteLine(strSql.ToString());
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<Dictionary<string, object>> mapList = dataSetToMapList(dataSet);
            return mapList;
        }

        public List<Dictionary<string, object>> dataSetToMapList(DataSet dataSet)
        {
            List<Dictionary<string, object>> mapList = new List<Dictionary<string, object>>();
            Console.WriteLine(dataSet.Tables[0].Rows.Count);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    string Time = dataSet.Tables[0].Rows[i]["time"].ToString();
                    int MagnesiumTotal = int.Parse(dataSet.Tables[0].Rows[i]["magnesium"].ToString());
                    int AluminumTotal = int.Parse(dataSet.Tables[0].Rows[i]["aluminum"].ToString());
                    map.Add("Time", Time);
                    map.Add("MagnesiumTotal", MagnesiumTotal);
                    map.Add("AluminumTotal", AluminumTotal);
                    map.Add("Total", MagnesiumTotal + AluminumTotal);
                    mapList.Add(map);
                }
                return mapList;
            }
            else
            {
                return null;
            }
        }



        public PartsTotal SetPartsTotal(CNCDto dto)
        {
            PartsTotal partsTotal = new PartsTotal();
            partsTotal.Id = GetRandomString();
            partsTotal.DeviceId = dto.DeviceId;
            partsTotal.MakeTime = DateTime.Now;
            partsTotal.CreateTime = DateTime.Now;
            return partsTotal;
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
            for (int i = 0; i < 11; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
        #endregion  BasicMethod
    }
}
