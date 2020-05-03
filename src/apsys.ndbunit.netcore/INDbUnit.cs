using System.Collections.Specialized;
using System.Data;

namespace apsys.ndbunit.netcore
{
    public interface INDbUnit
    {
        
        /// <summary>
        /// Gets the dataSet containing the tables where the operations are execute
        /// </summary>
        DataSet DataSet { get; }

        /// <summary>
        /// Gets the connection string to the database where the operations are execute
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Get a dataset with the tables and data from the database
        /// </summary>
        /// <returns></returns>
        DataSet GetDataSetFromDb();

        /// <summary>
        /// Get a dataset with the tables and data from the database
        /// </summary>
        /// <param name="tableNames">The list of tables to recover</param>
        /// <returns></returns>
        DataSet GetDataSetFromDb(StringCollection tableNames);

        /// <summary>
        /// Clear all the database data
        /// </summary>
        void ClearDatabase();

    }
}