using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class DeviceArchivesDAL
    {
        public DeviceArchivesDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="deviceArchives"></param>
        /// <returns></returns>
        public bool Add(DeviceArchives deviceArchives)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into device_archives(").Append("id,device_name,device_number,device_type,device_model,purchase_date,durable_years,create_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_name,@device_number,@device_type,@device_model,@purchase_date,@durable_years,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@purchase_date",MySqlDbType.DateTime),
                new MySqlParameter("@durable_years",MySqlDbType.Int32),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = deviceArchives.Id;
            parameters[1].Value = deviceArchives.DeviceName;
            parameters[2].Value = deviceArchives.DerviceNumber;
            parameters[3].Value = deviceArchives.DeviceType;
            parameters[4].Value = deviceArchives.DeviceModel;
            parameters[5].Value = deviceArchives.PurchaseDate;
            parameters[6].Value = deviceArchives.DurableYears;
            parameters[7].Value = deviceArchives.CreateTime;
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
        /// <param name="archives"></param>
        /// <returns></returns>
        public bool Update(DeviceArchives archives)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update device_archives set ");
            strSql.Append(" id=@id,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("device_number=@device_number,");
            strSql.Append("device_type=@device_type,");
            strSql.Append("device_model=@device_model,");
            strSql.Append("purchase_date=@purchase_date,");
            strSql.Append("durable_years=@durable_years,");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@purchase_date",MySqlDbType.DateTime),
                new MySqlParameter("@durable_years",MySqlDbType.Int32),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = archives.Id;
            parameters[1].Value = archives.DeviceName;
            parameters[2].Value = archives.DerviceNumber;
            parameters[3].Value = archives.DeviceType;
            parameters[4].Value = archives.DeviceModel;
            parameters[5].Value = archives.PurchaseDate;
            parameters[6].Value = archives.DurableYears;
            parameters[7].Value = archives.UpdateTime;
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
        public List<DeviceArchives> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from device_archives ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
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
            strSql.Append("DELETE FROM device_archives where id = '" + id + "'");
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
        /// 通过ID查询单调数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DeviceArchives GetById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM device_archives where id = '" + id + "'");
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
        public DeviceArchives DataRowToModel(DataRow dataRow)
        {
            DeviceArchives model = new DeviceArchives();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = dataRow["id"].ToString();
                }
                if (dataRow["device_name"] != null && dataRow["device_name"].ToString() != "")
                {
                    model.DeviceName = dataRow["device_name"].ToString();
                }
                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DerviceNumber = dataRow["device_number"].ToString();
                }

                if (dataRow["device_type"] != null && dataRow["device_type"].ToString() != "")
                {
                    model.DeviceType = dataRow["device_type"].ToString();
                }
                
                if (dataRow["device_model"] != null && dataRow["device_model"].ToString() != "")
                {
                    model.DeviceModel = dataRow["device_model"].ToString();
                }
                if (dataRow["purchase_date"] != null && dataRow["purchase_date"].ToString() != "")
                {
                    model.PurchaseDate = (DateTime)dataRow["purchase_date"];
                }

                if (dataRow["durable_years"] != null && dataRow["durable_years"].ToString() != "")
                {
                    model.DurableYears = int.Parse(dataRow["durable_years"].ToString());
                }

                if (dataRow["create_time"] != null && dataRow["create_time"].ToString() != "")
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
        public List<DeviceArchives> DataRowToModelList(DataSet dataSet)
        {
            List<DeviceArchives> deviceArchivesList = new List<DeviceArchives>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DeviceArchives archives = DataRowToModel(dataSet.Tables[0].Rows[i]);
                deviceArchivesList.Add(archives);
            }
            return deviceArchivesList;
        }

        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) as count FROM device_archives ");
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

        public DeviceArchives GetDeviceInfoByNumver(string number) 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM device_archives where  device_number = '"+number+"'");
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
        /// 设置modl
        /// </summary>
        /// <param name="archives"></param>
        /// <returns></returns>
        public DeviceArchives SetDeviceArchives(DeviceArchives archives)
        {
            DeviceArchives deviceArchives = new DeviceArchives();
            deviceArchives.Id = GetRandomString();
            deviceArchives.DeviceName = archives.DeviceName;
            deviceArchives.DeviceType = archives.DeviceType;
            deviceArchives.DerviceNumber = archives.DerviceNumber;
            deviceArchives.DeviceModel = archives.DeviceModel;
            deviceArchives.PurchaseDate = archives.PurchaseDate;
            deviceArchives.DurableYears = archives.DurableYears;
            deviceArchives.CreateTime = DateTime.Now;
            return deviceArchives;
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

        #endregion  BasicMethod
    }
}
