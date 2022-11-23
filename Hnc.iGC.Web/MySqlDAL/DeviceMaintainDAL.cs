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
                "purchase_date,durable_years,content,cycle,last_time,planned_time,actual_time,person_liable,early_warning_time,maintain_state,create_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_number,@device_name,@device_model,@device_type," +
                "@purchase_date,@durable_years,@content,@cycle,@last_time,@planned_time,@actual_time,@person_liable,@early_warning_time,@maintain_state,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@purchase_date",MySqlDbType.DateTime),
                new MySqlParameter("@durable_years",MySqlDbType.Int32),
                new MySqlParameter("@content",MySqlDbType.String),
                new MySqlParameter("@cycle",MySqlDbType.Int32),
                new MySqlParameter("@last_time",MySqlDbType.DateTime),
                new MySqlParameter("@planned_time",MySqlDbType.DateTime),
                new MySqlParameter("@actual_time",MySqlDbType.DateTime),
                new MySqlParameter("@person_liable",MySqlDbType.String),
                new MySqlParameter("@early_warning_time",MySqlDbType.Int32),
                new MySqlParameter("@maintain_state",MySqlDbType.Int32),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = maintain.Id;
            parameters[1].Value = maintain.DeviceNumber;
            parameters[2].Value = maintain.DeviceName;
            parameters[3].Value = maintain.DeviceModel;
            parameters[4].Value = maintain.DeviceType;
            parameters[5].Value = maintain.PurchaseDate;
            parameters[6].Value = maintain.DurableYears;
            parameters[7].Value = maintain.Content;
            parameters[8].Value = maintain.Cycle;
            parameters[9].Value = maintain.LastTime;
            parameters[10].Value = maintain.PlannedTime;
            parameters[11].Value = maintain.ActualTime;
            parameters[12].Value = maintain.PersonLiable;
            parameters[13].Value = maintain.EarlyWarningTime;
            parameters[14].Value = maintain.MaintainState;
            parameters[15].Value = maintain.CreateTime;
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
        /// 增加一条数据
        /// </summary>
        /// <param name="maintain"></param>
        /// <returns></returns>
        public bool AddHis(DeviceMaintain maintain)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into device_maintain_his(").Append("id,device_number,device_name,device_model,device_type," +
                "purchase_date,durable_years,content,cycle,last_time,planned_time,actual_time,person_liable,early_warning_time,maintain_state,create_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_number,@device_name,@device_model,@device_type," +
                "@purchase_date,@durable_years,@content,@cycle,@last_time,@planned_time,@actual_time,@person_liable,@early_warning_time,@maintain_state,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@purchase_date",MySqlDbType.DateTime),
                new MySqlParameter("@durable_years",MySqlDbType.Int32),
                new MySqlParameter("@content",MySqlDbType.String),
                new MySqlParameter("@cycle",MySqlDbType.Int32),
                new MySqlParameter("@last_time",MySqlDbType.DateTime),
                new MySqlParameter("@planned_time",MySqlDbType.DateTime),
                new MySqlParameter("@actual_time",MySqlDbType.DateTime),
                new MySqlParameter("@person_liable",MySqlDbType.String),
                new MySqlParameter("@early_warning_time",MySqlDbType.Int32),
                new MySqlParameter("@maintain_state",MySqlDbType.Int32),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = maintain.Id;
            parameters[1].Value = maintain.DeviceNumber;
            parameters[2].Value = maintain.DeviceName;
            parameters[3].Value = maintain.DeviceModel;
            parameters[4].Value = maintain.DeviceType;
            parameters[5].Value = maintain.PurchaseDate;
            parameters[6].Value = maintain.DurableYears;
            parameters[7].Value = maintain.Content;
            parameters[8].Value = maintain.Cycle;
            parameters[9].Value = maintain.LastTime;
            parameters[10].Value = maintain.PlannedTime;
            parameters[11].Value = maintain.ActualTime;
            parameters[12].Value = maintain.PersonLiable;
            parameters[13].Value = maintain.EarlyWarningTime;
            parameters[14].Value = maintain.MaintainState;
            parameters[15].Value = maintain.CreateTime;
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
            strSql.Append("purchase_date=@purchase_date,");
            strSql.Append("durable_years=@durable_years,");
            strSql.Append("content=@content,");
            strSql.Append("cycle=@cycle,");
            strSql.Append("last_time=@last_time,");
            strSql.Append("planned_time=@planned_time,");
            strSql.Append("actual_time=@actual_time,");
            strSql.Append("person_liable=@person_liable,");
            strSql.Append("early_warning_time=@early_warning_time,");
            strSql.Append("maintain_state=@maintain_state,");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where Id=@Id");
            MySqlParameter[] parameters =
            {
               new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@purchase_date",MySqlDbType.DateTime),
                new MySqlParameter("@durable_years",MySqlDbType.Int32),
                new MySqlParameter("@content",MySqlDbType.String),
                new MySqlParameter("@cycle",MySqlDbType.Int32),
                new MySqlParameter("@last_time",MySqlDbType.DateTime),
                new MySqlParameter("@planned_time",MySqlDbType.DateTime),
                new MySqlParameter("@actual_time",MySqlDbType.DateTime),
                new MySqlParameter("@person_liable",MySqlDbType.String),
                new MySqlParameter("@early_warning_time",MySqlDbType.Int32),
                new MySqlParameter("@maintain_state",MySqlDbType.Int32),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = maintain.Id;
            parameters[1].Value = maintain.DeviceNumber;
            parameters[2].Value = maintain.DeviceName;
            parameters[3].Value = maintain.DeviceModel;
            parameters[4].Value = maintain.DeviceType;
            parameters[5].Value = maintain.PurchaseDate;
            parameters[6].Value = maintain.DurableYears;
            parameters[7].Value = maintain.Content;
            parameters[8].Value = maintain.Cycle;
            parameters[9].Value = maintain.LastTime;
            parameters[10].Value = maintain.PlannedTime;
            parameters[11].Value = maintain.ActualTime;
            parameters[12].Value = maintain.PersonLiable;
            parameters[13].Value = maintain.EarlyWarningTime;
            parameters[14].Value = maintain.MaintainState;
            parameters[15].Value = maintain.UpdateTime;
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
            strSql.Append("DELETE FROM device_maintain where id = '" + id + "'");
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
            strSql.Append("select * FROM device_maintain where id = '" + id + "'");

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

                if (dataRow["device_name"] != null && dataRow["device_name"].ToString() != "")
                {
                    model.DeviceName = dataRow["device_name"].ToString();
                }
                if (dataRow["device_model"] != null && dataRow["device_model"].ToString() != "")
                {
                    model.DeviceModel = dataRow["device_model"].ToString();
                }
                if (dataRow["device_type"] != null && dataRow["device_type"].ToString() != "")
                {
                    model.DeviceType = dataRow["device_type"].ToString();
                }
                if (dataRow["purchase_date"] != null && dataRow["purchase_date"].ToString() != "")
                {
                    model.PurchaseDate = (DateTime)dataRow["purchase_date"];
                }

                if (dataRow["durable_years"] != null && dataRow["durable_years"].ToString() != "")
                {
                    model.DurableYears = int.Parse(dataRow["durable_years"].ToString());
                }

                if (dataRow["content"] != null && dataRow["content"].ToString() != "")
                {
                    model.Content = dataRow["content"].ToString();
                }

                if (dataRow["cycle"] != null && dataRow["cycle"].ToString() != "")
                {
                    model.Cycle = int.Parse(dataRow["cycle"].ToString());
                }

                if (dataRow["last_time"] != null && dataRow["last_time"].ToString() != "")
                {
                    model.LastTime = (DateTime)dataRow["last_time"];
                }
                if (dataRow["planned_time"] != null && dataRow["planned_time"].ToString() != "")
                {
                    model.PlannedTime = (DateTime)dataRow["planned_time"];
                }

                if (dataRow["actual_time"] != null && dataRow["actual_time"].ToString() != "")
                {
                    model.ActualTime = (DateTime)dataRow["actual_time"];
                }

                if (dataRow["person_liable"] != null && dataRow["person_liable"].ToString() != "")
                {
                    model.PersonLiable = dataRow["person_liable"].ToString();
                }
                if (dataRow["early_warning_time"] != null && dataRow["early_warning_time"].ToString() != "")
                {
                    model.EarlyWarningTime = int.Parse(dataRow["early_warning_time"].ToString());
                }
                if (dataRow["maintain_state"] != null && dataRow["maintain_state"].ToString() != "")
                {
                    model.MaintainState = int.Parse(dataRow["maintain_state"].ToString());
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
            strSql.Append("select count(1) count FROM device_maintain ");
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
        /// 设置modl
        /// </summary>
        /// <param name="maintain"></param>
        /// <returns></returns>
        public DeviceMaintain SetDeviceMaintain(DeviceMaintain maintain)
        {
            DeviceMaintain deviceMaintain = new DeviceMaintain();
            deviceMaintain.Id = GetRandomString();
            deviceMaintain.DeviceNumber = maintain.DeviceNumber;
            deviceMaintain.DeviceName = maintain.DeviceName;
            deviceMaintain.DeviceModel = maintain.DeviceModel;
            deviceMaintain.DeviceType = maintain.DeviceType;
            deviceMaintain.PurchaseDate = maintain.PurchaseDate;
            deviceMaintain.DurableYears = maintain.DurableYears;
            deviceMaintain.Content = maintain.Content;
            deviceMaintain.Cycle = maintain.Cycle;
            deviceMaintain.LastTime = maintain.LastTime;
            deviceMaintain.PlannedTime = maintain.PlannedTime;
            deviceMaintain.ActualTime = maintain.ActualTime;
            deviceMaintain.PersonLiable = maintain.PersonLiable;
            deviceMaintain.EarlyWarningTime = maintain.EarlyWarningTime;
            deviceMaintain.MaintainState = maintain.MaintainState;
            deviceMaintain.CreateTime = DateTime.Now;
            return deviceMaintain;
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
