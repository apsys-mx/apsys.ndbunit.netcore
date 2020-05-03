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
        protected override DbConnection CreateConnection() => new SqlConnection(this.ConnectionString);

        protected override void DisableTableConstraints(IDbTransaction dbTransaction)
        {
            throw new NotImplementedException();
        }

        protected override void EnabledTableConstraints(IDbTransaction dbTransaction)
        {
            throw new NotImplementedException();
        }
    }
}
