using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class DeviceDetailDAL
    {
        public DeviceDetailDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="deviceDetail"></param>
        /// <returns></returns>
        public bool Add(DeviceDetail deviceDetail)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into device_detail(").Append("id,device_id,device_type,device_name,device_number,acquisition_date,shift,retention_time,running_time,crop_rate,remarks,device_photo,create_time)");
            strSql.Append(" values (@id,@device_id,@device_type,@device_name,@device_number,@acquisition_date,@shift,@retention_time,@running_time,@crop_rate,@remarks,@device_photo,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@acquisition_date",MySqlDbType.DateTime),
                new MySqlParameter("@shift",MySqlDbType.String),
                new MySqlParameter("@retention_time",MySqlDbType.Int32),
                new MySqlParameter("@running_time",MySqlDbType.Int32),
                new MySqlParameter("@crop_rate",MySqlDbType.String),
                new MySqlParameter("@remarks",MySqlDbType.String),
                new MySqlParameter("@device_photo",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = deviceDetail.Id;
            parameters[1].Value = deviceDetail.DeviceId;
            parameters[2].Value = deviceDetail.DeviceType;
            parameters[3].Value = deviceDetail.DeviceName;
            parameters[4].Value = deviceDetail.DeviceNumber;
            parameters[5].Value = deviceDetail.AcquisitionDate;
            parameters[6].Value = deviceDetail.Shift;
            parameters[7].Value = deviceDetail.RetentionTime;
            parameters[8].Value = deviceDetail.RunningTime;
            parameters[9].Value = deviceDetail.CropRate;
            parameters[10].Value = deviceDetail.remarks;
            parameters[11].Value = deviceDetail.DevicePhoto;
            parameters[12].Value = deviceDetail.CreateTime;
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
        /// <param name="deviceDetail"></param>
        /// <returns></returns>
        public bool Update(DeviceDetail deviceDetail)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update device_detail set ");
            strSql.Append(" id=@id,");
            strSql.Append("device_id=@device_id,");
            strSql.Append("device_type=@device_type,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("device_number=@device_number,");
            strSql.Append("acquisition_date=@acquisition_date,");
            strSql.Append("shift=@shift,");
            strSql.Append("retention_time=@retention_time,");
            strSql.Append("running_time=@running_time,");
            strSql.Append("crop_rate=@crop_rate,");
            strSql.Append("remarks=@remarks,");
            strSql.Append("device_photo=@device_photo,");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@acquisition_date",MySqlDbType.DateTime),
                new MySqlParameter("@shift",MySqlDbType.String),
                new MySqlParameter("@retention_time",MySqlDbType.Int32),
                new MySqlParameter("@running_time",MySqlDbType.Int32),
                new MySqlParameter("@crop_rate",MySqlDbType.String),
                new MySqlParameter("@remarks",MySqlDbType.String),
                new MySqlParameter("@device_photo",MySqlDbType.String),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = deviceDetail.Id;
            parameters[1].Value = deviceDetail.DeviceId;
            parameters[2].Value = deviceDetail.DeviceType;
            parameters[3].Value = deviceDetail.DeviceName;
            parameters[4].Value = deviceDetail.DeviceNumber;
            parameters[5].Value = deviceDetail.AcquisitionDate;
            parameters[6].Value = deviceDetail.Shift;
            parameters[7].Value = deviceDetail.RetentionTime;
            parameters[8].Value = deviceDetail.RunningTime;
            parameters[9].Value = deviceDetail.CropRate;
            parameters[10].Value = deviceDetail.remarks;
            parameters[11].Value = deviceDetail.DevicePhoto;
            parameters[12].Value = deviceDetail.UpdateTime;
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
        /// 根据设备ID查询设备数据是否存在
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public bool getOneById(string deviceId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM  device_details WHERE DEVICE_ID = '" + deviceId + "'");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// 获取所有设备信息列表
        /// </summary>
        /// <returns></returns>
        public List<DeviceDetail> getAllDeviceDetail()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from device_detail");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            return DataRowToModelList(dataSet);
        }

        /// <summary>
        /// 根据ID查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DeviceDetail GetDeviceDetailById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from device_detail where device_id = '" + id + "'");
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
        /// 通过ID删除一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean DeleteById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("DELETE FROM device_detail where id = '" + id + "'");
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
        /// 查询列表
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<DeviceDetail> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from device_detail ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DataRowToModelList(DbHelperMySQL.Query(strSql.ToString()));
        }

        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) count FROM device_detail ");
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
        /// 数据转换成对象
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public List<DeviceDetail> DataRowToModelList(DataSet dataSet)
        {
            List<DeviceDetail> DeviceDetailsList = new List<DeviceDetail>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DeviceDetail device = DataRowToModel(dataSet.Tables[0].Rows[i]);
                DeviceDetailsList.Add(device);
            }
            return DeviceDetailsList;
        }


        public DeviceDetail DataRowToModel(DataRow dataRow)
        {
            DeviceDetail model = new DeviceDetail();
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
                if (dataRow["device_name"] != null && dataRow["device_name"].ToString() != "")
                {
                    model.DeviceName = dataRow["device_name"].ToString();
                }

                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DeviceNumber = dataRow["device_number"].ToString();
                }


                if (dataRow["acquisition_date"] != null && dataRow["acquisition_date"].ToString() != "")
                {
                    model.AcquisitionDate = (DateTime)dataRow["acquisition_date"];
                }

                if (dataRow["shift"] != null && dataRow["shift"].ToString() != "")
                {
                    model.Shift = dataRow["shift"].ToString();
                }

                if (dataRow["retention_time"] != null && dataRow["retention_time"].ToString() != "")
                {
                    model.RetentionTime = int.Parse(dataRow["retention_time"].ToString());
                }

                if (dataRow["running_time"] != null && dataRow["running_time"].ToString() != "")
                {
                    model.RunningTime = int.Parse(dataRow["running_time"].ToString());
                }
                if (dataRow["crop_rate"] != null && dataRow["crop_rate"].ToString() != "")
                {
                    model.CropRate = dataRow["crop_rate"].ToString();
                }
                if (dataRow["remarks"] != null && dataRow["remarks"].ToString() != "")
                {
                    model.remarks = dataRow["remarks"].ToString();
                }

                if (dataRow["device_photo"] != null && dataRow["device_photo"].ToString() != "")
                {
                    model.DevicePhoto = dataRow["device_photo"].ToString();
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


        public DeviceDetail SetDeviceDetail(DeviceDetail dto)
        {
            DeviceDetail device = new DeviceDetail();
            device.Id = GetRandomString();
            device.DeviceId = dto.DeviceId;
            device.DeviceName = dto.DeviceName;
            device.DeviceNumber = dto.DeviceNumber;
            device.DeviceType = dto.DeviceType;
            device.AcquisitionDate = dto.AcquisitionDate;
            device.Shift = dto.Shift;
            device.RetentionTime = dto.RetentionTime;
            device.RunningTime = dto.RunningTime;
            device.CropRate = dto.CropRate;
            device.remarks = dto.remarks;
            device.DevicePhoto = dto.DevicePhoto;
            device.CreateTime = DateTime.Now;
            return device;
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
