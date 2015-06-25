using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace MPSfwk
{
	public class DataAccess
	{
        private static OleDbConnection GetConnection()
        {
            OleDbConnection conn = new OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            return conn;
        }

        private static void CloseConnection(OleDbConnection conn)
        {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
            conn.Dispose();
        }
	
		public static OleDbDataReader ExecuteReader(OleDbCommand comando)
		{
			OleDbConnection conexao  = GetConnection();
			comando.Connection = conexao;
            //comando.CommandType = CommandType.StoredProcedure;

            comando.Connection.Open();
			OleDbDataReader reader = comando.ExecuteReader(CommandBehavior.CloseConnection);
		
			return reader;
		}

        public static void ExecuteNonQuery(OleDbCommand comando)
        {
            try
            {
                OleDbConnection conexao = GetConnection();
                comando.Connection = conexao;
                //comando.CommandType = CommandType.StoredProcedure;

                comando.Connection.Open();
                comando.ExecuteNonQuery();
                CloseConnection(comando.Connection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		public static int ExecuteScalarWithScopeIdentity(OleDbCommand comando)
		{			
			object scopeID = ExecuteScalar(comando);
			if (scopeID != null && scopeID != DBNull.Value)
				return Convert.ToInt32(scopeID);
			else
				return -1;
		}

		public static object ExecuteScalar(OleDbCommand comando)
		{
			OleDbConnection conexao = GetConnection();
			comando.Connection    = conexao;
            //comando.CommandType = CommandType.StoredProcedure;
			
			comando.Connection.Open();
			object obj = comando.ExecuteScalar();
            CloseConnection(comando.Connection);
			
			return obj;
		}
		
	}
}
namespace SQLServer
{
    public class DataAccess
    {
        private static SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            return conn;
        }

        private static void CloseConnection(SqlConnection conn)
        {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
            conn.Dispose();
        }

        public static SqlDataReader ExecuteReader(SqlCommand comando)
        {
            SqlConnection conexao = GetConnection();
            comando.Connection = conexao;
            //comando.CommandType = CommandType.StoredProcedure;

            comando.Connection.Open();
            SqlDataReader reader = comando.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static void ExecuteNonQuery(SqlCommand comando)
        {
            try
            {
                SqlConnection conexao = GetConnection();
                comando.Connection = conexao;
                //comando.CommandType = CommandType.StoredProcedure;

                comando.Connection.Open();
                comando.ExecuteNonQuery();
                CloseConnection(comando.Connection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int ExecuteScalarWithScopeIdentity(SqlCommand comando)
        {
            object scopeID = ExecuteScalar(comando);
            if (scopeID != null && scopeID != DBNull.Value)
                return Convert.ToInt32(scopeID);
            else
                return -1;
        }

        public static object ExecuteScalar(SqlCommand comando)
        {
            SqlConnection conexao = GetConnection();
            comando.Connection = conexao;
            //comando.CommandType = CommandType.StoredProcedure;

            comando.Connection.Open();
            object obj = comando.ExecuteScalar();
            CloseConnection(comando.Connection);

            return obj;
        }

    }
}