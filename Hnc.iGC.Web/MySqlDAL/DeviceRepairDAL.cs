using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class DeviceRepairDAL
    {
        public DeviceRepairDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="deviceRepair"></param>
        /// <returns></returns>
        public bool Add(DeviceRepair deviceRepair)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into device_repair(").Append("id,device_name,device_type,device_number,device_model,purchase_date," +
                "durable_years,start_time,end_time,reason,repair_personnel,repair_state,repair_duration,repair_cost,create_time)");
            strSql.Append(" values (@id,@device_name,@device_type,@device_number,@device_model,@purchase_date," +
                "@durable_years,@start_time,@end_time,@reason,@repair_personnel,@repair_state,@repair_duration,@repair_cost,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@purchase_date",MySqlDbType.DateTime),
                new MySqlParameter("@durable_years",MySqlDbType.Int32),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@end_time",MySqlDbType.DateTime),
                new MySqlParameter("@reason",MySqlDbType.String),
                new MySqlParameter("@repair_personnel",MySqlDbType.String),
                new MySqlParameter("@repair_state",MySqlDbType.Int32),
                new MySqlParameter("@repair_duration",MySqlDbType.Double),
                new MySqlParameter("@repair_cost",MySqlDbType.Decimal),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = deviceRepair.Id;
            parameters[1].Value = deviceRepair.DeviceName;
            parameters[2].Value = deviceRepair.DeviceType;
            parameters[3].Value = deviceRepair.DeviceNumber;
            parameters[4].Value = deviceRepair.DeviceModel;
            parameters[5].Value = deviceRepair.PurchaseDate;
            parameters[6].Value = deviceRepair.DurableYars;
            parameters[7].Value = deviceRepair.StartTime;
            parameters[8].Value = deviceRepair.EndTime;
            parameters[9].Value = deviceRepair.reason;
            parameters[10].Value = deviceRepair.RepairPersonnel;
            parameters[11].Value = deviceRepair.RepairState;
            parameters[12].Value = deviceRepair.RepairDuration;
            parameters[13].Value = deviceRepair.RepairCost;
            parameters[14].Value = deviceRepair.CreateTime;
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
        /// <param name="deviceRepair"></param>
        /// <returns></returns>
        public bool Update(DeviceRepair deviceRepair)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update device_repair set");
            strSql.Append(" id=@id,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("device_type=@device_type,");
            strSql.Append("device_number=@device_number,");
            strSql.Append("device_model=@device_model,");
            strSql.Append("purchase_date=@purchase_date,");
            strSql.Append("durable_years=@durable_years,");
            strSql.Append("start_time=@start_time,");
            strSql.Append("end_time=@end_time,");
            strSql.Append("reason=@reason,");
            strSql.Append("repair_personnel=@repair_personnel,");
            strSql.Append("repair_state=@repair_state,");
            strSql.Append("repair_duration=@repair_duration,");
            strSql.Append("repair_cost=@repair_cost,");
            strSql.Append("update_time=@update_time ");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@purchase_date",MySqlDbType.DateTime),
                new MySqlParameter("@durable_years",MySqlDbType.Int32),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@end_time",MySqlDbType.DateTime),
                new MySqlParameter("@reason",MySqlDbType.String),
                new MySqlParameter("@repair_personnel",MySqlDbType.String),
                new MySqlParameter("@repair_state",MySqlDbType.Int32),
                new MySqlParameter("@repair_duration",MySqlDbType.Double),
                new MySqlParameter("@repair_cost",MySqlDbType.Decimal),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = deviceRepair.Id;
            parameters[1].Value = deviceRepair.DeviceName;
            parameters[2].Value = deviceRepair.DeviceType;
            parameters[3].Value = deviceRepair.DeviceNumber;
            parameters[4].Value = deviceRepair.DeviceModel;
            parameters[5].Value = deviceRepair.PurchaseDate;
            parameters[6].Value = deviceRepair.DurableYars;
            parameters[7].Value = deviceRepair.StartTime;
            parameters[8].Value = deviceRepair.EndTime;
            parameters[9].Value = deviceRepair.reason;
            parameters[10].Value = deviceRepair.RepairPersonnel;
            parameters[11].Value = deviceRepair.RepairState;
            parameters[12].Value = deviceRepair.RepairDuration;
            parameters[13].Value = deviceRepair.RepairCost;
            parameters[14].Value = deviceRepair.UpdateTime;
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
        public List<DeviceRepair> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from device_repair ");
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
            strSql.Append("DELETE FROM device_repair where id = '" + id+"'");
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
        public DeviceRepair GetById(string id) 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM device_repair where id = '" + id + "'");
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
        public DeviceRepair DataRowToModel(DataRow dataRow)
        {
            DeviceRepair model = new DeviceRepair();
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
                if (dataRow["device_type"] != null && dataRow["device_type"].ToString() != "")
                {
                    model.DeviceType = dataRow["device_type"].ToString();
                }

                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DeviceNumber = dataRow["device_number"].ToString();
                }

                if (dataRow["device_model"] != null && dataRow["device_model"].ToString() != "")
                {
                    model.DeviceModel = dataRow["device_model"].ToString();
                }

                if (dataRow["purchase_date"] != null && dataRow["purchase_date"].ToString() != "")
                {
                    model.PurchaseDate =(DateTime)dataRow["purchase_date"];
                }
                if (dataRow["durable_years"] != null && dataRow["durable_years"].ToString() != "")
                {
                    model.DurableYars = int.Parse(dataRow["durable_years"].ToString());
                }
             
                if (dataRow["start_time"] != null && dataRow["start_time"].ToString() != "")
                {
                    model.StartTime = (DateTime)dataRow["start_time"];
                }

                if (dataRow["end_time"] != null && dataRow["end_time"].ToString() != "")
                {
                    model.EndTime = (DateTime)dataRow["end_time"];
                }

                if (dataRow["reason"] != null && dataRow["reason"].ToString() != "")
                {
                    model.reason = dataRow["reason"].ToString();
                }

                if (dataRow["repair_personnel"] != null && dataRow["repair_personnel"].ToString() != "")
                {
                    model.RepairPersonnel = dataRow["repair_personnel"].ToString();
                }
                if (dataRow["repair_state"] != null && dataRow["repair_state"].ToString() != "")
                {
                    model.RepairState = int.Parse(dataRow["repair_state"].ToString());
                }


                if (dataRow["repair_duration"] != null && dataRow["repair_duration"].ToString() != "")
                {
                    model.RepairDuration = double.Parse(dataRow["repair_duration"].ToString());
                }
                if (dataRow["repair_cost"] != null && dataRow["repair_cost"].ToString() != "")
                {
                    model.RepairCost = decimal.Parse(dataRow["repair_cost"].ToString());
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
        public List<DeviceRepair> DataRowToModelList(DataSet dataSet)
        {
            List<DeviceRepair> deviceRepairsList = new List<DeviceRepair>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DeviceRepair deviceRepair = DataRowToModel(dataSet.Tables[0].Rows[i]);
                deviceRepairsList.Add(deviceRepair);
            }
            return deviceRepairsList;
        }

        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) count FROM device_repair ");
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
        /// <param name="deviceRepair"></param>
        /// <returns></returns>
        public DeviceRepair SetDeviceRepair(DeviceRepair deviceRepair)
        {
            DeviceRepair repair = new DeviceRepair();
            repair.Id = GetRandomString();
            repair.DeviceName = deviceRepair.DeviceName;
            repair.DeviceType = deviceRepair.DeviceType;
            repair.DeviceNumber = deviceRepair.DeviceNumber;
            repair.DeviceModel = deviceRepair.DeviceModel;
            repair.PurchaseDate = deviceRepair.PurchaseDate;
            repair.DurableYars = deviceRepair.DurableYars;
            repair.StartTime = deviceRepair.StartTime;
            repair.EndTime = deviceRepair.EndTime;
            repair.reason = deviceRepair.reason;
            repair.RepairPersonnel = deviceRepair.RepairPersonnel;
            repair.RepairState = deviceRepair.RepairState;
            repair.RepairDuration = deviceRepair.RepairDuration;
            repair.RepairCost = deviceRepair.RepairCost;
            repair.CreateTime = DateTime.Now;
            return repair;
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
            //string fguid = Guid.NewGuid().ToString().Replace("-", "");
            return s;
        }

        #endregion  BasicMethod
    }
}
