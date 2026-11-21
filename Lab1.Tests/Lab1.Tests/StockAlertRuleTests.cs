using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab1.Tests
{
    [TestClass]
    public class StockAlertRuleTests
    {
        [DataTestMethod]
        [DataRow(6, 5, false, false)] // Выше порога предупреждений нет
        [DataRow(5, 5, false, false)] // Достигли порога сверху
        [DataRow(4, 5, false, true)]  // Первое предупреждение
        [DataRow(4, 5, true, true)]  // Запас всё ещё низкий
        [DataRow(5, 5, true, true)]  // Достигли порога снизу
        [DataRow(6, 5, true, false)] // Запас пополнен выше порога
        [DataRow(0, 0, false, false)] // Уведомления выключены
        [DataRow(0, 0, true, false)] // Отключили активное предупреждение
        public void ShouldNotify_ReturnsExpectedState(
            int quantity, int threshold, bool wasActive, bool expected)
        {
            bool actual = StockAlertRule.ShouldNotify(quantity, threshold, wasActive);

            Assert.AreEqual(expected, actual);
        }
    }
}
