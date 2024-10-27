using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using SingleResponsibilityPrinciple;

namespace TradeProcessorTests
{
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

        [Fact]
        public void TestFileWithFourTrades()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TradeProcessorTests.goodtrades.txt");
            var tradeProcessor = new TradeProcessor();

            //Act
            int count = tradeProcessor.ReadTradeData(tradeStream).Count();
            //Assert
            Assert.Equal(4, count);
        }

        [Fact]
        public void TestDatabaseStorageWithFourTrades()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TradeProcessorTests.goodtrades.txt");
            var tradeProcessor = new TradeProcessor();

            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            //Assert
            Assert.Equal(countBefore + 4, CountDbRecords());
        }

        [Fact]
        public void TestDatabaseStorageWithTenTrades()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TradeProcessorTests.tentrades.txt");
            var tradeProcessor = new TradeProcessor();

            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            //Assert
            int countAfter = CountDbRecords();
            Assert.Equal(countBefore + 10, countAfter);
        }

        [Fact]
        public void TestFileWithZeroTrades() {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TradeProcessorTests.zerotrades.txt");
            var tradeProcessor = new TradeProcessor();

            //Act

            int count = tradeProcessor.ReadTradeData(tradeStream).Count();
            //Assert
 
            Assert.Equal(0, count);
        }

        [Fact]
        public void TestNullFile() {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("nulltrades.txt");
            var tradeProcessor = new TradeProcessor();

            // Act & Assert
            Assert.Throws<ArgumentNullException> (() => 
                tradeProcessor.ReadTradeData(tradeStream));
        }

        [Fact]
        public void TestNegativeTradeAmount() {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TradeProcessorTests.negativetradeamount.txt"); // File with only negative amounts. SHould not be added as trades
            var tradeProcessor = new TradeProcessor();

            // Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);

            // Assert
            int countAfter = CountDbRecords();
            Assert.Equal(countBefore, countAfter);
        } 

        [Fact]
        public void TestBadTradeData() {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TradeProcessorTests.badtrades.txt"); // File with the first line being invalid. SHould not add any trades
            var tradeProcessor = new TradeProcessor();

            // Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);

            // Assert
            int countAfter = CountDbRecords();
            Assert.Equal(countBefore, countAfter);
        }
    }
}