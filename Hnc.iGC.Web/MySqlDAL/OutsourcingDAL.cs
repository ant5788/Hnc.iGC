using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class OutsourcingDAL
    {
        public OutsourcingDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="outsourcing"></param>
        /// <returns></returns>
        public bool Add(Outsourcing outsourcing)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into outsourcing(").Append("id,Outsourcing_Order,Supplier,Item_Number,Item_Description,Number,Received_Quantity,Qualified_Quantity,To_Be_Inspected,Outsourcing_Time,Actual_Required_Date" +
                ",Contract_Signing_Date,Contract_Arrival_Date,Delivery_Time,Early_Warning,Actual_Arrival_Date,Procurement_Team,Remarks,Create_Time)");
            strSql.Append(" values (");
            strSql.Append("@id,@Outsourcing_Order,@Supplier,@Item_Number,@Item_Description,@Number,@Received_Quantity,@Qualified_Quantity,@To_Be_Inspected,@Outsourcing_Time,@Actual_Required_Date" +
                ",@Contract_Signing_Date,@Contract_Arrival_Date,@Delivery_Time,@Early_Warning,@Actual_Arrival_Date,@Procurement_Team,@Remarks,@Create_Time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@Outsourcing_Order",MySqlDbType.String),
                new MySqlParameter("@Supplier",MySqlDbType.String),
                new MySqlParameter("@Item_Number",MySqlDbType.String),
                new MySqlParameter("@Item_Description",MySqlDbType.String),
                new MySqlParameter("@Number",MySqlDbType.Int32),
                new MySqlParameter("@Received_Quantity",MySqlDbType.Int32),
                new MySqlParameter("@Qualified_Quantity",MySqlDbType.Int32),
                new MySqlParameter("@To_Be_Inspected",MySqlDbType.Int32),
                new MySqlParameter("@Outsourcing_Time",MySqlDbType.DateTime),
                new MySqlParameter("@Actual_Required_Date",MySqlDbType.DateTime),
                new MySqlParameter("@Contract_Signing_Date",MySqlDbType.DateTime),
                new MySqlParameter("@Contract_Arrival_Date",MySqlDbType.DateTime),
                new MySqlParameter("@Delivery_Time",MySqlDbType.DateTime),
                new MySqlParameter("@Early_Warning",MySqlDbType.Int32),
                new MySqlParameter("@Actual_Arrival_Date",MySqlDbType.DateTime),
                new MySqlParameter("@Procurement_Team",MySqlDbType.String),
                new MySqlParameter("@Remarks",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = outsourcing.Id;
            parameters[1].Value = outsourcing.OutsourcingOrder;
            parameters[2].Value = outsourcing.Supplier;
            parameters[3].Value = outsourcing.ItemNumber;
            parameters[4].Value = outsourcing.ItemDescription;
            parameters[5].Value = outsourcing.Number;
            parameters[6].Value = outsourcing.ReceivedQuantity;
            parameters[7].Value = outsourcing.QualifiedQuantity;
            parameters[8].Value = outsourcing.ToBeInspected;
            parameters[9].Value = outsourcing.OutsourcingTime;
            parameters[10].Value = outsourcing.ActualRequiredDate;
            parameters[11].Value = outsourcing.ContractSigningDate;
            parameters[12].Value = outsourcing.ContractArrivalDate;
            parameters[13].Value = outsourcing.DeliveryTime;
            parameters[14].Value = outsourcing.EarlyWarning;
            parameters[15].Value = outsourcing.ActualArrivalDate;
            parameters[16].Value = outsourcing.ProcurementTeam;
            parameters[17].Value = outsourcing.Remarks;
            parameters[18].Value = outsourcing.CreateTime;
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
        /// <param name="outsourcing"></param>
        /// <returns></returns>
        public bool Update(Outsourcing outsourcing)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update outsourcing set ");
            strSql.Append(" id=@id,");
            strSql.Append("Outsourcing_Order=@Outsourcing_Order,");
            strSql.Append("Supplier=@Supplier,");
            strSql.Append("Item_Number=@Item_Number,");
            strSql.Append("Item_Description=@Item_Description,");
            strSql.Append("Number=@Number,");
            strSql.Append("Received_Quantity=@Received_Quantity,");
            strSql.Append("Qualified_Quantity=@Qualified_Quantity,");
            strSql.Append("To_Be_Inspected=@To_Be_Inspected,");
            strSql.Append("Outsourcing_Time=@Outsourcing_Time,");
            strSql.Append("Actual_Required_Date=@Actual_Required_Date,");
            strSql.Append("Contract_Signing_Date=@Contract_Signing_Date,");
            strSql.Append("Contract_Arrival_Date=@Contract_Arrival_Date,");
            strSql.Append("Delivery_Time=@Delivery_Time,");
            strSql.Append("Early_Warning=@Early_Warning,");
            strSql.Append("Actual_Arrival_Date=@Actual_Arrival_Date,");
            strSql.Append("Procurement_Team=@Procurement_Team,");
            strSql.Append("Remarks=@Remarks,");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@Outsourcing_Order",MySqlDbType.String),
                new MySqlParameter("@Supplier",MySqlDbType.String),
                new MySqlParameter("@Item_Number",MySqlDbType.String),
                new MySqlParameter("@Item_Description",MySqlDbType.String),
                new MySqlParameter("@Number",MySqlDbType.Int32),
                new MySqlParameter("@Received_Quantity",MySqlDbType.Int32),
                new MySqlParameter("@Qualified_Quantity",MySqlDbType.Int32),
                new MySqlParameter("@To_Be_Inspected",MySqlDbType.Int32),
                new MySqlParameter("@Outsourcing_Time",MySqlDbType.DateTime),
                new MySqlParameter("@Actual_Required_Date",MySqlDbType.DateTime),
                new MySqlParameter("@Contract_Signing_Date",MySqlDbType.DateTime),
                new MySqlParameter("@Contract_Arrival_Date",MySqlDbType.DateTime),
                new MySqlParameter("@Delivery_Time",MySqlDbType.DateTime),
                new MySqlParameter("@Early_Warning",MySqlDbType.Int32),
                new MySqlParameter("@Actual_Arrival_Date",MySqlDbType.DateTime),
                new MySqlParameter("@Procurement_Team",MySqlDbType.String),
                new MySqlParameter("@Remarks",MySqlDbType.String),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = outsourcing.Id;
            parameters[1].Value = outsourcing.OutsourcingOrder;
            parameters[2].Value = outsourcing.Supplier;
            parameters[3].Value = outsourcing.ItemNumber;
            parameters[4].Value = outsourcing.ItemDescription;
            parameters[5].Value = outsourcing.Number;
            parameters[6].Value = outsourcing.ReceivedQuantity;
            parameters[7].Value = outsourcing.QualifiedQuantity;
            parameters[8].Value = outsourcing.ToBeInspected;
            parameters[9].Value = outsourcing.OutsourcingTime;
            parameters[10].Value = outsourcing.ActualRequiredDate;
            parameters[11].Value = outsourcing.ContractSigningDate;
            parameters[12].Value = outsourcing.ContractArrivalDate;
            parameters[13].Value = outsourcing.DeliveryTime;
            parameters[14].Value = outsourcing.EarlyWarning;
            parameters[15].Value = outsourcing.ActualArrivalDate;
            parameters[16].Value = outsourcing.ProcurementTeam;
            parameters[17].Value = outsourcing.Remarks;
            parameters[18].Value = outsourcing.UpdateTime;
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
        public List<Outsourcing> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from outsourcing ");
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
            strSql.Append("DELETE FROM outsourcing where id = '" + id+"'");
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
        public Outsourcing GetById(string id) 
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM outsourcing where id = '" + id + "'");
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
        public Outsourcing DataRowToModel(DataRow dataRow)
        {
            Outsourcing model = new Outsourcing();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = dataRow["id"].ToString();
                }

                if (dataRow["Outsourcing_Order"] != null && dataRow["Outsourcing_Order"].ToString() != "")
                {
                    model.OutsourcingOrder = dataRow["Outsourcing_Order"].ToString();
                }

                if (dataRow["Supplier"] != null && dataRow["Supplier"].ToString() != "")
                {
                    model.Supplier = dataRow["Supplier"].ToString();
                }

                if (dataRow["Item_Number"] != null && dataRow["Item_Number"].ToString() != "")
                {
                    model.ItemNumber = dataRow["Item_Number"].ToString();
                }
                if (dataRow["Item_Description"] != null && dataRow["Item_Description"].ToString() != "")
                {
                    model.ItemDescription = dataRow["Item_Description"].ToString();
                }
                if (dataRow["Number"] != null && dataRow["Number"].ToString() != "")
                {
                    model.Number = int.Parse(dataRow["Number"].ToString());
                }

                if (dataRow["Received_Quantity"] != null && dataRow["Received_Quantity"].ToString() != "")
                {
                    model.ReceivedQuantity = int.Parse(dataRow["Received_Quantity"].ToString());
                }

                if (dataRow["Qualified_Quantity"] != null && dataRow["Qualified_Quantity"].ToString() != "")
                {
                    model.QualifiedQuantity = int.Parse(dataRow["Qualified_Quantity"].ToString());
                }

                if (dataRow["To_Be_Inspected"] != null && dataRow["To_Be_Inspected"].ToString() != "")
                {
                    model.ToBeInspected = int.Parse(dataRow["To_Be_Inspected"].ToString());
                }

                if (dataRow["Outsourcing_Time"] != null && dataRow["Outsourcing_Time"].ToString() != "")
                {
                    model.OutsourcingTime = (DateTime)dataRow["Outsourcing_Time"];
                }

                if (dataRow["Actual_Required_Date"] != null && dataRow["Actual_Required_Date"].ToString() != "")
                {
                    model.ActualRequiredDate = (DateTime)dataRow["Actual_Required_Date"];
                }

                if (dataRow["Contract_Signing_Date"] != null && dataRow["Contract_Signing_Date"].ToString() != "")
                {
                    model.ContractSigningDate = (DateTime)dataRow["Contract_Signing_Date"];
                }

                if (dataRow["Contract_Arrival_Date"] != null && dataRow["Contract_Arrival_Date"].ToString() != "")
                {
                    model.ContractArrivalDate = (DateTime)dataRow["Contract_Arrival_Date"];
                }

                if (dataRow["Delivery_Time"] != null && dataRow["Delivery_Time"].ToString() != "")
                {
                    model.DeliveryTime = (DateTime)dataRow["Delivery_Time"];
                }

                if (dataRow["Early_Warning"] != null && dataRow["Early_Warning"].ToString() != "")
                {
                    model.EarlyWarning = int.Parse(dataRow["Early_Warning"].ToString());
                }

                if (dataRow["Actual_Arrival_Date"] != null && dataRow["Actual_Arrival_Date"].ToString() != "")
                {
                    model.ActualArrivalDate = (DateTime)dataRow["Actual_Arrival_Date"];
                }

                if (dataRow["Procurement_Team"] != null && dataRow["Procurement_Team"].ToString() != "")
                {
                    model.ProcurementTeam = dataRow["Procurement_Team"].ToString();
                }

                if (dataRow["Remarks"] != null && dataRow["Remarks"].ToString() != "")
                {
                    model.Remarks = dataRow["Remarks"].ToString();
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
        public List<Outsourcing> DataRowToModelList(DataSet dataSet)
        {
            List<Outsourcing> outsourcingList = new List<Outsourcing>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                Outsourcing outsourcing = DataRowToModel(dataSet.Tables[0].Rows[i]);
                outsourcingList.Add(outsourcing);
            }
            return outsourcingList;
        }

        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) count FROM outsourcing ");
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
        /// <param name="outs"></param>
        /// <returns></returns>
        public Outsourcing SetOutsourcing(Outsourcing outs)
        {
            Outsourcing outsourcing = new Outsourcing();
            outsourcing.Id = GetRandomString();
            outsourcing.OutsourcingOrder = outs.OutsourcingOrder;
            outsourcing.Supplier = outs.Supplier;
            outsourcing.ItemNumber = outs.ItemNumber;
            outsourcing.ItemDescription = outs.ItemDescription;
            outsourcing.Number = outs.Number;
            outsourcing.ReceivedQuantity = outs.ReceivedQuantity;
            outsourcing.QualifiedQuantity = outs.QualifiedQuantity;
            outsourcing.ToBeInspected = outs.ToBeInspected;
            outsourcing.OutsourcingTime = outs.OutsourcingTime;
            outsourcing.ActualRequiredDate = outs.ActualRequiredDate;
            outsourcing.ContractSigningDate = outs.ContractSigningDate;
            outsourcing.ContractArrivalDate = outs.ContractArrivalDate;
            outsourcing.DeliveryTime = outs.DeliveryTime;
            outsourcing.EarlyWarning = outs.EarlyWarning;
            outsourcing.ActualArrivalDate = outs.ActualArrivalDate;
            outsourcing.ProcurementTeam = outs.ProcurementTeam;
            outsourcing.Remarks = outs.Remarks;
            outsourcing.CreateTime = DateTime.Now;
            outsourcing.UpdateTime = outs.UpdateTime;
            return outsourcing;
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
