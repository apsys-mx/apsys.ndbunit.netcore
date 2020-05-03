using apsys.ndbunit.netcore.tests;
using NUnit.Framework;
using System.Data;

namespace apsys.ndbunit.netcore.sqlserver.tests
{
    public class SqlClienteNDbUnitTests: NDbUnitTests<SqlClienteNDbUnit>
    {

        /// <summary>
        /// Create the class under test of type SqlClienteNDbUnit
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected override SqlClienteNDbUnit CreateClassUnderTest(DataSet schema, string connectionString) => 
            new SqlClienteNDbUnit(schema, connectionString);


        [Test]
        public void IsNotNull()
        {
            Assert.IsNotNull(this.ClassUnderTest);
        }
        
        /// <summary>
        /// Get the connection string to the database
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionString() => "Server=localhost\\SQLEXPRESS;Database=AdventureWorks;Trusted_Connection=True;";
    }
}