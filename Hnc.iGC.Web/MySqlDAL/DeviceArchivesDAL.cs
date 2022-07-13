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
        /// <param name="cutterTotal"></param>
        /// <returns></returns>
        public bool Add(DeviceArchives deviceArchives)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into device_archives(").Append("id,device_name,device_type,device_number,asset_number,archives_number,create_time,update_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_name,@device_type,@device_number,@asset_number,@archives_number,@,archives_number,@create_time,@update_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@asset_number",MySqlDbType.String),
                new MySqlParameter("@archives_number",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = deviceArchives.Id;
            parameters[1].Value = deviceArchives.DeviceName;
            parameters[2].Value = deviceArchives.DeviceType;
            parameters[3].Value = deviceArchives.DerviceNumber;
            parameters[4].Value = deviceArchives.AssetBumber;
            parameters[5].Value = deviceArchives.archivesNumber;
            parameters[6].Value = deviceArchives.CreateTime;
            parameters[7].Value = deviceArchives.UpdateTime;
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
        public bool Update(DeviceArchives archives)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update device_archives set ");
            strSql.Append(" id=@id,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("device_type=@device_type,");
            strSql.Append("device_number=@device_number,");
            strSql.Append("asset_number=@asset_number,");
            strSql.Append("archives_number=@archives_number,");
            strSql.Append("create_time=@create_time");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters =
            {
                 new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@asset_number",MySqlDbType.String),
                new MySqlParameter("@archives_number",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime) };
            parameters[0].Value = archives.Id;
            parameters[1].Value = archives.DeviceName;
            parameters[2].Value = archives.DeviceType;
            parameters[3].Value = archives.DerviceNumber;
            parameters[4].Value = archives.AssetBumber;
            parameters[5].Value = archives.archivesNumber;
            parameters[6].Value = archives.CreateTime;
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
            strSql.Append("DELETE * FROM device_archives where id = '"+id+"'");
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

                if (dataRow["device_type"] != null && dataRow["device_type"].ToString() != "")
                {
                    model.DeviceType = dataRow["device_type"].ToString();
                }
                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DerviceNumber = dataRow["device_number"].ToString();
                }
                if (dataRow["asset_number"] != null && dataRow["asset_number"].ToString() != "")
                {
                    model.AssetBumber = dataRow["asset_number"].ToString();
                }
                if (dataRow["archives_number"] != null && dataRow["archives_number"].ToString() != "")
                {
                    model.archivesNumber = dataRow["archives_number"].ToString();
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
            strSql.Append("select count(1) FROM device_archives ");
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
        /// <param name="archives"></param>
        /// <returns></returns>
        public DeviceArchives SetDeviceArchives(DeviceArchives archives)
        {
            DeviceArchives deviceArchives = new DeviceArchives();
            deviceArchives.Id = GetRandomString();
            deviceArchives.DeviceName = archives.DeviceName;
            deviceArchives.DeviceType = archives.DeviceType;
            deviceArchives.DerviceNumber = archives.DerviceNumber;
            deviceArchives.AssetBumber = archives.AssetBumber;
            deviceArchives.archivesNumber = archives.archivesNumber;
            deviceArchives.CreateTime = DateTime.Now;
            deviceArchives.UpdateTime = DateTime.Now;
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
            for (int i = 0; i < 11; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        #endregion  BasicMethod
    }
}
