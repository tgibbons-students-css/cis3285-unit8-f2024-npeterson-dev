using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingleResponsibilityPrinciple;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple.Tests
{
    [TestClass()]
    public class TradeProcessorTests
    {

        private int CountDbRecords()
        {
            string tomConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\kimno\source\repos\tgibbons-students-css\cis3285-unit8-f2024-npeterson-dev\Unit8_SRP_F24\DataFiles\tradedatabase.mdf;Integrated Security=True;Connect Timeout=30";
            //string azureConnectString = @"Server=tcp:cis3285-sql-server.database.windows.net,1433; Initial Catalog = Unit8_TradesDatabase; Persist Security Info=False; User ID=cis3285;Password=Saints4SQL; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 60;";
            // Change the connection string used to match the one you want
            using (var connection = new SqlConnection(tomConnectionString))
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
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit8_SRP_F24Tests.goodtrades1.txt");
            var tradeProcessor = new TradeProcessor();

            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            //Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore + 4, countAfter);
        }

        [TestMethod()]
        public void TestTenFile()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit8_SRP_F24Tests.goodtrades10.txt");
            var tradeProcessor = new TradeProcessor();

            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            //Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore + 10, countAfter);
        }

        [TestMethod()]
        public void TestZeroFile()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit8_SRP_F24Tests.zerotrades.txt");
            var tradeProcessor = new TradeProcessor();

            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            //Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadTradeData_NonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var tradeProcessor = new TradeProcessor();
            string nonExistentFilePath = @"C:\path\to\nonexistentfile.txt";

            // Attempt to read from a non-existent file
            using (var stream = new FileStream(nonExistentFilePath, FileMode.Open))
            {
                tradeProcessor.ReadTradeData(stream);
            }

            // Assert is handled by the ExpectedException attribute
        }

        [TestMethod()]
        public void TestBadTradeFile_TooManyValues()
        {
            // Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit8_SRP_F24Tests.badtrade.txt");
            var tradeProcessor = new TradeProcessor();

            // Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            int countAfter = CountDbRecords();

            // Assert
            Assert.AreEqual(countBefore, countAfter, "Trades should not be added to the database due to a bad trade format.");
        }


        [TestMethod()]
        public void ProcessTradesTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void ReadTradeDataTest()
        {
            Assert.Fail();
        }
    }
}