using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hnc.iGC.Web
{
    public class EmployeeDAL
    {
        public EmployeeDAL() { }
        #region  BasicMethod

        
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<Employee> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from employee_table ");
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
        public Employee GetById(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM employee_table where id = '" + id + "'");
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
            strSql.Append("select count(1) count FROM employee_table ");
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
        public Employee DataRowToModel(DataRow dataRow)
        {
            Employee model = new Employee();
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
                if (dataRow["remarks"] != null && dataRow["remarks"].ToString() != "")
                {
                    model.Remarks = dataRow["remarks"].ToString();
                }
            }
            return model;
        }

        /// <summary>
        /// 转list对象数据
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public List<Employee> DataRowToModelList(DataSet dataSet)
        {
            List<Employee> EmployeeList = new List<Employee>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                Employee employee = DataRowToModel(dataSet.Tables[0].Rows[i]);
                EmployeeList.Add(employee);
            }
            return EmployeeList;
        }

        #endregion  BasicMethod
    }
}
