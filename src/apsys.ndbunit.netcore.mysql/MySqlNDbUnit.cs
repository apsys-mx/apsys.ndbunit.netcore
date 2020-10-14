using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;

namespace apsys.ndbunit.netcore.mysql
{
    public class MySqlNDbUnit : NDbUnit
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="connectionString"></param>
        public MySqlNDbUnit(DataSet dataSet, string connectionString) 
            : base(dataSet, connectionString)
        {
        }

        /// <summary>
        /// Creates the connection to the database
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection() => new MySqlConnection(this.ConnectionString);

        protected override void DisableTableConstraints(IDbTransaction dbTransaction, DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        protected override void EnabledTableConstraints(IDbTransaction dbTransaction, DataTable dataTable)
        {
            throw new NotImplementedException();
        }
    }
}
