using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class ImagesMesDAL
    {
        public ImagesMesDAL() { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="imagesMes"></param>
        /// <returns></returns>
        public bool Add(Images_Mes imagesMes)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into images_mes(").Append("id,File_Name,File_Type,File_Size,Add_Time,File_Con)");
            strSql.Append(" values (@id,@File_Name,@File_Type,@File_Size,@Add_Time,@File_Con)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id",MySqlDbType.String),
                new MySqlParameter("@File_Name",MySqlDbType.String),
                new MySqlParameter("@File_Type",MySqlDbType.String),
                new MySqlParameter("@File_Size",MySqlDbType.String),
                new MySqlParameter("@Add_Time",MySqlDbType.DateTime),
                new MySqlParameter("@File_Con",MySqlDbType.Blob)};
            parameters[0].Value = imagesMes.Id;
            parameters[1].Value = imagesMes.FileName;
            parameters[2].Value = imagesMes.FileType;
            parameters[3].Value = imagesMes.FileSize;
            parameters[4].Value = imagesMes.AddTime;
            parameters[5].Value = imagesMes.FileCon;
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
        /// 查询详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Images_Mes GetById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM images_mes where id = '" + id + "'");
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
        public Images_Mes DataRowToModel(DataRow dataRow)
        {
            Images_Mes model = new Images_Mes();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = dataRow["ID"].ToString();
                }
                if (dataRow["File_Name"] != null && dataRow["File_Name"].ToString() != "")
                {
                    model.FileName = dataRow["File_Name"].ToString();
                }

                if (dataRow["File_Type"] != null && dataRow["File_Type"].ToString() != "")
                {
                    model.FileType = dataRow["File_Type"].ToString();
                }
                if (dataRow["File_Size"] != null && dataRow["File_Size"].ToString() != "")
                {
                    model.FileSize = long.Parse(dataRow["File_Size"].ToString());;
                }

                if (dataRow["Add_Time"] != null && dataRow["Add_Time"].ToString() != "")
                {
                    model.AddTime = (DateTime)dataRow["Add_Time"];
                }

                if (dataRow["Modify_Time"] != null && dataRow["Modify_Time"].ToString() != "")
                {
                    model.ModifyTime = (DateTime)dataRow["Modify_Time"];
                }

                if (dataRow["File_Con"] != null && dataRow["File_Con"].ToString() != "")
                {
                    model.FileCon = (byte[])dataRow["File_Con"];
                }

            }
            return model;
        }

       
        #endregion  BasicMethod
    }
}
