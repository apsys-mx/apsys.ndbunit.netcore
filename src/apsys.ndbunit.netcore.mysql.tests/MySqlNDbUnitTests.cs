using apsys.ndbunit.netcore.tests;
using NUnit.Framework;
using System.Data;

namespace apsys.ndbunit.netcore.mysql.tests
{
    public class MySqlNDbUnitTests : NDbUnitTests<MySqlNDbUnit>
    {
        /// <summary>
        /// Create the class under test of type MySqlNDbUnit
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected override MySqlNDbUnit CreateClassUnderTest(DataSet schema, string connectionString) =>
            new MySqlNDbUnit(schema, connectionString);

        [Test]
        public void IsNotNull()
        {
            Assert.IsNotNull(this.ClassUnderTest);
        }

        /// <summary>
        /// Get the connection string to the database
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionString() => "Server=localhost;Database=adventureworks;Uid=root;Pwd=root";

    }
}