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
        /// <param name="deviceDetails"></param>
        /// <returns></returns>
        public bool Add(DeviceDetail deviceDetails)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into device_detail(").Append("id,device_id,device_name,assets_number,device_photo,create_time,update_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_id,@device_name,@DeviceNumber,@AssetsNumber,@DevicePhoto,@CreateTime,@UpdateTime)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@DeviceNumber",MySqlDbType.String),
                new MySqlParameter("@AssetsNumber",MySqlDbType.String),
                new MySqlParameter("@DevicePhoto",MySqlDbType.Blob),
                new MySqlParameter("@CreateTime",MySqlDbType.DateTime),
                new MySqlParameter("@UpdateTime",MySqlDbType.DateTime) };
            parameters[0].Value = deviceDetails.Id;
            parameters[1].Value = deviceDetails.DeviceId;
            parameters[2].Value = deviceDetails.DeviceName;
            parameters[3].Value = deviceDetails.DeviceNumber;
            parameters[4].Value = deviceDetails.AssetsNumber;
            parameters[5].Value = deviceDetails.DevicePhoto;
            parameters[6].Value = deviceDetails.CreateTime;
            parameters[7].Value = deviceDetails.UpdateTime;
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


        public DeviceDetail GetDeviceDetailById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from device_detail where id = '" + id + "'");
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

                if (dataRow["assets_number"] != null && dataRow["assets_number"].ToString() != "")
                {
                    model.AssetsNumber = dataRow["assets_number"].ToString();
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
            device.DeviceName = dto.DeviceNumber;
            device.AssetsNumber = dto.AssetsNumber;
            device.DevicePhoto = dto.DevicePhoto;
            device.CreateTime = new DateTime();
            device.UpdateTime = dto.UpdateTime;
            return device;
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
