using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator;

namespace SimpleCalcTestProject
{
    /*  A small number of tests to cover basic functionality.
     *  Please select 'Test' and 'Run all tests' to run.
     */

    [TestClass]
    public class CalculatorTests
    {        
        [TestMethod]
        public void AddTest()
        {
            // Arrange
            SimpleCalculator sc = new SimpleCalculator();
            // Act
            decimal added = sc.CalculateTwoDecimals("add", 3, 2);
            // Assert
            Assert.AreEqual(5, added);
        }

        [TestMethod]
        public void SubtractTest()
        {
            // Arrange
            SimpleCalculator sc = new SimpleCalculator();
            // Act
            decimal subtracted = sc.CalculateTwoDecimals("subtract", 3, 2);
            // Assert
            Assert.AreEqual(1, subtracted);
        }

        [TestMethod]
        public void MultiplyTest()
        {
            // Arrange
            SimpleCalculator sc = new SimpleCalculator();
            // Act
            decimal multiplied = sc.CalculateTwoDecimals("multiply", 3, 2);
            // Assert
            Assert.AreEqual(6, multiplied);
        }
    }
}
