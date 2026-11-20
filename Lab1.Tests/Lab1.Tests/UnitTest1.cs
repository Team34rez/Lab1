using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab1.Tests
{
    [TestClass]
    public class InventoryItemTests
    {
        [TestMethod]
        public void Constructor_AssignsAllProperties()
        {
            var item = new InventoryItem("Молоток", 5, 125.50m, "Инструменты");

            Assert.AreEqual("Молоток", item.Name);
            Assert.AreEqual(5, item.Quantity);
            Assert.AreEqual(125.50m, item.Price);
            Assert.AreEqual("Инструменты", item.Category);
        }
        [TestMethod]
        public void DefaultConstructor_CreatesItemWithDefaultValues()
        {
            var item = new InventoryItem();

            Assert.IsNull(item.Name);
            Assert.AreEqual(0, item.Quantity);
            Assert.AreEqual(0m, item.Price);
            Assert.IsNull(item.Category);
        }

        [TestMethod]
        public void ToString_FormatsItemForInventoryList()
        {
            var item = new InventoryItem("Молоток", 5, 125.5m, "Инструменты");
            var separator = System.Globalization.CultureInfo.CurrentCulture
                .NumberFormat.NumberDecimalSeparator;

            var result = item.ToString();

            Assert.AreEqual(
                $"Молоток — 5 шт. — 125{separator}50 руб. — Инструменты",
                result);
        }
    }
}
