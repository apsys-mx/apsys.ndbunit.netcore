using NUnit.Framework;
using System.Data;

namespace apsys.ndbunit.netcore.tests
{
    public abstract class NDbUnitTests<T> where T: NDbUnit
    {

        public T ClassUnderTest { get; private set; }

        /// <summary>
        /// One time setup
        /// </summary>
        [SetUp]
        public void Setup()
        {
            string connectionString = this.GetConnectionString();
            AdventureWorksSchema schema = new AdventureWorksSchema();
            this.ClassUnderTest = this.CreateClassUnderTest(schema, connectionString);
        }

        [Test]
        public void GetDatabase()
        {
            DataSet dataSet = this.ClassUnderTest.GetDataSetFromDb();
            Assert.That(dataSet.Tables.Count, Is.EqualTo(1));
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

    }
}