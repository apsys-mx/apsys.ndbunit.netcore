using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace apsys.ndbunit.netcore.sqlserver
{
    public class SqlClienteNDbUnit : NDbUnit
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="connectionString"></param>
        public SqlClienteNDbUnit(DataSet dataSet, string connectionString)
            : base(dataSet, connectionString)
        {
        }

        /// <summary>
        /// Creates the connection to the database
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection() => new SqlConnection(this.ConnectionString);

        protected override void DisableTableConstraints(IDbTransaction dbTransaction, DataTable dataTable)
        {
            DbCommand sqlCommand = new SqlCommand("ALTER TABLE " + (dataTable.TableName) + " NOCHECK CONSTRAINT ALL");
            sqlCommand.Connection = (DbConnection)dbTransaction.Connection;
            sqlCommand.Transaction = (DbTransaction)dbTransaction;
            sqlCommand.ExecuteNonQuery();
        }

        protected override void EnabledTableConstraints(IDbTransaction dbTransaction, DataTable dataTable)
        {
            DbCommand sqlCommand = new SqlCommand("ALTER TABLE " + (dataTable.TableName) + " CHECK CONSTRAINT ALL");
            sqlCommand.Connection = (DbConnection)dbTransaction.Connection;
            sqlCommand.Transaction = (DbTransaction)dbTransaction;
            sqlCommand.ExecuteNonQuery();
        }
    }
}
