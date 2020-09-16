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
            Assert.That(dataSet.Tables.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetDatabase_ReadAllTables_TablesFoundWithRecords()
        {
            var cnn = this.ClassUnderTest.CreateConnection();
            LoadAddressTable(cnn, this.LoadAddressDataRows, "SELECT * FROM Address");
            LoadCustomersTable(cnn, this.LoadCustomerDataRows, "SELECT * FROM Customer");

            DataSet dataSet = this.ClassUnderTest.GetDataSetFromDb();
            DataTable addressDataTable = dataSet.Tables["Address"];
            Assert.IsNotNull(addressDataTable, $"No Address table found in the dataset");
            Assert.That(addressDataTable.Rows.Count, Is.EqualTo(450));

            DataTable customerDataTable = dataSet.Tables["Customer"];
            Assert.IsNotNull(customerDataTable, $"No Customer table found in the dataset");
            Assert.That(customerDataTable.Rows.Count, Is.EqualTo(847));
        }

        /// <summary>
        /// Load the address table
        /// </summary>
        private void LoadCustomersTable(DbConnection cnn, Action<AdventureWorksSchema> loadCustomers, string selectCommandText)
        {
            AdventureWorksSchema awDataset = new AdventureWorksSchema();

            var factory = DbProviderFactories.GetFactory(cnn);
            DbDataAdapter adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = factory.CreateCommand();
            adapter.SelectCommand.CommandText = selectCommandText;
            adapter.SelectCommand.Connection = cnn;
            DbCommandBuilder commandBuilder = factory.CreateCommandBuilder();
            commandBuilder.DataAdapter = adapter;
            commandBuilder.GetInsertCommand();

            loadCustomers(awDataset);

            adapter.Update(awDataset, "Customer");
        }

        /// <summary>
        /// Load the address table
        /// </summary>
        private void LoadAddressTable(DbConnection cnn, Action<AdventureWorksSchema> loadAddressRows, string selectCommandText)
        {
            AdventureWorksSchema awDataset = new AdventureWorksSchema();

            var factory = DbProviderFactories.GetFactory(cnn);
            DbDataAdapter adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = factory.CreateCommand();
            adapter.SelectCommand.CommandText = selectCommandText;
            adapter.SelectCommand.Connection = cnn;
            DbCommandBuilder commandBuilder = factory.CreateCommandBuilder();
            commandBuilder.DataAdapter = adapter;
            commandBuilder.GetInsertCommand();

            loadAddressRows(awDataset);

            adapter.Update(awDataset, "Address");
        }

        private void LoadAddressDataRows(AdventureWorksSchema schema)
        {
            foreach (var addressLine in this.GetTextFileLines("Address.txt"))
            {
                string[] allAddressColumns = addressLine.Split('|');
                var addressId = int.Parse(allAddressColumns[0]);
                schema.Address.Rows.Add(addressId, allAddressColumns[1], allAddressColumns[2], allAddressColumns[3], allAddressColumns[4], allAddressColumns[5], allAddressColumns[6], DateTime.Parse(allAddressColumns[7].ToString()).ToString("yyyy-MM-dd H:mm:ss"));
            }
        }

        private void LoadCustomerDataRows(AdventureWorksSchema schema)
        {
            foreach (var customerLine in this.GetTextFileLines("Customer.txt"))
            {
                string[] allCustomerColumns = customerLine.Split('|');
                var customerId = int.Parse(allCustomerColumns[0]);
                schema.Customer.Rows.Add(
                    customerId, 
                    false, 
                    allCustomerColumns[2], 
                    allCustomerColumns[3], 
                    allCustomerColumns[4], 
                    allCustomerColumns[5], 
                    allCustomerColumns[6], 
                    allCustomerColumns[7],
                    allCustomerColumns[8],
                    allCustomerColumns[9],
                    allCustomerColumns[10],
                    allCustomerColumns[11],
                    allCustomerColumns[12],
                    //allCustomerColumns[13],
                    DateTime.Parse(allCustomerColumns[14].ToString()).ToString("yyyy-MM-dd H:mm:ss"));
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