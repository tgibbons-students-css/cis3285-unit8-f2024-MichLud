using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingleResponsibilityPrinciple; // Add this line to reference the namespace containing TradeProcessor

namespace SingleResponsibilityPrinciple.Tests
{
    [TestClass()]
    public class TradeProcessorTests
    {
        private int CountDbRecords()
        {
            string azureConnectString = @"Server=tcp:cis3285.database.windows.net,1433;Initial Catalog=SampleDB;Persist Security Info=False;User ID=mludwig2;Password=Eialamu223;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            // Change the connection string used to match the one you want
            using (var connection = new SqlConnection(azureConnectString))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                string myScalarQuery = "SELECT COUNT(*) FROM trade";
                SqlCommand myCommand = new SqlCommand(myScalarQuery, connection);
                //myCommand.Connection.Open();
                int count = (int)myCommand.ExecuteScalar();
                connection.Close();
                return count;
            }
        }

        [TestMethod()]
        public void TestNormalFile()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TradeProcessorTests.goodtrades.txt");
            var tradeProcessor = new TradeProcessor();

            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            //Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore + 4, countAfter);
        }
    }
}