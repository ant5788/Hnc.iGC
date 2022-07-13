using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class CheckPointDAL
    {
        public CheckPointDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="checkPoint"></param>
        /// <returns></returns>
        public bool Add(CheckPoint checkPoint)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into check_point(").Append("id,device_name,device_number,assets_number,type," +
                "status,start_time,end_time,details,inspector,create_time,update_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_name,@device_number,@assets_number,@type,@status," +
                "@start_time,@end_time,@details,@inspector,@create_time,@update_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@assets_number",MySqlDbType.String),
                new MySqlParameter("@type",MySqlDbType.Int32),
                new MySqlParameter("@status",MySqlDbType.Int32),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@end_time",MySqlDbType.DateTime),
                new MySqlParameter("@details",MySqlDbType.String),
                new MySqlParameter("@inspector",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = checkPoint.Id;
            parameters[1].Value = checkPoint.DeviceName;
            parameters[2].Value = checkPoint.DeviceNumber;
            parameters[3].Value = checkPoint.AssetNumber;
            parameters[4].Value = checkPoint.Type;
            parameters[5].Value = checkPoint.State;
            parameters[6].Value = checkPoint.StartTime;
            parameters[7].Value = checkPoint.EndTime;
            parameters[8].Value = checkPoint.Details;
            parameters[9].Value = checkPoint.Inspector;
            parameters[10].Value = checkPoint.CreateTime;
            parameters[11].Value = checkPoint.UpdateTime;
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
        /// <param name="checkPoint"></param>
        /// <returns></returns>
        public bool Update(CheckPoint checkPoint)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update check_point set ");
            strSql.Append(" id=@id,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("device_number=@device_number,");
            strSql.Append("assets_number=@assets_number,");
            strSql.Append("type=@type,");
            strSql.Append("status=@status,");
            strSql.Append("start_time=@start_time");
            strSql.Append("end_time=@end_time");
            strSql.Append("details=@details");
            strSql.Append("inspector=@inspector");
            strSql.Append("create_time=@create_time");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters =
            {
               new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@assets_number",MySqlDbType.String),
                new MySqlParameter("@type",MySqlDbType.Int32),
                new MySqlParameter("@status",MySqlDbType.Int32),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@end_time",MySqlDbType.DateTime),
                new MySqlParameter("@details",MySqlDbType.String),
                new MySqlParameter("@inspector",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = checkPoint.Id;
            parameters[1].Value = checkPoint.DeviceName;
            parameters[2].Value = checkPoint.DeviceNumber;
            parameters[3].Value = checkPoint.AssetNumber;
            parameters[4].Value = checkPoint.Type;
            parameters[5].Value = checkPoint.State;
            parameters[6].Value = checkPoint.StartTime;
            parameters[7].Value = checkPoint.EndTime;
            parameters[8].Value = checkPoint.Details;
            parameters[9].Value = checkPoint.Inspector;
            parameters[10].Value = checkPoint.CreateTime;
            parameters[11].Value = checkPoint.UpdateTime;
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
        public List<CheckPoint> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from check_point ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strSql);
            }
            return DataRowToModelList(DbHelperMySQL.Query(strSql.ToString()));
        }

        /// <summary>
        /// 通过ID删除一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean DeleteById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("DELETE * FROM check_point where id = '" + id+"'");
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
        /// 查询详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CheckPoint GetById(string id) 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM check_point where id = '" + id + "'");
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
        /// 数据转为对象 toModel
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public CheckPoint DataRowToModel(DataRow dataRow)
        {
            CheckPoint model = new CheckPoint();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = dataRow["ID"].ToString();
                }
                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DeviceName = dataRow["device_number"].ToString();
                }

                if (dataRow["device_type"] != null && dataRow["device_type"].ToString() != "")
                {
                    model.DeviceNumber = dataRow["device_type"].ToString();
                }
                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.AssetNumber = dataRow["device_number"].ToString();
                }
                if (dataRow["asset_number"] != null && dataRow["asset_number"].ToString() != "")
                {
                    model.Type = Int32.Parse(dataRow["asset_number"].ToString());
                }
                if (dataRow["status"] != null && dataRow["status"].ToString() != "")
                {
                    model.State = Int32.Parse(dataRow["status"].ToString());
                }
                
                if (dataRow["start_time"] != null && dataRow["start_time"].ToString() != "")
                {
                    model.StartTime = (DateTime)dataRow["start_time"];
                }

                if (dataRow["end_time"] != null && dataRow["end_time"].ToString() != "")
                {
                    model.EndTime = (DateTime)dataRow["end_time"];
                }
                if (dataRow["details"] != null && dataRow["details"].ToString() != "")
                {
                    model.Details = dataRow["details"].ToString();
                }

                if (dataRow["inspector"] != null && dataRow["inspector"].ToString() != "")
                {
                    model.Inspector = dataRow["inspector"].ToString();
                }

                if (dataRow["update_time"] != null && dataRow["update_time"].ToString() != "")
                {
                    model.CreateTime = (DateTime)dataRow["create_time"];
                }
                
                if (dataRow["update_time"] != null && dataRow["update_time"].ToString() != "")
                {
                    model.UpdateTime = (DateTime)dataRow["create_time"];
                }
            }
            return model;
        }

        /// <summary>
        /// 转list对象数据
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public List<CheckPoint> DataRowToModelList(DataSet dataSet)
        {
            List<CheckPoint> checkPointList = new List<CheckPoint>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                CheckPoint checkPoint = DataRowToModel(dataSet.Tables[0].Rows[i]);
                checkPointList.Add(checkPoint);
            }
            return checkPointList;
        }

        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM device_maintain ");
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
        /// 设置modl
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CheckPoint SetCheckPoint(CheckPoint point)
        {
            CheckPoint checkPoint = new CheckPoint();
            checkPoint.Id = GetRandomString();
            checkPoint.DeviceName = point.DeviceName;
            checkPoint.DeviceNumber = point.DeviceNumber;
            checkPoint.AssetNumber = point.AssetNumber;
            checkPoint.Type = point.Type;
            checkPoint.State = point.State;
            checkPoint.StartTime = DateTime.Now;
            checkPoint.EndTime = point.EndTime;
            checkPoint.Details = point.Details;
            checkPoint.Inspector = point.Inspector;
            checkPoint.CreateTime = DateTime.Now;
            checkPoint.UpdateTime = point.UpdateTime;
            return checkPoint;
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
