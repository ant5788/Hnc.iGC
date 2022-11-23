using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class DictCodeDAL
    {
        public DictCodeDAL() { }
        #region  BasicMethod


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<DictCode> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dict_code ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DataRowToModelList(DbHelperMySQL.Query(strSql.ToString()));
        }



        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DictCode GetById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM dict_code where id = '" + id + "'");
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
        /// 统计条数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) count FROM dict_code ");
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
        /// 数据转为对象 toModel
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public DictCode DataRowToModel(DataRow dataRow)
        {
            DictCode model = new DictCode();
            if (null != dataRow)
            {
                if (dataRow["id"] != null && dataRow["id"].ToString() != "")
                {
                    model.Id = int.Parse(dataRow["ID"].ToString());
                }
                if (dataRow["code_group"] != null && dataRow["code_group"].ToString() != "")
                {
                    model.CodeGroup = int.Parse(dataRow["code_group"].ToString());
                }
                if (dataRow["code"] != null && dataRow["code"].ToString() != "")
                {
                    model.Code = int.Parse(dataRow["code"].ToString());
                }
                if (dataRow["code_msg"] != null && dataRow["code_msg"].ToString() != "")
                {
                    model.CodeMsg = dataRow["code_msg"].ToString();
                }
                if (dataRow["remark"] != null && dataRow["remark"].ToString() != "")
                {
                    model.remark = dataRow["remark"].ToString();
                }
            }
            return model;
        }

        /// <summary>
        /// 转list对象数据
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public List<DictCode> DataRowToModelList(DataSet dataSet)
        {
            List<DictCode> dictCodeList = new List<DictCode>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DictCode dictCode = DataRowToModel(dataSet.Tables[0].Rows[i]);
                dictCodeList.Add(dictCode);
            }
            return dictCodeList;
        }
        #endregion  BasicMethod
    }
}
