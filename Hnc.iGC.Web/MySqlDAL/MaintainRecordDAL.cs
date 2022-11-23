using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class MaintainRecordDAL
    {
        public MaintainRecordDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool Add(MaintainRecord record)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into maintain_record(").Append("Id,device_name,device_number,device_model,person_in_charge,maintain_category,cycle," +
                "content,external_maintenance,axis_cutters,knife_handle_holster,liquid,Coolant,air_gun,computer,electric_control,toolbox," +
                "Maintainer,maintan_time,confirm,confirm_people,confirm_time,create_time)");
            strSql.Append(" values (@Id,@device_name,@device_number,@device_model,@person_in_charge,@maintain_category,@cycle," +
                "@content,@external_maintenance,@axis_cutters,@knife_handle_holster,@liquid,@Coolant,@air_gun,@computer,@electric_control,@toolbox," +
                "@Maintainer,@maintan_time,@confirm,@confirm_people,@confirm_time,@create_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@person_in_charge",MySqlDbType.String),
                new MySqlParameter("@maintain_category",MySqlDbType.String),
                new MySqlParameter("@cycle",MySqlDbType.String),
                new MySqlParameter("@content",MySqlDbType.String),
                new MySqlParameter("@external_maintenance",MySqlDbType.Int32),
                new MySqlParameter("@axis_cutters",MySqlDbType.Int32),
                new MySqlParameter("@knife_handle_holster",MySqlDbType.Int32),
                new MySqlParameter("@liquid",MySqlDbType.Int32),
                new MySqlParameter("@Coolant",MySqlDbType.Int32),
                new MySqlParameter("@air_gun",MySqlDbType.Int32),
                new MySqlParameter("@computer",MySqlDbType.Int32),
                new MySqlParameter("@electric_control",MySqlDbType.Int32),
                new MySqlParameter("@toolbox",MySqlDbType.Int32),
                new MySqlParameter("@Maintainer",MySqlDbType.String),
                new MySqlParameter("@maintan_time",MySqlDbType.DateTime),
                new MySqlParameter("@confirm",MySqlDbType.String),
                new MySqlParameter("@confirm_people",MySqlDbType.String),
                new MySqlParameter("@confirm_time",MySqlDbType.DateTime),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = record.Id;
            parameters[1].Value = record.DeviceName;
            parameters[2].Value = record.DeviceNumber;
            parameters[3].Value = record.DeviceModel;
            parameters[4].Value = record.PersonInCharge;
            parameters[5].Value = record.MaintainCategory;
            parameters[6].Value = record.Cycle;
            parameters[7].Value = record.Content;
            parameters[8].Value = record.ExternalMaintenance;
            parameters[9].Value = record.AxisCutters;
            parameters[10].Value = record.KnifeHandleHolster;
            parameters[11].Value = record.Liquid;
            parameters[12].Value = record.Coolant;
            parameters[13].Value = record.AirGun;
            parameters[14].Value = record.Computer;
            parameters[15].Value = record.ElectricControl;
            parameters[16].Value = record.ToolBox;
            parameters[17].Value = record.Maintainer;
            parameters[18].Value = record.MaintanTime;
            parameters[19].Value = record.Confirm;
            parameters[20].Value = record.ConfirmPeople;
            parameters[21].Value = record.ConfirmTime;
            parameters[22].Value = record.CreateTime;
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
        /// <param name="record"></param>
        /// <returns></returns>
        public bool Update(MaintainRecord record)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update maintain_record set ");
            strSql.Append(" id=@id,");
            strSql.Append("device_name=@device_name,");
            strSql.Append("device_number=@device_number,");
            strSql.Append("device_model=@device_model,");
            strSql.Append("person_in_charge=@person_in_charge,");
            strSql.Append("maintain_category=@maintain_category,");
            strSql.Append("cycle=@cycle,");
            strSql.Append("content=@content,");
            strSql.Append("external_maintenance=@external_maintenance,");
            strSql.Append("axis_cutters=@axis_cutters,");
            strSql.Append("knife_handle_holster=@knife_handle_holster,");
            strSql.Append("liquid=@liquid,");
            strSql.Append("Coolant=@Coolant,");
            strSql.Append("air_gun=@air_gun,");
            strSql.Append("computer=@computer,");
            strSql.Append("electric_control=@electric_control,");
            strSql.Append("toolbox=@toolbox,");
            strSql.Append("Maintainer=@Maintainer,");
            strSql.Append("maintan_time=@maintan_time,");
            strSql.Append("confirm=@confirm,");
            strSql.Append("confirm_people=@confirm_people,");
            strSql.Append("confirm_time=@confirm_time,");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@device_name",MySqlDbType.String),
                new MySqlParameter("@device_number",MySqlDbType.String),
                new MySqlParameter("@device_model",MySqlDbType.String),
                new MySqlParameter("@person_in_charge",MySqlDbType.String),
                new MySqlParameter("@maintain_category",MySqlDbType.String),
                new MySqlParameter("@cycle",MySqlDbType.String),
                new MySqlParameter("@content",MySqlDbType.String),
                new MySqlParameter("@external_maintenance",MySqlDbType.Int32),
                new MySqlParameter("@axis_cutters",MySqlDbType.Int32),
                new MySqlParameter("@knife_handle_holster",MySqlDbType.Int32),
                new MySqlParameter("@liquid",MySqlDbType.Int32),
                new MySqlParameter("@Coolant",MySqlDbType.Int32),
                new MySqlParameter("@air_gun",MySqlDbType.Int32),
                new MySqlParameter("@computer",MySqlDbType.Int32),
                new MySqlParameter("@electric_control",MySqlDbType.Int32),
                new MySqlParameter("@toolbox",MySqlDbType.Int32),
                new MySqlParameter("@Maintainer",MySqlDbType.String),
                new MySqlParameter("@maintan_time",MySqlDbType.DateTime),
                new MySqlParameter("@confirm",MySqlDbType.String),
                new MySqlParameter("@confirm_people",MySqlDbType.String),
                new MySqlParameter("@confirm_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = record.Id;
            parameters[1].Value = record.DeviceName;
            parameters[2].Value = record.DeviceNumber;
            parameters[3].Value = record.DeviceModel;
            parameters[4].Value = record.PersonInCharge;
            parameters[5].Value = record.MaintainCategory;
            parameters[6].Value = record.Cycle;
            parameters[7].Value = record.Content;
            parameters[8].Value = record.ExternalMaintenance;
            parameters[9].Value = record.AxisCutters;
            parameters[10].Value = record.KnifeHandleHolster;
            parameters[11].Value = record.Liquid;
            parameters[12].Value = record.Coolant;
            parameters[13].Value = record.AirGun;
            parameters[14].Value = record.Computer;
            parameters[15].Value = record.ElectricControl;
            parameters[16].Value = record.ToolBox;
            parameters[17].Value = record.Maintainer;
            parameters[18].Value = record.MaintanTime;
            parameters[19].Value = record.Confirm;
            parameters[20].Value = record.ConfirmPeople;
            parameters[21].Value = record.ConfirmTime;
            parameters[22].Value = record.UpdateTime;
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
        public List<MaintainRecord> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from maintain_record ");
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
            strSql.Append("DELETE FROM maintain_record where id = '" + id + "'");
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
        public MaintainRecord GetById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM maintain_record where id = '" + id + "'");
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
        public MaintainRecord DataRowToModel(DataRow dataRow)
        {
            MaintainRecord model = new MaintainRecord();
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

                if (dataRow["device_number"] != null && dataRow["device_number"].ToString() != "")
                {
                    model.DeviceNumber = dataRow["device_number"].ToString();
                }

                if (dataRow["device_model"] != null && dataRow["device_model"].ToString() != "")
                {
                    model.DeviceModel = dataRow["device_model"].ToString();
                }

                if (dataRow["person_in_charge"] != null && dataRow["person_in_charge"].ToString() != "")
                {
                    model.PersonInCharge = dataRow["person_in_charge"].ToString();
                }

                if (dataRow["maintain_category"] != null && dataRow["maintain_category"].ToString() != "")
                {
                    model.MaintainCategory = dataRow["maintain_category"].ToString();
                }

                if (dataRow["cycle"] != null && dataRow["cycle"].ToString() != "")
                {
                    model.Cycle = dataRow["cycle"].ToString().ToString();
                }

                if (dataRow["content"] != null && dataRow["content"].ToString() != "")
                {
                    model.Content = dataRow["content"].ToString();
                }

                if (dataRow["external_maintenance"] != null && dataRow["external_maintenance"].ToString() != "")
                {
                    model.ExternalMaintenance = int.Parse(dataRow["external_maintenance"].ToString());
                }

                if (dataRow["axis_cutters"] != null && dataRow["axis_cutters"].ToString() != "")
                {
                    model.AxisCutters = int.Parse(dataRow["axis_cutters"].ToString());
                }

                if (dataRow["knife_handle_holster"] != null && dataRow["knife_handle_holster"].ToString() != "")
                {
                    model.KnifeHandleHolster = int.Parse(dataRow["knife_handle_holster"].ToString());
                }


                if (dataRow["liquid"] != null && dataRow["liquid"].ToString() != "")
                {
                    model.Liquid = int.Parse(dataRow["liquid"].ToString());
                }

                if (dataRow["Coolant"] != null && dataRow["Coolant"].ToString() != "")
                {
                    model.Coolant = int.Parse(dataRow["Coolant"].ToString());
                }

                if (dataRow["air_gun"] != null && dataRow["air_gun"].ToString() != "")
                {
                    model.AirGun = int.Parse(dataRow["air_gun"].ToString());
                }

                if (dataRow["computer"] != null && dataRow["computer"].ToString() != "")
                {
                    model.Computer = int.Parse(dataRow["computer"].ToString());
                }

                if (dataRow["electric_control"] != null && dataRow["electric_control"].ToString() != "")
                {
                    model.ElectricControl = int.Parse(dataRow["electric_control"].ToString());
                }

                if (dataRow["toolbox"] != null && dataRow["toolbox"].ToString() != "")
                {
                    model.ToolBox = int.Parse(dataRow["toolbox"].ToString());
                }

                if (dataRow["Maintainer"] != null && dataRow["Maintainer"].ToString() != "")
                {
                    model.Maintainer = dataRow["Maintainer"].ToString();
                }

                if (dataRow["maintan_time"] != null && dataRow["maintan_time"].ToString() != "")
                {
                    model.MaintanTime = (DateTime)dataRow["maintan_time"];
                }

                if (dataRow["confirm"] != null && dataRow["confirm"].ToString() != "")
                {
                    model.Confirm = dataRow["confirm"].ToString();
                }

               
                if (dataRow["confirm_people"] != null && dataRow["confirm_people"].ToString() != "")
                {
                    model.ConfirmPeople = dataRow["confirm_people"].ToString();
                }
                if (dataRow["confirm_time"] != null && dataRow["confirm_time"].ToString() != "")
                {
                    model.ConfirmTime = (DateTime)dataRow["confirm_time"];
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
        public List<MaintainRecord> DataRowToModelList(DataSet dataSet)
        {
            List<MaintainRecord> maintainRecordList = new List<MaintainRecord>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                MaintainRecord maintainRecord = DataRowToModel(dataSet.Tables[0].Rows[i]);
                maintainRecordList.Add(maintainRecord);
            }
            return maintainRecordList;
        }

        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) count FROM maintain_record ");
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
        public MaintainRecord SetMaintainRecord(MaintainRecord record)
        {
            MaintainRecord maintainRecord = new MaintainRecord();
            maintainRecord.Id = GetRandomString();
            maintainRecord.DeviceName = record.DeviceName;
            maintainRecord.DeviceNumber = record.DeviceNumber;
            maintainRecord.DeviceModel = record.DeviceModel;
            maintainRecord.PersonInCharge = record.PersonInCharge;
            maintainRecord.MaintainCategory = record.MaintainCategory;
            maintainRecord.Cycle = record.Cycle;
            maintainRecord.Content = record.Content;
            maintainRecord.ExternalMaintenance = record.ExternalMaintenance;
            maintainRecord.AxisCutters = record.AxisCutters;
            maintainRecord.KnifeHandleHolster = record.KnifeHandleHolster;
            maintainRecord.Liquid = record.Liquid;
            maintainRecord.Coolant = record.Coolant;
            maintainRecord.AirGun = record.AirGun;
            maintainRecord.Computer = record.Computer;
            maintainRecord.ElectricControl = record.ElectricControl;
            maintainRecord.ToolBox = record.ToolBox;
            maintainRecord.Maintainer = record.Maintainer;
            maintainRecord.MaintanTime = record.MaintanTime;
            maintainRecord.Confirm = record.Confirm;
            maintainRecord.ConfirmPeople = record.ConfirmPeople;
            maintainRecord.ConfirmTime = record.ConfirmTime;
            maintainRecord.CreateTime = DateTime.Now;
            return maintainRecord;
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
