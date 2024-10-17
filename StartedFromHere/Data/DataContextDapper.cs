using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data
{
    class DataContextDapper
    {
        private readonly IConfiguration _config;
        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Load every instance of a valid Entity from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }


        /// <summary>
        /// Load a single instance of an Entity from the database. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql);
        }

        // public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
        // {

        //     SqlCommand commandWithParameters = new SqlCommand(sql);

        //     foreach (SqlParameter parameter in parameters)
        //     {
        //         commandWithParameters.Parameters.Add(parameter);
        //     }

        //     SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        //     dbConnection.Open();
        //     commandWithParameters.Connection = dbConnection;

        //     int rowsAffected = commandWithParameters.ExecuteNonQuery();

        //     dbConnection.Close();

        //     return rowsAffected > 0;
            
        // }

        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
        {
            SqlCommand commandWithParams = new SqlCommand(sql);

            foreach(SqlParameter parameter in parameters)
            {
                commandWithParams.Parameters.Add(parameter);
            }

            SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            dbConnection.Open();

            commandWithParams.Connection = dbConnection;

            int rowsAffected = commandWithParams.ExecuteNonQuery();

            dbConnection.Close();

            return rowsAffected > 0;
        }
        
    }
}