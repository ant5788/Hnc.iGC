using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class CalibrationPlanDAL
    {
        public CalibrationPlanDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="calibrationPlan"></param>
        /// <returns></returns>
        public bool Add(CalibrationPlan calibrationPlan)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into calibration_plan(").Append("id,plan_name,device_name,device_number,device_type,asset_number,start_time,end_time,plan_state,create_time)");
            strSql.Append(" values (@id,@plan_name,@device_name,@device_number,@device_type,@asset_number,@start_time,@end_time,@plan_state,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@plan_name",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@asset_number",MySqlDbType.String),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@end_time",MySqlDbType.DateTime),
                new MySqlParameter("@plan_state",MySqlDbType.Int32),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = calibrationPlan.Id;
            parameters[1].Value = calibrationPlan.PlanName;
            parameters[2].Value = calibrationPlan.DeviceName;
            parameters[3].Value = calibrationPlan.DeviceNumber;
            parameters[4].Value = calibrationPlan.DeviceType;
            parameters[5].Value = calibrationPlan.AssetNumber;
            parameters[6].Value = calibrationPlan.StartTime;
            parameters[7].Value = calibrationPlan.EndTime;
            parameters[8].Value = calibrationPlan.PlanState;
            parameters[9].Value = calibrationPlan.CreateTime;
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
        /// <param name="calibrationPlan"></param>
        /// <returns></returns>
        public bool Update(CalibrationPlan calibrationPlan)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update calibration_plan set ");
            strSql.Append(" id=@id,");
            strSql.Append("plan_name=@plan_name,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("device_number=@device_number,");
            strSql.Append("device_type=@device_type,");
            strSql.Append("asset_number=@asset_number,");
            strSql.Append("start_time=@start_time,");
            strSql.Append("end_time=@end_time,");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@plan_name",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@asset_number",MySqlDbType.String),
                new MySqlParameter("@start_time",MySqlDbType.DateTime),
                new MySqlParameter("@end_time",MySqlDbType.DateTime),
                new MySqlParameter("@plan_state",MySqlDbType.Int32),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = calibrationPlan.Id;
            parameters[1].Value = calibrationPlan.PlanName;
            parameters[2].Value = calibrationPlan.DeviceName;
            parameters[3].Value = calibrationPlan.DeviceNumber;
            parameters[4].Value = calibrationPlan.DeviceType;
            parameters[5].Value = calibrationPlan.AssetNumber;
            parameters[6].Value = calibrationPlan.StartTime;
            parameters[7].Value = calibrationPlan.EndTime;
            parameters[8].Value = calibrationPlan.PlanState;
            parameters[9].Value = calibrationPlan.UpdateTime;
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
        public List<CalibrationPlan> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from calibration_plan ");
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
            strSql.Append("DELETE FROM calibration_plan where id = '" + id + "'");
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
        public CalibrationPlan GetById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM calibration_plan where id = '" + id + "'");
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
        public CalibrationPlan DataRowToModel(DataRow dataRow)
        {
            CalibrationPlan model = new CalibrationPlan();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = dataRow["id"].ToString();
                }
                if (dataRow["plan_name"] != null && dataRow["plan_name"].ToString() != "")
                {
                    model.PlanName = dataRow["plan_name"].ToString();
                }
                if (dataRow["device_name"] != null && dataRow["device_name"].ToString() != "")
                {
                    model.DeviceName = dataRow["device_name"].ToString();
                }

               
                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DeviceNumber = dataRow["device_number"].ToString();
                }
                if (dataRow["device_type"] != null && dataRow["device_type"].ToString() != "")
                {
                    model.DeviceType = dataRow["device_type"].ToString();
                }

                if (dataRow["asset_number"] != null && dataRow["asset_number"].ToString() != "")
                {
                    model.AssetNumber = dataRow["asset_number"].ToString();
                }

                if (dataRow["start_time"] != null && dataRow["start_time"].ToString() != "")
                {
                    model.StartTime = (DateTime)dataRow["start_time"];
                }
                if (dataRow["end_time"] != null && dataRow["end_time"].ToString() != "")
                {
                    model.EndTime = (DateTime)dataRow["end_time"];
                }
                if (dataRow["plan_state"] != null && dataRow["plan_state"].ToString() != "")
                {
                    model.PlanState = int.Parse(dataRow["plan_state"].ToString());
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
        public List<CalibrationPlan> DataRowToModelList(DataSet dataSet)
        {
            List<CalibrationPlan> planList = new List<CalibrationPlan>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                CalibrationPlan plan = DataRowToModel(dataSet.Tables[0].Rows[i]);
                planList.Add(plan);
            }
            return planList;
        }

        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) as count FROM calibration_plan ");
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
        /// <param name="calibration"></param>
        /// <returns></returns>
        public CalibrationPlan SetCalibrationPlan(CalibrationPlan calibration)
        {
            CalibrationPlan plan = new CalibrationPlan();
            plan.Id = GetRandomString();
            plan.PlanName = calibration.PlanName;
            plan.DeviceName = calibration.DeviceName;
            plan.DeviceNumber = calibration.DeviceNumber;
            plan.DeviceType = calibration.DeviceType;
            plan.AssetNumber = calibration.AssetNumber;
            plan.StartTime = calibration.StartTime;
            plan.EndTime = calibration.EndTime;
            plan.CreateTime = DateTime.Now;
            return plan;
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

        #endregion  BasicMethod
    }
}
