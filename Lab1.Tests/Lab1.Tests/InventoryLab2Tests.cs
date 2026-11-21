using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab1.Tests
{
    [TestClass]
    public class InventoryItemInputTests
    {
        [TestMethod]
        public void TryCreate_AllowsZeroAndTrimsText()
        {
            bool success = InventoryItemInput.TryCreate(
                "  Молоток  ", "0", "0", "  Инструменты  ",
                out InventoryItem item, out string error);

            Assert.IsTrue(success, error);
            Assert.IsNull(error);
            Assert.AreEqual("Молоток", item.Name);
            Assert.AreEqual(0, item.Quantity);
            Assert.AreEqual(0m, item.Price);
            Assert.AreEqual("Инструменты", item.Category);
        }

        [DataTestMethod]
        [DataRow(" ", "5", "100", "Инструменты", "Введите название и категорию товара.")]
        [DataRow("Молоток", "5", "100", "", "Введите название и категорию товара.")]
        [DataRow("Молоток", "-1", "100", "Инструменты", "Количество должно быть целым числом от нуля.")]
        [DataRow("Молоток", "три", "100", "Инструменты", "Количество должно быть целым числом от нуля.")]
        [DataRow("Молоток", "1.5", "100", "Инструменты", "Количество должно быть целым числом от нуля.")]
        [DataRow("Молоток", "2147483648", "100", "Инструменты", "Количество должно быть целым числом от нуля.")]
        [DataRow("Молоток", "5", "-1", "Инструменты", "Введите неотрицательную цену.")]
        [DataRow("Молоток", "5", "сто", "Инструменты", "Введите неотрицательную цену.")]
        public void TryCreate_RejectsInvalidInput(
            string name, string quantity, string price, string category, string expectedError)
        {
            bool success = InventoryItemInput.TryCreate(
                name, quantity, price, category, out InventoryItem item, out string error);

            Assert.IsFalse(success);
            Assert.IsNull(item);
            Assert.AreEqual(expectedError, error);
        }
    }

    [TestClass]
    public class InventoryFormTests
    {
        [TestMethod]
        public void FirstLaunch_ShowsEnabledControlsAndEmptyInventory()
        {
            WithTemporaryInventory(path =>
            {
                using (var form = OpenForm(path, item => true))
                {
                    form.Show();

                    foreach (string name in new[]
                    {
                        "nameTextBox", "quantityTextBox", "priceTextBox", "categoryTextBox",
                        "itemsListBox", "addItemButton", "updateItemButton", "removeItemButton"
                    })
                    {
                        var control = FindControl<Control>(form, name);
                        Assert.IsTrue(control.Visible, name);
                        Assert.IsTrue(control.Enabled, name);
                    }

                    Assert.AreEqual(0, GetItems(form).Items.Count);
                    Assert.IsFalse(File.Exists(path));
                }
            });
        }

        [TestMethod]
        public void AddItem_SavesAllFieldsAndRestoresThemOnNextLaunch()
        {
            WithTemporaryInventory(path =>
            {
                using (var form = OpenForm(path, item => true))
                {
                    form.Show();
                    EnterItem(form, "Молоток", 5, 125.50m, "Инструменты");
                    FindControl<Button>(form, "addItemButton").PerformClick();

                    Assert.AreEqual(1, GetItems(form).Items.Count);
                    Assert.IsTrue(File.Exists(path));
                }

                using (var reloaded = OpenForm(path, item => true))
                {
                    var items = GetItems(reloaded);
                    Assert.AreEqual(1, items.Items.Count);
                    var item = (InventoryItem)items.Items[0];
                    Assert.AreEqual("Молоток", item.Name);
                    Assert.AreEqual(5, item.Quantity);
                    Assert.AreEqual(125.50m, item.Price);
                    Assert.AreEqual("Инструменты", item.Category);
                }
            });
        }

        [TestMethod]
        public void SelectAndEditItem_UpdatesFieldsAndPersistsChanges()
        {
            WithTemporaryInventory(path =>
            {
                using (var form = OpenForm(path, item => true))
                {
                    form.Show();
                    EnterItem(form, "Молоток", 5, 125m, "Инструменты");
                    FindControl<Button>(form, "addItemButton").PerformClick();
                    var items = GetItems(form);
                    items.ClearSelected();
                    EnterItem(form, "", 0, 0m, "");
                    items.SelectedIndex = 0;

                    Assert.AreEqual("Молоток", FindControl<TextBox>(form, "nameTextBox").Text);
                    Assert.AreEqual("5", FindControl<TextBox>(form, "quantityTextBox").Text);
                    Assert.AreEqual(125m.ToString("0.00"), FindControl<TextBox>(form, "priceTextBox").Text);
                    Assert.AreEqual("Инструменты", FindControl<TextBox>(form, "categoryTextBox").Text);

                    EnterItem(form, "Кисть", 0, 0m, "Расходники");
                    FindControl<Button>(form, "updateItemButton").PerformClick();
                    Assert.AreEqual("Кисть", ((InventoryItem)items.Items[0]).Name);
                }

                using (var reloaded = OpenForm(path, item => true))
                {
                    var items = GetItems(reloaded);
                    Assert.AreEqual(1, items.Items.Count);
                    var item = (InventoryItem)items.Items[0];
                    Assert.AreEqual("Кисть", item.Name);
                    Assert.AreEqual(0, item.Quantity);
                    Assert.AreEqual(0m, item.Price);
                    Assert.AreEqual("Расходники", item.Category);
                }
            });
        }

        [TestMethod]
        public void RemoveItem_RespectsConfirmationAndPersistsDeletion()
        {
            WithTemporaryInventory(path =>
            {
                bool confirmDeletion = false;
                using (var form = OpenForm(path, item => confirmDeletion))
                {
                    form.Show();
                    EnterItem(form, "Молоток", 5, 125m, "Инструменты");
                    FindControl<Button>(form, "addItemButton").PerformClick();
                    var items = GetItems(form);
                    items.SelectedIndex = 0;
                    var removeButton = FindControl<Button>(form, "removeItemButton");

                    removeButton.PerformClick();
                    Assert.AreEqual(1, items.Items.Count);

                    confirmDeletion = true;
                    removeButton.PerformClick();
                    Assert.AreEqual(0, items.Items.Count);
                }

                using (var reloaded = OpenForm(path, item => true))
                {
                    Assert.AreEqual(0, GetItems(reloaded).Items.Count);
                }
            });
        }

        [TestMethod]
        public void LoadInvalidXml_DisablesEditingAndKeepsFile()
        {
            WithTemporaryInventory(path =>
            {
                const string invalidXml = "<broken>";
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, invalidXml);
                string noticeTitle = null;

                using (var form = new Form1(path, item => true, (message, title) => noticeTitle = title))
                {
                    form.Show();

                    Assert.AreEqual("Ошибка загрузки", noticeTitle);
                    Assert.IsFalse(FindControl<Button>(form, "addItemButton").Enabled);
                    Assert.IsFalse(FindControl<Button>(form, "updateItemButton").Enabled);
                    Assert.IsFalse(FindControl<Button>(form, "removeItemButton").Enabled);
                    Assert.AreEqual(invalidXml, File.ReadAllText(path));
                }
            });
        }
        [TestMethod]
        public void EditQuantity_PreservesOriginalPriceInXml()
        {
            WithTemporaryInventory(path =>
            {
                const decimal originalPrice = 125.567m;

                using (var form = OpenForm(path, item => true))
                {
                    form.Show();
                    FindControl<TextBox>(form, "nameTextBox").Text = "Проверка цены";
                    FindControl<TextBox>(form, "quantityTextBox").Text = "5";
                    FindControl<TextBox>(form, "priceTextBox").Text = originalPrice.ToString();
                    FindControl<TextBox>(form, "categoryTextBox").Text = "Тест";
                    FindControl<Button>(form, "addItemButton").PerformClick();

                    var items = GetItems(form);
                    Assert.AreEqual(1, items.Items.Count);
                    Assert.AreEqual(originalPrice, ((InventoryItem)items.Items[0]).Price);

                    items.SelectedIndex = 0;
                    FindControl<TextBox>(form, "quantityTextBox").Text = "8";
                    FindControl<Button>(form, "updateItemButton").PerformClick();
                }

                using (var reloaded = OpenForm(path, item => true))
                {
                    var item = (InventoryItem)GetItems(reloaded).Items[0];
                    Assert.AreEqual(8, item.Quantity);
                    Assert.AreEqual(originalPrice, item.Price);
                }
            });
        }
        [TestMethod]
        public void StockAlert_AtThresholdAfterRestock_ContinuesAfterRestart()
        {
            WithTemporaryInventory(path =>
            {
                string notice = null;
                Action<string, string> captureNotice = (message, title) =>
                {
                    Assert.AreEqual("Низкий запас", title);
                    notice = message;
                };

                using (var form = new Form1(path, item => true, captureNotice))
                {
                    form.Show();
                    EnterItem(form, "Болты", 4, 10m, "Крепёж");
                    FindControl<NumericUpDown>(form, "stockThresholdInput").Value = 5;
                    FindControl<Button>(form, "addItemButton").PerformClick();
                    Assert.AreEqual("Болты — осталось 4 шт.", notice);

                    GetItems(form).SelectedIndex = 0;
                    FindControl<TextBox>(form, "quantityTextBox").Text = "5";
                    FindControl<Button>(form, "updateItemButton").PerformClick();
                    Assert.AreEqual("Болты — осталось 5 шт.", notice);
                }

                notice = null;
                using (var reloaded = new Form1(path, item => true, captureNotice))
                {
                    reloaded.Show();
                    Application.DoEvents(); // Даём форме обработать событие Shown

                    Assert.AreEqual("Болты — осталось 5 шт.", notice);
                    GetItems(reloaded).SelectedIndex = 0;
                    Assert.AreEqual(5m,
                        FindControl<NumericUpDown>(reloaded, "stockThresholdInput").Value);
                }
            });
        }
        private static Form1 OpenForm(string path, Func<InventoryItem, bool> confirmRemoval)
        {
            return new Form1(path, confirmRemoval, (message, title) => Assert.Fail(message));
        }

        private static T FindControl<T>(Form1 form, string name) where T : Control
        {
            var controls = form.Controls.Find(name, true);
            Assert.AreEqual(1, controls.Length, name);
            Assert.IsInstanceOfType(controls[0], typeof(T), name);
            return (T)controls[0];
        }

        private static ListBox GetItems(Form1 form)
        {
            return FindControl<ListBox>(form, "itemsListBox");
        }

        private static void EnterItem(
            Form1 form, string name, int quantity, decimal price, string category)
        {
            FindControl<TextBox>(form, "nameTextBox").Text = name;
            FindControl<TextBox>(form, "quantityTextBox").Text = quantity.ToString();
            FindControl<TextBox>(form, "priceTextBox").Text = price.ToString("0.00");
            FindControl<TextBox>(form, "categoryTextBox").Text = category;
        }

        private static void WithTemporaryInventory(Action<string> test)
        {
            string folder = Path.Combine(Path.GetTempPath(), "Lab1.Tests-" + Guid.NewGuid());
            string path = Path.Combine(folder, "inventory.xml");
            ExceptionDispatchInfo failure = null;

            // WinForms requires an STA thread. MSTest 2.2.10 runs the test body
            // on a worker thread whose apartment state we do not control.
            var thread = new Thread(() =>
            {
                try
                {
                    test(path);
                }
                catch (Exception ex)
                {
                    failure = ExceptionDispatchInfo.Capture(ex);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);

            try
            {
                thread.Start();
                thread.Join();
                failure?.Throw();
            }
            finally
            {
                if (Directory.Exists(folder))
                {
                    File.Delete(path);
                    File.Delete(path + ".tmp");
                    Directory.Delete(folder);
                }
            }
        }
    }
}

