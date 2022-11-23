using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class LaboratoryDeviceDAL
    {
        public LaboratoryDeviceDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="laboratory"></param>
        /// <returns></returns>
        public bool Add(LaboratoryDevice laboratory)
        {
            StringBuilder strSql = new();
            strSql.Append("insert into laboratory_device_manage(").Append("id,device_id,device_name,device_type,device_number,asset_number,area,device_photo,create_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@device_id,@device_name,@device_type,@device_number,@asset_number,@area,@device_photo,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@asset_number",MySqlDbType.String),
                new MySqlParameter("@area",MySqlDbType.Int32),
                new MySqlParameter("@device_photo",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime) };
            parameters[0].Value = laboratory.Id;
            parameters[1].Value = laboratory.DeviceId;
            parameters[2].Value = laboratory.DeviceName;
            parameters[3].Value = laboratory.DeviceType;
            parameters[4].Value = laboratory.DeviceNumber;
            parameters[5].Value = laboratory.AssetNumber;
            parameters[6].Value = laboratory.Area;
            parameters[7].Value = laboratory.DevicePhoto;
            parameters[8].Value = laboratory.CreateTime;
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
        /// <param name="boxDevice"></param>
        /// <returns></returns>
        public bool Update(LaboratoryDevice boxDevice)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update laboratory_device_manage set ");
            strSql.Append(" id=@id,");
            strSql.Append("device_id=@device_id,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("device_type=@device_type,");
            strSql.Append("device_number=@device_number,");
            strSql.Append("asset_number=@asset_number,");
            strSql.Append("area=@area,");
            strSql.Append("device_photo=@device_photo,");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where Id=@Id");
            MySqlParameter[] parameters =
            {
               new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_type",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@asset_number",MySqlDbType.String),
                new MySqlParameter("@area",MySqlDbType.Int32),
                new MySqlParameter("@device_photo",MySqlDbType.String),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = boxDevice.Id;
            parameters[1].Value = boxDevice.DeviceId;
            parameters[2].Value = boxDevice.DeviceName;
            parameters[3].Value = boxDevice.DeviceType;
            parameters[4].Value = boxDevice.DeviceNumber;
            parameters[5].Value = boxDevice.AssetNumber;
            parameters[6].Value = boxDevice.Area;
            parameters[7].Value = boxDevice.DevicePhoto;
            parameters[8].Value = boxDevice.UpdateTime;
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
        /// 查询数据详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public LaboratoryDevice GetOneById(string Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM LABORATORY_DEVICE_MANAGE WHERE ID = '" + Id + "'");
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
        /// 根据ID查询设备数据是否存在
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool CheckOneById(string Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM  laboratory_device_manage WHERE DEVICE_ID = '" + Id + "'");
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
            strSql.Append("DELETE * FROM laboratory_device_manage");
            int dataSet = DbHelperMySQL.ExecuteSql(strSql.ToString());
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("DELETE FROM laboratory_device_manage where id ='" + id + "'");
            int dataSet = DbHelperMySQL.ExecuteSql(strSql.ToString());
            if (dataSet > 0)
            {
                return true;
            }
            return false;
        }


        public List<LaboratoryDevice> GetAllDeviceList(int pageNo, int pageSize)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from laboratory_device_manage where 1=1 limit " + (pageNo - 1) * pageSize + "," + pageSize);
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            return DataRowToModelList(dataSet);
        }

        public List<Dictionary<string, object>> GetSixAreaDeviceNumber()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT A.AREA_NAME, COUNT(L.AREA) AS NUM FROM LABORATORY_DEVICE_MANAGE L LEFT JOIN AREA_LIST A ON A.AREA_CODE = L.AREA GROUP BY L.AREA");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                DataRow dataRow = dataSet.Tables[0].Rows[i];
                var name = dataRow["AREA_NAME"].ToString();
                var num = dataRow["NUM"].ToString();
                data.Add("name", name);
                data.Add("num", num);
                list.Add(data);
            }
            return list;
        }

        /// <summary>
        /// 按区域查询设备列表
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public List<LaboratoryDevice> GetAreaDeviceListByAraeCode(int areaCode) 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM  laboratory_device_manage WHERE area = '" + areaCode + "'");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            return DataRowToModelList(dataSet);
        }

        /// <summary>
        /// 各状态时间统计
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public List<TemperStatusDuration> GetStatusDuration(string deviceId) 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select runState,state,sum(duration) duration FROM temper_state_total where device_id = '" + deviceId + "' GROUP BY state");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<TemperStatusDuration> list = new List<TemperStatusDuration>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                TemperStatusDuration statusDuration = new TemperStatusDuration();
                DataRow dataRow = dataSet.Tables[0].Rows[i];
                statusDuration.RunState = dataRow["runState"].ToString();
                statusDuration.state = int.Parse(dataRow["state"].ToString());
                statusDuration.duration = double.Parse(dataRow["duration"].ToString());
                list.Add(statusDuration);
            }
            return list;
        }

      
        public List<LaboratoryDevice> DataRowToModelList(DataSet dataSet)
        {
            List<LaboratoryDevice> temperBoxDeviceList = new List<LaboratoryDevice>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                LaboratoryDevice boxDevice = DataRowToModel(dataSet.Tables[0].Rows[i]);
                temperBoxDeviceList.Add(boxDevice);
            }
            return temperBoxDeviceList;
        }


        public LaboratoryDevice DataRowToModel(DataRow dataRow)
        {
            LaboratoryDevice model = new LaboratoryDevice();
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

                if (dataRow["device_type"] != null && dataRow["device_type"].ToString() != "")
                {
                    model.DeviceType = dataRow["device_type"].ToString();
                }

                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DeviceNumber = dataRow["device_number"].ToString();
                }

                if (dataRow["asset_number"] != null && dataRow["asset_number"].ToString() != "")
                {
                    model.AssetNumber = dataRow["asset_number"].ToString();
                }

                if (dataRow["area"] != null && dataRow["area"].ToString() != "")
                {
                    model.Area = int.Parse(dataRow["area"].ToString());
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



        public List<AreaList> GetSixArea() 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from area_list");
            DataSet dataSet = DbHelperMySQL.Query(strSql.ToString());
            List<AreaList> list = new List<AreaList>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                AreaList area = new AreaList();
                DataRow dataRow = dataSet.Tables[0].Rows[i];
                area.Id = dataRow["id"].ToString();
                area.AreaName = dataRow["area_name"].ToString();
                area.AreaCode = int.Parse(dataRow["area_code"].ToString());
                area.CreateTime = (DateTime)dataRow["create_time"];
                area.UpdateTime = (DateTime)dataRow["update_time"];
                list.Add(area);
            }
            return list;
        }



        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) count FROM laboratory_device_manage ");
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

        public LaboratoryDevice SetLaboratoryDevice(LaboratoryDevice box)
        {
            LaboratoryDevice device = new LaboratoryDevice();
            device.Id = GetRandomString();
            //虚拟设备自定义ID
            device.DeviceId = GetRandomString();
            device.DeviceName = box.DeviceName;
            device.DeviceType = box.DeviceType;
            device.DeviceNumber = box.DeviceNumber;
            device.AssetNumber = box.AssetNumber;
            device.Area = box.Area;
            device.CreateTime = DateTime.Now;
            device.DevicePhoto = box.DevicePhoto;
            device.UpdateTime = box.UpdateTime;
            return device;
        }

        public LaboratoryDevice SetLaboratoryDevice(TemperBoxDto box)
        {
            LaboratoryDevice device = new LaboratoryDevice();
            device.Id = GetRandomString();
            device.DeviceId = box.DeviceId;
            device.DeviceName = box.Name+"-"+box.Description;
            device.CreateTime = DateTime.Now;
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
            for (int i = 0; i < 32; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        #endregion  BasicMethod
    }
}
