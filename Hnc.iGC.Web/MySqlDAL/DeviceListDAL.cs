using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class DeviceListDAL
    {
        public DeviceListDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="deviceList"></param>
        /// <returns></returns>
        public bool Add(DeviceList deviceList)
        {
            StringBuilder strSql = new();
            strSql.Append("insert into device_list(").Append("id,device_id,device_name,Description,IP,port)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_id,@device_name,@Description,@IP,@port)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@Description",MySqlDbType.String),
                new MySqlParameter("@IP",MySqlDbType.String),
                new MySqlParameter("@port",MySqlDbType.String) };
            parameters[0].Value = deviceList.Id;
            parameters[1].Value = deviceList.DeviceId;
            parameters[2].Value = deviceList.DeviceName;
            parameters[3].Value = deviceList.Description;
            parameters[4].Value = deviceList.Ip;
            parameters[5].Value = deviceList.Port;
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
            strSql.Append("SELECT * FROM  DEVICE_LIST WHERE DEVICE_ID = '" + deviceId + "'");
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
        /// 全部删除设备列表数据
        /// </summary>
        public void DeleteAll() 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("DELETE * FROM device_list");
            int dataSet = DbHelperMySQL.ExecuteSql(strSql.ToString());
        }



        public List<DeviceList> getAllDevice()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from device_list");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            return DataRowToModelList(dataSet);
        }


        public List<DeviceList> DataRowToModelList(DataSet dataSet)
        {
            List<DeviceList> deviceLists = new List<DeviceList>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DeviceList device = DataRowToModel(dataSet.Tables[0].Rows[i]);
                deviceLists.Add(device);
            }
            return deviceLists;
        }


        public DeviceList DataRowToModel(DataRow dataRow)
        {
            DeviceList model = new DeviceList();
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
                if (dataRow["Description"] != null && dataRow["Description"].ToString() != "")
                {
                    model.Description = dataRow["Description"].ToString();
                }

                if (dataRow["IP"] != null && dataRow["IP"].ToString() != "")
                {
                    model.Ip = dataRow["IP"].ToString();
                }

                if (dataRow["port"] != null && dataRow["port"].ToString() != "")
                {
                    model.Port = dataRow["port"].ToString();
                }
            }
            return model;
        }


        public DeviceList SetDeviceList(CNCDto dto)
        {
            DeviceList device = new DeviceList();
            device.Id = GetRandomString();
            device.DeviceId = dto.DeviceId;
            device.DeviceName = dto.Name;
            device.Description = dto.Description;
            device.Ip = dto.IP;
            device.Port = Convert.ToString(dto.Port);
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
