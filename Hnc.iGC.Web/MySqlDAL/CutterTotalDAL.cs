using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class CutterTotalDAL
    {
        public CutterTotalDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="cutterTotal"></param>
        /// <returns></returns>
        public bool Add(CutterTotal cutterTotal)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into cutter_total(").Append("id,device_id,cutter_number,start_time,create_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_id,@cutter_number,@start_time,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@cutter_number",MySqlDbType.Int32),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = cutterTotal.Id;
            parameters[1].Value = cutterTotal.DeviceId;
            parameters[2].Value = cutterTotal.CutterNumber;
            parameters[3].Value = cutterTotal.StartTime;
            parameters[4].Value = cutterTotal.CreateTime;
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
        /// 根据ID修改一条数据
        /// </summary>
        /// <param name="cutterTotal"></param>
        /// <returns></returns>
        public bool Update(CutterTotal cutterTotal)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update cutter_total set ");
            strSql.Append(" id=@id,");
            strSql.Append("device_id=@device_id,");
            strSql.Append("cutter_number=@cutter_number,");
            strSql.Append("start_time=@start_time,");
            strSql.Append("end_time=@end_time,");
            strSql.Append("use_duration=@use_duration,");
            strSql.Append("create_time=@create_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters =
            {
                 new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@cutter_number",MySqlDbType.Int32),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@end_time",MySqlDbType.DateTime),
                new MySqlParameter("@use_duration",MySqlDbType.Double),
                new MySqlParameter("@create_time",MySqlDbType.DateTime) };
            parameters[0].Value = cutterTotal.DeviceId;
            parameters[1].Value = cutterTotal.CutterNumber;
            parameters[2].Value = cutterTotal.StartTime;
            parameters[3].Value = cutterTotal.EndTime;
            parameters[4].Value = cutterTotal.UseDuration;
            parameters[5].Value = cutterTotal.CreateTime;
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
        /// 根据设备ID和刀号查询一个对象
        /// </summary>
        /// <param name="devicId"></param>
        /// <param name="cutterNumber"></param>
        /// <returns></returns>
        public CutterTotal GetModelByParameters(string devicId,int cutterNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,device_id,cutter_number,start_time,end_time,use_duration,create_time from cutter_total ");
            strSql.Append("where device_id = ?");
            strSql.Append(" and cutter_number = ?");
            strSql.Append(" and end_time is null");
            strSql.Append(" and use_duration is null");
           
            MySqlParameter[] parameters =
            {
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@cutter_number",MySqlDbType.Int32)
            };
            parameters[0].Value = devicId;
            parameters[1].Value = cutterNumber;
           
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

        public CutterTotal DataRowToModel(DataRow dataRow)
        {
            CutterTotal model = new CutterTotal();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = dataRow["id"].ToString();
                }
                if (dataRow["device_id"] != null && dataRow["device_id"].ToString() != "")
                {
                    model.DeviceId = dataRow["device_id"].ToString();
                }
                if (dataRow["cutter_number"] != null && dataRow["cutter_number"].ToString() != "")
                {
                    model.CutterNumber = int.Parse(dataRow["cutter_number"].ToString());
                }

                if (dataRow["start_time"] != null && dataRow["start_time"].ToString() != "")
                {
                    model.StartTime = (DateTime)dataRow["start_time"];
                }

                if (dataRow["end_time"] != null && dataRow["end_time"].ToString() != "")
                {
                    model.EndTime = (DateTime)dataRow["end_time"];
                }
                if (dataRow["use_duration"] != null && dataRow["use_duration"].ToString() != "")
                {
                    model.UseDuration = double.Parse(dataRow["use_duration"].ToString());
                }
                if (dataRow["create_time"] != null && dataRow["create_time"].ToString() != "")
                {
                    model.EndTime = (DateTime)dataRow["create_time"];
                }
            }
            return model;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<CutterTotal> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,device_id,cutter_number,start_time,end_time,use_duration,create_time from cutter_total ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where "+strSql);
            }
            return DataRowToModelList(DbHelperMySQL.Query(strSql.ToString()));
        }



        public List<CutterTotal> DataRowToModelList(DataSet dataSet) 
        { 
            List<CutterTotal> cutterTotalList = new List<CutterTotal>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++) 
            {
                CutterTotal cutterTotal = DataRowToModel(dataSet.Tables[0].Rows[i]);
                cutterTotalList.Add(cutterTotal);
            } 
            return cutterTotalList;
        }

        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM cutter_total ");
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

        public CutterTotal SetCutterTotal(CNCDto dto) 
        {
            CutterTotal cutterTotal = new CutterTotal();
            cutterTotal.Id = GetRandomString();
            cutterTotal.DeviceId =dto.DeviceId;
            cutterTotal.CutterNumber = dto.CurrentCutterNumber;
            cutterTotal.StartTime = dto.SystemTime;
            cutterTotal.CreateTime = DateTime.Now;
            return cutterTotal;
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
