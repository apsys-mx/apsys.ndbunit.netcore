using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;

namespace apsys.ndbunit.netcore
{

    /// <summary>
    /// Abstract base class for database specific implementation
    /// </summary>
    public abstract class NDbUnit: INDbUnit
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="connectionString"></param>
        protected NDbUnit(DataSet dataSet, string connectionString)
        {
            this.DataSet = dataSet;
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets the dataSet containing the tables where the operations are execute
        /// </summary>
        public DataSet DataSet { get; private set; }

        /// <summary>
        /// Gets the connection string to the database where the operations are execute
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Get a dataset with the tables and data from the database
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataSetFromDb()
        {
            using DbConnection cnn = this.CreateConnection();
            DataSet dsetResult = this.DataSet.Clone();
            dsetResult.EnforceConstraints = false;
            var dbFactory = DbProviderFactories.GetFactory(cnn);
            foreach (DataTable table in this.DataSet.Tables)
            {
                DbCommand selectCommand = cnn.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM {table.TableName}";

                DbDataAdapter adapter = dbFactory.CreateDataAdapter();
                adapter.SelectCommand = selectCommand;
                adapter.Fill(dsetResult, table.TableName);
            }
            dsetResult.EnforceConstraints = true;
            return dsetResult;
        }

        /// <summary>
        /// Get a dataset with the tables and data from the database
        /// </summary>
        /// <param name="tableNames">The list of tables to recover</param>
        /// <returns></returns>
        public DataSet GetDataSetFromDb(StringCollection tableNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clear all the database data
        /// </summary>
        public void ClearDatabase()
        {
            using IDbConnection cnn = this.CreateConnection();
            cnn.Open();

            using (IDbTransaction transaction = cnn.BeginTransaction())
            {
                try
                {
                    foreach (DataTable dataTable in this.DataSet.Tables)
                    {
                        var cmd = cnn.CreateCommand();
                        cmd.Transaction = transaction;
                        cmd.CommandText = $"DELETE FROM {dataTable.TableName}";
                        cmd.Connection = cnn;
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    transaction.Rollback();
                }
            }
            cnn.Close();
        }

        /// <summary>
        /// Creates a DbConnection
        /// </summary>
        /// <returns></returns>
        public abstract DbConnection CreateConnection();

        /// <summary>
        /// Enable datatable's constraints
        /// </summary>
        /// <param name="dbTransaction"></param>
        protected abstract void EnabledTableConstraints(IDbTransaction dbTransaction);

        /// <summary>
        /// Disable datatable's constraints
        /// </summary>
        /// <param name="dbTransaction"></param>
        protected abstract void DisableTableConstraints(IDbTransaction dbTransaction);

    }
}