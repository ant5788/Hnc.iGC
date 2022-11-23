using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class PassRateDAL
    {
        public PassRateDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="passRate"></param>
        /// <returns></returns>
        public bool Add(PassRate passRate)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into pass_rate(").Append("id,Order_No,Part_Order,Part_Name,Inspection_Quantity," +
                "Unqualified_Quantity,Problem,Working_Procedure,Cause1,Cause2,Person_Liable,Solutions,Inspection_Date,Inspector,remarks,Create_Time)");
            strSql.Append(" values (");
            strSql.Append("@id,@Order_No,@Part_Order,@Part_Name,@Inspection_Quantity," +
                "@Unqualified_Quantity,@Problem,@Working_Procedure,@Cause1,@Cause2,@Person_Liable,@Solutions,@Inspection_Date,@Inspector,@remarks,@Create_Time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@Order_No",MySqlDbType.String),
                new MySqlParameter("@Part_Order",MySqlDbType.String),
                new MySqlParameter("@Part_Name",MySqlDbType.String),
                new MySqlParameter("@Inspection_Quantity",MySqlDbType.Int32),
                new MySqlParameter("@Unqualified_Quantity",MySqlDbType.Int32),
                new MySqlParameter("@Problem",MySqlDbType.String),
                new MySqlParameter("@Working_Procedure",MySqlDbType.String),
                new MySqlParameter("@Cause1",MySqlDbType.String),
                new MySqlParameter("@Cause2",MySqlDbType.String),
                new MySqlParameter("@Person_Liable",MySqlDbType.String),
                new MySqlParameter("@Solutions",MySqlDbType.String),
                new MySqlParameter("@InspectionDate",MySqlDbType.DateTime),
                new MySqlParameter("@Inspector",MySqlDbType.String),
                new MySqlParameter("@remarks",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime)};
            parameters[0].Value = passRate.Id;
            parameters[1].Value = passRate.OrderNo;
            parameters[2].Value = passRate.PartOrder;
            parameters[3].Value = passRate.PartName;
            parameters[4].Value = passRate.InspectionQuantity;
            parameters[5].Value = passRate.UnqualifiedQuantity;
            parameters[6].Value = passRate.Problem;
            parameters[7].Value = passRate.WorkingProcedure;
            parameters[8].Value = passRate.Cause1;
            parameters[9].Value = passRate.Cause2;
            parameters[10].Value = passRate.PersonLiable;
            parameters[11].Value = passRate.Solutions;
            parameters[12].Value = passRate.InspectionDate;
            parameters[13].Value = passRate.Inspector;
            parameters[14].Value = passRate.remarks;
            parameters[15].Value = passRate.CreateTime;
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
        /// <param name="passRate"></param>
        /// <returns></returns>
        public bool Update(PassRate passRate)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update pass_rate set ");
            strSql.Append(" id=@id,");
            strSql.Append("Order_No=@Order_No,");
            strSql.Append("Part_Order=@Part_Order,");
            strSql.Append("Part_Name=@Part_Name,");
            strSql.Append("Inspection_Quantity=@Inspection_Quantity,");
            strSql.Append("Unqualified_Quantity=@Unqualified_Quantity,");
            strSql.Append("Problem=@Problem,");
            strSql.Append("Working_Procedure=@Working_Procedure,");
            strSql.Append("Cause1=@Cause1,");
            strSql.Append("Cause2=@Cause2,");
            strSql.Append("Person_Liable=@Person_Liable,");
            strSql.Append("Solutions=@Solutions,");
            strSql.Append("Inspection_Date=@Inspection_Date,");
            strSql.Append("Inspector=@Inspector,");
            strSql.Append("remarks=@remarks,");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@Order_No",MySqlDbType.String),
                new MySqlParameter("@Part_Order",MySqlDbType.String),
                new MySqlParameter("@Part_Name",MySqlDbType.String),
                new MySqlParameter("@Inspection_Quantity",MySqlDbType.Int32),
                new MySqlParameter("@Unqualified_Quantity",MySqlDbType.Int32),
                new MySqlParameter("@Problem",MySqlDbType.String),
                new MySqlParameter("@Working_Procedure",MySqlDbType.String),
                new MySqlParameter("@Cause1",MySqlDbType.String),
                new MySqlParameter("@Cause2",MySqlDbType.String),
                new MySqlParameter("@Person_Liable",MySqlDbType.String),
                new MySqlParameter("@Solutions",MySqlDbType.String),
                new MySqlParameter("@InspectionDate",MySqlDbType.DateTime),
                new MySqlParameter("@Inspector",MySqlDbType.String),
                new MySqlParameter("@remarks",MySqlDbType.String),
                new MySqlParameter("@update_Time",MySqlDbType.DateTime)};
            parameters[0].Value = passRate.Id;
            parameters[1].Value = passRate.OrderNo;
            parameters[2].Value = passRate.PartOrder;
            parameters[3].Value = passRate.PartName;
            parameters[4].Value = passRate.InspectionQuantity;
            parameters[5].Value = passRate.UnqualifiedQuantity;
            parameters[6].Value = passRate.Problem;
            parameters[7].Value = passRate.WorkingProcedure;
            parameters[8].Value = passRate.Cause1;
            parameters[9].Value = passRate.Cause2;
            parameters[10].Value = passRate.PersonLiable;
            parameters[11].Value = passRate.Solutions;
            parameters[12].Value = passRate.InspectionDate;
            parameters[13].Value = passRate.Inspector;
            parameters[14].Value = passRate.remarks;
            parameters[15].Value = passRate.UpdateTime;
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
        public List<PassRate> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from pass_rate ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            Console.WriteLine(strSql.ToString());
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
            strSql.Append("DELETE FROM pass_rate where id = '" + id + "'");
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
        public PassRate GetById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM pass_rate where id = '" + id + "'");
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
        public PassRate DataRowToModel(DataRow dataRow)
        {
            PassRate model = new PassRate();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = dataRow["ID"].ToString();
                }
                if (dataRow["Order_No"] != null && dataRow["Order_No"].ToString() != "")
                {
                    model.OrderNo = dataRow["Order_No"].ToString();
                }

                if (dataRow["Part_Order"] != null && dataRow["Part_Order"].ToString() != "")
                {
                    model.PartOrder = dataRow["Part_Order"].ToString();
                }
                if (dataRow["Part_Name"] != null && dataRow["Part_Name"].ToString() != "")
                {
                    model.PartName = dataRow["Part_Name"].ToString();
                }
                if (dataRow["Inspection_Quantity"] != null && dataRow["Inspection_Quantity"].ToString() != "")
                {
                    model.InspectionQuantity = Int32.Parse(dataRow["Inspection_Quantity"].ToString());
                }
                if (dataRow["Unqualified_Quantity"] != null && dataRow["Unqualified_Quantity"].ToString() != "")
                {
                    model.UnqualifiedQuantity = Int32.Parse(dataRow["Unqualified_Quantity"].ToString());
                }

                if (dataRow["Problem"] != null && dataRow["Problem"].ToString() != "")
                {
                    model.Problem = dataRow["Problem"].ToString();
                }

                if (dataRow["Working_Procedure"] != null && dataRow["Working_Procedure"].ToString() != "")
                {
                    model.WorkingProcedure = dataRow["Working_Procedure"].ToString();
                }

                if (dataRow["Cause1"] != null && dataRow["Cause1"].ToString() != "")
                {
                    model.Cause1 = dataRow["Cause1"].ToString();
                }

                if (dataRow["Cause2"] != null && dataRow["Cause2"].ToString() != "")
                {
                    model.Cause2 = dataRow["Cause2"].ToString();
                }

                if (dataRow["Person_Liable"] != null && dataRow["Person_Liable"].ToString() != "")
                {
                    model.PersonLiable = dataRow["Person_Liable"].ToString();
                }
                if (dataRow["Solutions"] != null && dataRow["Solutions"].ToString() != "")
                {
                    model.Solutions = dataRow["Solutions"].ToString();
                }
                if (dataRow["Inspection_Date"] != null && dataRow["Inspection_Date"].ToString() != "")
                {
                    model.InspectionDate = (DateTime)dataRow["Inspection_Date"];
                }
                if (dataRow["Inspector"] != null && dataRow["Inspector"].ToString() != "")
                {
                    model.Inspector = dataRow["Inspector"].ToString();
                }

                if (dataRow["remarks"] != null && dataRow["remarks"].ToString() != "")
                {
                    model.remarks = dataRow["remarks"].ToString();
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
        public List<PassRate> DataRowToModelList(DataSet dataSet)
        {
            List<PassRate> passRateList = new List<PassRate>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                PassRate passRate = DataRowToModel(dataSet.Tables[0].Rows[i]);
                passRateList.Add(passRate);
            }
            return passRateList;
        }

        /// <summary>
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) count FROM pass_rate ");
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
        /// <param name="pass"></param>
        /// <returns></returns>
        public PassRate SetPassRate(PassRate pass)
        {
            PassRate passRate = new PassRate();
            passRate.Id = GetRandomString();
            passRate.OrderNo = pass.OrderNo;
            passRate.PartOrder = pass.PartOrder;
            passRate.PartName = pass.PartName;
            passRate.InspectionQuantity = pass.InspectionQuantity;
            passRate.UnqualifiedQuantity = pass.UnqualifiedQuantity;
            passRate.Problem = pass.Problem;
            passRate.WorkingProcedure = pass.WorkingProcedure;
            passRate.Cause1 = pass.Cause1;
            passRate.Cause2 = pass.Cause2;
            passRate.PersonLiable = pass.PersonLiable;
            passRate.Solutions = pass.Solutions;
            passRate.InspectionDate = pass.InspectionDate;
            passRate.Inspector = pass.Inspector;
            passRate.remarks = pass.remarks;
            passRate.UpdateTime = pass.UpdateTime;
            passRate.CreateTime = DateTime.Now;
            return passRate;
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
