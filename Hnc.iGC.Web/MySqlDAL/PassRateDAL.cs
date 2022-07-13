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
            strSql.Append("insert into pass_rate(").Append("id,number,name,date,feeding_number, waste_number,pass_rate,create_time,update_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@number,@name,@date,@feeding_number, @waste_number,@pass_rate,@create_time,@update_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@number",MySqlDbType.String),
                new MySqlParameter("@name",MySqlDbType.String),
                new MySqlParameter("@date",MySqlDbType.DateTime),
                new MySqlParameter("@feeding_number",MySqlDbType.Int32),
                new MySqlParameter("@waste_number",MySqlDbType.Int32),
                new MySqlParameter("@pass_rate",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = passRate.Id;
            parameters[1].Value = passRate.Number;
            parameters[2].Value = passRate.Name;
            parameters[3].Value = passRate.Date;
            parameters[4].Value = passRate.FeedingNumber;
            parameters[5].Value = passRate.WasteNumber;
            parameters[6].Value = passRate.Pass_Rate;
            parameters[10].Value = passRate.CreateTime;
            parameters[11].Value = passRate.UpdateTime;
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
            strSql.Append("number=@number,");
            strSql.Append("name=@name,");
            strSql.Append("date=@date,");
            strSql.Append("type=@type,");
            strSql.Append("feeding_number=@feeding_number,");
            strSql.Append("pass_rate=@pass_rate");
            strSql.Append("create_time=@create_time");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters =
            {
               new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@number",MySqlDbType.String),
                new MySqlParameter("@name",MySqlDbType.String),
                new MySqlParameter("@date",MySqlDbType.DateTime),
                new MySqlParameter("@feeding_number",MySqlDbType.Int32),
                new MySqlParameter("@waste_number",MySqlDbType.Int32),
                new MySqlParameter("@pass_rate",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = passRate.Id;
            parameters[1].Value = passRate.Number;
            parameters[2].Value = passRate.Name;
            parameters[3].Value = passRate.Date;
            parameters[4].Value = passRate.FeedingNumber;
            parameters[5].Value = passRate.WasteNumber;
            parameters[6].Value = passRate.Pass_Rate;
            parameters[10].Value = passRate.CreateTime;
            parameters[11].Value = passRate.UpdateTime;
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
                strSql.Append(" where " + strSql);
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
            strSql.Append("DELETE * FROM pass_rate where id = '" + id+"'");
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
                if (dataRow["number"] != null && dataRow["number"].ToString() != "")
                {
                    model.Number = dataRow["number"].ToString();
                }

                if (dataRow["name"] != null && dataRow["name"].ToString() != "")
                {
                    model.Name = dataRow["name"].ToString();
                }
                if (dataRow["date"] != null && dataRow["date"].ToString() != "")
                {
                    model.Date = (DateTime)dataRow["date"];
                }
                if (dataRow["feeding_number"] != null && dataRow["feeding_number"].ToString() != "")
                {
                    model.FeedingNumber = Int32.Parse(dataRow["feeding_number"].ToString());
                }
                if (dataRow["waste_number"] != null && dataRow["waste_number"].ToString() != "")
                {
                    model.WasteNumber = Int32.Parse(dataRow["waste_number"].ToString());
                }
           
                if (dataRow["Pass_Rate"] != null && dataRow["Pass_Rate"].ToString() != "")
                {
                    model.Pass_Rate = dataRow["Pass_Rate"].ToString();
                }

                if (dataRow["update_time"] != null && dataRow["update_time"].ToString() != "")
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
            strSql.Append("select count(1) FROM pass_rate ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            object obj = DbHelperSQL.GetSingle(strSql.ToString());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }


        /// <summary>
        /// 设置modl
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public PassRate SetCheckPoint(PassRate pass)
        {
            PassRate passRate = new PassRate();
            passRate.Id = GetRandomString();
            passRate.Number = pass.Number;
            passRate.Name = pass.Name;
            passRate.Date = DateTime.Now;
            passRate.FeedingNumber = pass.FeedingNumber;
            passRate.WasteNumber = pass.WasteNumber;
            passRate.Pass_Rate = pass.Pass_Rate;
            passRate.CreateTime = DateTime.Now;
            passRate.UpdateTime = pass.UpdateTime;
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
