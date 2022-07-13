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
            strSql.Append("insert into outsourcing(").Append("id,outsource_date,return_date,processing_number,processing_components,processoin_quality,create_time,update_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@outsource_date,@return_date,@processing_number,@processing_components,@processoin_quality,@create_time,@update_time)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@outsource_date",MySqlDbType.DateTime),
                new MySqlParameter("@return_date",MySqlDbType.DateTime),
                new MySqlParameter("@processing_number",MySqlDbType.Int32),
                new MySqlParameter("@processing_components",MySqlDbType.String),
                new MySqlParameter("@processoin_quality",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = outsourcing.Id;
            parameters[1].Value = outsourcing.OutsourceDate;
            parameters[2].Value = outsourcing.ReturnDate;
            parameters[3].Value = outsourcing.Number;
            parameters[4].Value = outsourcing.Components;
            parameters[5].Value = outsourcing.Quality;
            parameters[6].Value = outsourcing.CreateTime;
            parameters[7].Value = outsourcing.UpdateTime;
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
            strSql.Append("outsource_date=@outsource_date,");
            strSql.Append("return_date=@return_date,");
            strSql.Append("processing_number=@processing_number,");
            strSql.Append("processing_components=@processing_components,");
            strSql.Append("processoin_quality=@processoin_quality,");
            strSql.Append("create_time=@create_time");
            strSql.Append("update_time=@update_time");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters =
            {
                 new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@outsource_date",MySqlDbType.DateTime),
                new MySqlParameter("@return_date",MySqlDbType.DateTime),
                new MySqlParameter("@processing_number",MySqlDbType.Int32),
                new MySqlParameter("@processing_components",MySqlDbType.String),
                new MySqlParameter("@processoin_quality",MySqlDbType.String),
                new MySqlParameter("@create_time",MySqlDbType.DateTime),
                new MySqlParameter("@update_time",MySqlDbType.DateTime)};
            parameters[0].Value = outsourcing.Id;
            parameters[1].Value = outsourcing.OutsourceDate;
            parameters[2].Value = outsourcing.ReturnDate;
            parameters[3].Value = outsourcing.Number;
            parameters[4].Value = outsourcing.Components;
            parameters[5].Value = outsourcing.Quality;
            parameters[6].Value = outsourcing.CreateTime;
            parameters[7].Value = outsourcing.UpdateTime;
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
            strSql.Append("DELETE * FROM outsourcing where id = '" + id+"'");
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
                if (dataRow["outsource_date"] != null && dataRow["outsource_date"].ToString() != "")
                {
                    model.OutsourceDate = (DateTime)dataRow["outsource_date"];
                }

                if (dataRow["ReturnDate"] != null && dataRow["ReturnDate"].ToString() != "")
                {
                    model.ReturnDate = (DateTime)dataRow["return_date"];
                }
                if (dataRow["processing_number"] != null && dataRow["processing_number"].ToString() != "")
                {
                    model.Number = int.Parse(dataRow["processing_number"].ToString());
                }
                if (dataRow["processing_components"] != null && dataRow["processing_components"].ToString() != "")
                {
                    model.Components = dataRow["processing_components"].ToString();
                }
                if (dataRow["processoin_quality"] != null && dataRow["processoin_quality"].ToString() != "")
                {
                    model.Quality = dataRow["processoin_quality"].ToString();
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
            strSql.Append("select count(1) FROM outsourcing ");
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
        /// <param name="outs"></param>
        /// <returns></returns>
        public Outsourcing SetOutsourcing(Outsourcing outs)
        {
            Outsourcing outsourcing = new Outsourcing();
            outsourcing.Id = GetRandomString();
            outsourcing.OutsourceDate = outs.OutsourceDate;
            outsourcing.ReturnDate = outs.ReturnDate;
            outsourcing.Number = outs.Number;
            outsourcing.Components = outs.Components;
            outsourcing.Quality = outs.Quality;
            outsourcing.CreateTime = DateTime.Now;
            outsourcing.UpdateTime = DateTime.Now;
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
