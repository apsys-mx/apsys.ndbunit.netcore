using NUnit.Framework;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;

namespace apsys.ndbunit.netcore.tests
{
    public abstract class NDbUnitTests<T> where T: NDbUnit
    {

        public T ClassUnderTest { get; private set; }

        /// <summary>
        /// Setup
        /// </summary>
        [SetUp]
        public void Setup()
        {
            string connectionString = this.GetConnectionString();
            AdventureWorksSchema schema = new AdventureWorksSchema();
            this.ClassUnderTest = this.CreateClassUnderTest(schema, connectionString);
            this.ClassUnderTest.ClearDatabase();
        }

        [Test]
        public void GetDatabase_ReadAllTables_TablesCountCorrect()
        {
            DataSet dataSet = this.ClassUnderTest.GetDataSetFromDb();
            Assert.That(dataSet.Tables.Count, Is.EqualTo(1));
        }

        [TestCase("Address", 450)]
        public void GetDatabase_ReadAllTables_TablesFoundWithRecords(string tableName, int expectedRowCount)
        {
            LoadAddressTable();
            DataSet dataSet = this.ClassUnderTest.GetDataSetFromDb();
            DataTable dataTable = dataSet.Tables[tableName];
            Assert.IsNotNull(dataTable);
            Assert.That(dataTable.Rows.Count, Is.EqualTo(expectedRowCount));
        }

        /// <summary>
        /// Create the class under test
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected abstract T CreateClassUnderTest(DataSet schema, string connectionString);

        /// <summary>
        /// Get the connection string 
        /// </summary>
        /// <returns></returns>
        protected internal abstract string GetConnectionString();


        private void LoadAddressTable()
        {
            using var cnn = this.ClassUnderTest.CreateConnection();
            cnn.Open();

            using var trx = cnn.BeginTransaction();
            var location = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var rootDir =  new FileInfo(location.AbsolutePath).Directory;
            var filePath = Path.Combine(rootDir.FullName, "data", "Address.txt");
            var allAddressLines = File.ReadLines(filePath);

            try
            {
                foreach (var addressLine in allAddressLines)
                {
                    string[] parameters = addressLine.Split('|');
                    var addressId = int.Parse(parameters[0]);
                    DbCommand insertCommand = cnn.CreateCommand();
                    insertCommand.Transaction = trx;
                    insertCommand.CommandType = CommandType.Text;
                    insertCommand.CommandText =
                        "INSERT INTO ADDRESS" +
                        "(AddressID, AddressLine1, AddressLine2, City, StateProvince, CountryRegion, PostalCode, ModifiedDate)" +
                        "VALUES" +
                        $"({addressId}, '{"AddressLine1"}', '{"AddressLine2"}', '{"City"}', '{"StateProvince"}', '{"CountryRegion"}', '{"PostalCode"}', '{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss")}')";

                    insertCommand.ExecuteNonQuery();
                }
                trx.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                trx.Rollback();
            }
            cnn.Close();
        }
    }
}