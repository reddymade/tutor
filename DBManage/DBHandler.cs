using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace InstantTutors.DBManage
{
    public class DBHandler
    {
        public string sqlConnectString;
        public SqlConnection sqlConnection;
        public DBHandler()
        {
            sqlConnectString = ConfigurationManager.ConnectionStrings["ITutorsConnection"].ToString();            
            sqlConnection = new SqlConnection(sqlConnectString);
        }
        public DataTable GetData(string query)
        {
            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCommand);
            DataTable sqlDt = new DataTable();
            sqlDa.Fill(sqlDt);
            return sqlDt;
        }

        public int TranscatData(string query)
        {
            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            return sqlCommand.ExecuteNonQuery();
        }

    }
}