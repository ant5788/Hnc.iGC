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
            strSql.Append("insert into check_point(").Append("Id,number,operator,device_name,device_number,device_model,spare_parts,liquid,pressure,Handle,safety_devices," +
                "instnt_pressure,fan_screen,drive_motor,leakage_oil_gas_water,principal_axis,appearance,electrical_part,create_time)");
            strSql.Append(" values (@Id,@number,@operator,@device_name,@device_number,@device_model,@spare_parts,@liquid,@pressure,@Handle,@safety_devices," +
                "@instnt_pressure,@fan_screen,@drive_motor,@leakage_oil_gas_water,@principal_axis,@appearance,@electrical_part,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@number",MySqlDbType.String),
                new MySqlParameter("@operator",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@spare_parts",MySqlDbType.Int32),
                new MySqlParameter("@liquid",MySqlDbType.Int32),
                new MySqlParameter("@pressure",MySqlDbType.Int32),
                new MySqlParameter("@Handle",MySqlDbType.Int32),
                new MySqlParameter("@safety_devices",MySqlDbType.Int32),
                new MySqlParameter("@instnt_pressure",MySqlDbType.Int32),
                new MySqlParameter("@fan_screen",MySqlDbType.Int32),
                new MySqlParameter("@drive_motor",MySqlDbType.Int32),
                new MySqlParameter("@leakage_oil_gas_water",MySqlDbType.Int32),
                new MySqlParameter("@principal_axis",MySqlDbType.Int32),
                new MySqlParameter("@appearance",MySqlDbType.Int32),
                new MySqlParameter("@electrical_part",MySqlDbType.Int32),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = checkPoint.Id;
            parameters[1].Value = checkPoint.Number;
            parameters[2].Value = checkPoint.Operator;
            parameters[3].Value = checkPoint.DeviceName;
            parameters[4].Value = checkPoint.DeviceNumber;
            parameters[5].Value = checkPoint.DeviceModel;
            parameters[6].Value = checkPoint.SpareParts;
            parameters[7].Value = checkPoint.Liquid;
            parameters[8].Value = checkPoint.Pressure;
            parameters[9].Value = checkPoint.Handle;
            parameters[10].Value = checkPoint.SafetyDevices;
            parameters[11].Value = checkPoint.InstrumentPressure;
            parameters[12].Value = checkPoint.FanScreen;
            parameters[13].Value = checkPoint.DriveMotor;
            parameters[14].Value = checkPoint.LeakageOilGasWater;
            parameters[15].Value = checkPoint.PrincipalAxis;
            parameters[16].Value = checkPoint.Appearance;
            parameters[17].Value = checkPoint.ElectricalPart;
            parameters[18].Value = checkPoint.CreateTime;
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
            strSql.Append("number=@number,");
            strSql.Append("operator=@operator,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("device_number=@device_number,");
            strSql.Append("device_model=@device_model,");
            strSql.Append("spare_parts=@spare_parts,");
            strSql.Append("liquid=@liquid,");
            strSql.Append("pressure=@pressure,");
            strSql.Append("Handle=@Handle,");
            strSql.Append("safety_devices=@safety_devices,");
            strSql.Append("instnt_pressure=@instnt_pressure,");
            strSql.Append("fan_screen=@fan_screen,");
            strSql.Append("drive_motor=@drive_motor,");
            strSql.Append("leakage_oil_gas_water=@leakage_oil_gas_water,");
            strSql.Append("principal_axis=@principal_axis,");
            strSql.Append("appearance=@appearance,");
            strSql.Append("electrical_part=@electrical_part,");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@number",MySqlDbType.String),
                new MySqlParameter("@operator",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@spare_parts",MySqlDbType.Int32),
                new MySqlParameter("@liquid",MySqlDbType.Int32),
                new MySqlParameter("@pressure",MySqlDbType.Int32),
                new MySqlParameter("@Handle",MySqlDbType.Int32),
                new MySqlParameter("@safety_devices",MySqlDbType.Int32),
                new MySqlParameter("@instnt_pressure",MySqlDbType.Int32),
                new MySqlParameter("@fan_screen",MySqlDbType.Int32),
                new MySqlParameter("@drive_motor",MySqlDbType.Int32),
                new MySqlParameter("@leakage_oil_gas_water",MySqlDbType.Int32),
                new MySqlParameter("@principal_axis",MySqlDbType.Int32),
                new MySqlParameter("@appearance",MySqlDbType.Int32),
                new MySqlParameter("@electrical_part",MySqlDbType.Int32),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = checkPoint.Id;
            parameters[1].Value = checkPoint.Number;
            parameters[2].Value = checkPoint.Operator;
            parameters[3].Value = checkPoint.DeviceName;
            parameters[4].Value = checkPoint.DeviceNumber;
            parameters[5].Value = checkPoint.DeviceModel;
            parameters[6].Value = checkPoint.SpareParts;
            parameters[7].Value = checkPoint.Liquid;
            parameters[8].Value = checkPoint.Pressure;
            parameters[9].Value = checkPoint.Handle;
            parameters[10].Value = checkPoint.SafetyDevices;
            parameters[11].Value = checkPoint.InstrumentPressure;
            parameters[12].Value = checkPoint.FanScreen;
            parameters[13].Value = checkPoint.DriveMotor;
            parameters[14].Value = checkPoint.LeakageOilGasWater;
            parameters[15].Value = checkPoint.PrincipalAxis;
            parameters[16].Value = checkPoint.Appearance;
            parameters[17].Value = checkPoint.ElectricalPart;
            parameters[18].Value = checkPoint.UpdateTime;
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
            strSql.Append("DELETE FROM check_point where id = '" + id + "'");
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

                if (dataRow["number"] != null && dataRow["number"].ToString() != "")
                {
                    model.Number = dataRow["number"].ToString();
                }

                if (dataRow["Operator"] != null && dataRow["Operator"].ToString() != "")
                {
                    model.Operator = dataRow["Operator"].ToString();
                }

                if (dataRow["device_name"] != null && dataRow["device_name"].ToString() != "")
                {
                    model.DeviceName = dataRow["device_name"].ToString();
                }

                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DeviceNumber = dataRow["device_number"].ToString();
                }

                if (dataRow["device_model"] != null && dataRow["device_model"].ToString() != "")
                {
                    model.DeviceModel = dataRow["device_model"].ToString();
                }

                if (dataRow["spare_parts"] != null && dataRow["spare_parts"].ToString() != "")
                {
                    model.SpareParts = int.Parse(dataRow["spare_parts"].ToString());
                }

                if (dataRow["liquid"] != null && dataRow["liquid"].ToString() != "")
                {
                    model.Liquid = int.Parse(dataRow["liquid"].ToString());
                }

                if (dataRow["pressure"] != null && dataRow["pressure"].ToString() != "")
                {
                    model.Pressure = int.Parse(dataRow["pressure"].ToString());
                }

                if (dataRow["Handle"] != null && dataRow["Handle"].ToString() != "")
                {
                    model.Handle = int.Parse(dataRow["Handle"].ToString());
                }

                if (dataRow["safety_devices"] != null && dataRow["safety_devices"].ToString() != "")
                {
                    model.SafetyDevices = int.Parse(dataRow["safety_devices"].ToString());
                }


                if (dataRow["instnt_pressure"] != null && dataRow["instnt_pressure"].ToString() != "")
                {
                    model.InstrumentPressure = int.Parse(dataRow["instnt_pressure"].ToString());
                }

                if (dataRow["fan_screen"] != null && dataRow["fan_screen"].ToString() != "")
                {
                    model.FanScreen = int.Parse(dataRow["fan_screen"].ToString());
                }

                if (dataRow["drive_motor"] != null && dataRow["drive_motor"].ToString() != "")
                {
                    model.DriveMotor = int.Parse(dataRow["drive_motor"].ToString());
                }

                if (dataRow["drive_motor"] != null && dataRow["drive_motor"].ToString() != "")
                {
                    model.DriveMotor = int.Parse(dataRow["drive_motor"].ToString());
                }

                if (dataRow["leakage_oil_gas_water"] != null && dataRow["leakage_oil_gas_water"].ToString() != "")
                {
                    model.LeakageOilGasWater = int.Parse(dataRow["leakage_oil_gas_water"].ToString());
                }

                if (dataRow["principal_axis"] != null && dataRow["principal_axis"].ToString() != "")
                {
                    model.PrincipalAxis = int.Parse(dataRow["principal_axis"].ToString());
                }

                if (dataRow["appearance"] != null && dataRow["appearance"].ToString() != "")
                {
                    model.Appearance = int.Parse(dataRow["appearance"].ToString());
                }

                if (dataRow["electrical_part"] != null && dataRow["electrical_part"].ToString() != "")
                {
                    model.ElectricalPart = int.Parse(dataRow["electrical_part"].ToString());
                }

                if (dataRow["create_time"] != null && dataRow["create_time"].ToString() != "")
                {
                    model.CreateTime = (DateTime)dataRow["create_time"];
                }

                if (dataRow["update_time"] != null && dataRow["update_time"].ToString() != "")
                {
                    model.UpdateTime = (DateTime)dataRow["update_time"];
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
            strSql.Append("select count(1) count FROM check_point ");
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
        /// <param name="point"></param>
        /// <returns></returns>
        public CheckPoint SetCheckPoint(CheckPoint point)
        {
            CheckPoint checkPoint = new CheckPoint();
            checkPoint.Id = GetRandomString();
            checkPoint.Number = point.Number;
            checkPoint.Operator = point.Operator;
            checkPoint.DeviceName = point.DeviceName;
            checkPoint.DeviceNumber = point.DeviceNumber;
            checkPoint.DeviceModel = point.DeviceModel;
            checkPoint.SpareParts = point.SpareParts;
            checkPoint.Liquid = point.Liquid;
            checkPoint.Pressure = point.Pressure;
            checkPoint.Handle = point.Handle;
            checkPoint.SafetyDevices = point.SafetyDevices;
            checkPoint.InstrumentPressure = point.InstrumentPressure;
            checkPoint.FanScreen = point.FanScreen;
            checkPoint.DriveMotor = point.DriveMotor;
            checkPoint.LeakageOilGasWater = point.LeakageOilGasWater;
            checkPoint.PrincipalAxis = point.PrincipalAxis;
            checkPoint.Appearance = point.Appearance;
            checkPoint.ElectricalPart = point.ElectricalPart;
            checkPoint.CreateTime = DateTime.Now;
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
            for (int i = 0; i < 32; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        #endregion  BasicMethod
    }
}
