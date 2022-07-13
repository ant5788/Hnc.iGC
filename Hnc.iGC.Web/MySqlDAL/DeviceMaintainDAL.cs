using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class DeviceMaintainDAL
    {
        public DeviceMaintainDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="maintain"></param>
        /// <returns></returns>
        public bool Add(DeviceMaintain maintain)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into device_maintain(").Append("id,device_number,device_name,device_model,device_type," +
                "device_state,content,cycle,device_classification,person_liable,user_dep,create_time,update_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_number,@device_name,@device_model,@device_type,@device_state," +
                "@content,@cycle,@device_classification,@person_liable,@user_dep,@create_time,@update_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_state",MySqlDbType.String),
                new MySqlParameter("@content",MySqlDbType.String),
                new MySqlParameter("@cycle",MySqlDbType.String),
                new MySqlParameter("@device_classification",MySqlDbType.String),
                new MySqlParameter("@person_liable",MySqlDbType.String),
                new MySqlParameter("@user_dep",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = maintain.Id;
            parameters[1].Value = maintain.DeviceNumber;
            parameters[2].Value = maintain.DeviceName;
            parameters[3].Value = maintain.DeviceModel;
            parameters[4].Value = maintain.DeviceType;
            parameters[5].Value = maintain.DeviceState;
            parameters[6].Value = maintain.Content;
            parameters[7].Value = maintain.Cycle;
            parameters[8].Value = maintain.DeviceClassification;
            parameters[9].Value = maintain.PersonLiable;
            parameters[10].Value = maintain.UserDep;
            parameters[11].Value = maintain.CreateTime;
            parameters[12].Value = maintain.UpdateTime;
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
        /// <param name="maintain"></param>
        /// <returns></returns>
        public bool Update(DeviceMaintain maintain)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update device_maintain set ");
            strSql.Append(" id=@id,");
            strSql.Append("device_number=@device_number,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("device_model=@device_model,");
            strSql.Append("device_type=@device_type,");
            strSql.Append("device_state=@device_state,");
            strSql.Append("content=@content");
            strSql.Append("cycle=@cycle");
            strSql.Append("device_classification=@device_classification");
            strSql.Append("person_liable=@person_liable");
            strSql.Append("user_dep=@user_dep");
            strSql.Append("create_time=@create_time");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters =
            {
               new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_state",MySqlDbType.String),
                new MySqlParameter("@content",MySqlDbType.String),
                new MySqlParameter("@cycle",MySqlDbType.String),
                new MySqlParameter("@device_classification",MySqlDbType.String),
                new MySqlParameter("@person_liable",MySqlDbType.String),
                new MySqlParameter("@user_dep",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = maintain.Id;
            parameters[1].Value = maintain.DeviceNumber;
            parameters[2].Value = maintain.DeviceName;
            parameters[3].Value = maintain.DeviceModel;
            parameters[4].Value = maintain.DeviceType;
            parameters[5].Value = maintain.DeviceState;
            parameters[6].Value = maintain.Content;
            parameters[7].Value = maintain.Cycle;
            parameters[7].Value = maintain.DeviceClassification;
            parameters[7].Value = maintain.PersonLiable;
            parameters[7].Value = maintain.UserDep;
            parameters[7].Value = maintain.CreateTime;
            parameters[7].Value = maintain.UpdateTime;
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
        public List<DeviceMaintain> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from device_maintain ");
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
            strSql.Append("DELETE * FROM device_maintain where id = '" + id+"'");
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
        public DeviceMaintain GetById(string id) 
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
        public DeviceMaintain DataRowToModel(DataRow dataRow)
        {
            DeviceMaintain model = new DeviceMaintain();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = dataRow["id"].ToString();
                }
                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DeviceNumber = dataRow["device_number"].ToString();
                }

                if (dataRow["device_type"] != null && dataRow["device_type"].ToString() != "")
                {
                    model.DeviceName = dataRow["device_type"].ToString();
                }
                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DeviceModel = dataRow["device_number"].ToString();
                }
                if (dataRow["asset_number"] != null && dataRow["asset_number"].ToString() != "")
                {
                    model.DeviceType = dataRow["asset_number"].ToString();
                }
                if (dataRow["archives_number"] != null && dataRow["archives_number"].ToString() != "")
                {
                    model.DeviceState = dataRow["archives_number"].ToString();
                }
                
                if (dataRow["archives_number"] != null && dataRow["archives_number"].ToString() != "")
                {
                    model.Content = dataRow["archives_number"].ToString();
                }
                
                if (dataRow["archives_number"] != null && dataRow["archives_number"].ToString() != "")
                {
                    model.Cycle = dataRow["archives_number"].ToString();
                }
                
                if (dataRow["device_classification"] != null && dataRow["device_classification"].ToString() != "")
                {
                    model.DeviceClassification = dataRow["device_classification"].ToString();
                }                
                if (dataRow["person_liable"] != null && dataRow["person_liable"].ToString() != "")
                {
                    model.PersonLiable = dataRow["person_liable"].ToString();
                }                
                if (dataRow["user_dep"] != null && dataRow["user_dep"].ToString() != "")
                {
                    model.UserDep = dataRow["user_dep"].ToString();
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
        public List<DeviceMaintain> DataRowToModelList(DataSet dataSet)
        {
            List<DeviceMaintain> MaintainList = new List<DeviceMaintain>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DeviceMaintain maintain = DataRowToModel(dataSet.Tables[0].Rows[i]);
                MaintainList.Add(maintain);
            }
            return MaintainList;
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
