using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        //[TestCase("Customer", 800)]
        public void GetDatabase_ReadAllTables_TablesFoundWithRecords(string tableName, int expectedRowCount)
        {
            var cnn = this.ClassUnderTest.CreateConnection();
            LoadAddressTable(cnn);

            DataSet dataSet = this.ClassUnderTest.GetDataSetFromDb();
            DataTable dataTable = dataSet.Tables[tableName];
            Assert.IsNotNull(dataTable);
            Assert.That(dataTable.Rows.Count, Is.EqualTo(expectedRowCount));
        }

        /// <summary>
        /// Load the address table
        /// </summary>
        private void LoadAddressTable(DbConnection cnn)
        {
            AdventureWorksSchema awDataset = new AdventureWorksSchema();

            var factory = DbProviderFactories.GetFactory(cnn);
            DbDataAdapter adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = factory.CreateCommand();
            adapter.SelectCommand.CommandText = "SELECT * FROM Address";
            adapter.SelectCommand.Connection = cnn;
            DbCommandBuilder commandBuilder = factory.CreateCommandBuilder();
            commandBuilder.DataAdapter = adapter;
            commandBuilder.GetInsertCommand();

            this.LoadAddressDataRows(awDataset);

            adapter.Update(awDataset, "Address");
        }

        private void LoadAddressDataRows(AdventureWorksSchema schema)
        {
            foreach (var addressLine in this.GetTextFileLines("Address.txt"))
            {
                string[] allAddressColumns = addressLine.Split('|');
                var addressId = int.Parse(allAddressColumns[0]);
                var dataRow = schema.Address.Rows.Add(addressId, allAddressColumns[1], allAddressColumns[2], allAddressColumns[3], allAddressColumns[4], allAddressColumns[5], allAddressColumns[6], DateTime.Parse(allAddressColumns[7].ToString()).ToString("yyyy-MM-dd H:mm:ss"));
            }
        }

        /// <summary>
        /// Get all lines in a text file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private IEnumerable<string> GetTextFileLines(string fileName)
        {
            var location = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var rootDir = new FileInfo(location.AbsolutePath).Directory;
            var filePath = Path.Combine(rootDir.FullName, "data", fileName);
            return File.ReadLines(filePath);
        }
    }
}