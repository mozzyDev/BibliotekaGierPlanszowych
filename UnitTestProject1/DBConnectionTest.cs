using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BibliotekaGierPlanszowych;
using System.Collections.Generic;


namespace UnitTestProject1
{
    [TestClass]
    public class DBConnectionTest
    {
        [TestMethod]
        public void DatabaseDataGettingTest()
        {
            // Arrange
            DBConnection testedObject = new DBConnection();

            //Act
            List<String> testedList = testedObject.DatabaseDataGetting("category", "id_category", 0);

            //Assert
            Assert.IsNotNull(testedList);
        }

        [TestMethod]
        public void DatabaseQueryExecuteTest()
        {
            // Arrange
            DBConnection testedObject = new DBConnection();

            //Act
            List<String> testedList = testedObject.DatabasQueryExecute("SELECT * FROM 'category'");

            //Assert
            Assert.IsNotNull(testedList);
        }

        [TestMethod]
        public void IsDatabaseDataGetOneString()
        {
            // Arrange
            DBConnection testedObject = new DBConnection();

            //Act
            String testedList = testedObject.DatabaseDataGetOne("SELECT * FROM 'category'");

            //Assert
            Assert.IsInstanceOfType(testedList, typeof(String));
        }
    }
}
