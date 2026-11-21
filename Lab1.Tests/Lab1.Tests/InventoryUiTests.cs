using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab1.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class InventoryUiTests
    {
        private const string Hammer = "ЛР3 — Молоток";
        private const string Bolts = "ЛР5 — Болты";
        private const string LowStock = "Низкий запас";
        private const string BadName = "Введите название и категорию товара.";
        private const string BadQuantity = "Количество должно быть целым числом от нуля.";
        private const string BadPrice = "Введите неотрицательную цену.";
        private static readonly TimeSpan UiTimeout = TimeSpan.FromSeconds(8);

        private string folder;
        private string InventoryFile => Path.Combine(folder, "inventory.xml");
        private Application app;
        private UIA3Automation automation;
        private Window window;

        [TestInitialize]
        public void Prepare()
        {
            folder = Path.Combine(Path.GetTempPath(), "Lab1-UI-" + Guid.NewGuid());
            Directory.CreateDirectory(folder);
            automation = new UIA3Automation();
        }

        private void Start(bool waitForMainWindow = true)
        {
            var info = new ProcessStartInfo(typeof(InventoryItem).Assembly.Location)
            {
                UseShellExecute = false
            };
            info.EnvironmentVariables["LAB1_TEST_INVENTORY_FILE"] = InventoryFile;
            app = Application.Launch(info);
            if (waitForMainWindow)
                WaitForMainWindow();
        }

        private void WaitForMainWindow()
        {
            window = Retry.WhileNull(() => app.GetAllTopLevelWindows(automation)
                .FirstOrDefault(w => w.Name == "Управление инвентарём"), UiTimeout).Result;
            Assert.IsNotNull(window, "Не открылась главная форма Lab1.");
        }

        private void Stop()
        {
            if (app == null) return;
            try
            {
                if (!app.HasExited) app.Close();
            }
            finally
            {
                if (!app.HasExited) app.Kill();
                app.Dispose();
                app = null;
                window = null;
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { Stop(); }
            finally
            {
                automation?.Dispose();
                if (folder != null && Directory.Exists(folder))
                {
                    File.Delete(InventoryFile);
                    File.Delete(InventoryFile + ".tmp");
                    Directory.Delete(folder);
                }
            }
        }

        private AutomationElement Find(string id)
        {
            var control = window.FindFirstDescendant(cf => cf.ByAutomationId(id));
            Assert.IsNotNull(control, "На форме нет элемента " + id);
            return control;
        }

        private ListBox Items => Find("itemsListBox").AsListBox();
        private TextBox Field(string id) => Find(id).AsTextBox();

        private static void Wait(Func<bool> condition, string failure)
        {
            Assert.IsTrue(Retry.WhileFalse(condition, UiTimeout).Success, failure);
        }

        private void Fill(string name, string quantity, string price, string category)
        {
            Field("nameTextBox").Text = name;
            Field("quantityTextBox").Text = quantity;
            Field("priceTextBox").Text = price;
            Field("categoryTextBox").Text = category;
        }

        private void SetThreshold(int value)
        {
            var threshold = Find("stockThresholdInput");
            var editor = threshold.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            if (editor != null)
                editor.AsTextBox().Text = value.ToString(CultureInfo.InvariantCulture);
            else
                threshold.AsSpinner().Value = value;

            // Завершаем ввод в NumericUpDown до вызова обработчика кнопки.
            Field("nameTextBox").Focus();
        }

        private int GetThreshold()
        {
            var threshold = Find("stockThresholdInput");
            var editor = threshold.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            return editor == null ? (int)threshold.AsSpinner().Value
                : int.Parse(editor.AsTextBox().Text, CultureInfo.CurrentCulture);
        }

        private void Press(string id) => Find(id).AsButton().Invoke();

        private void ExpectRow(string name, int quantity, int price, string category)
        {
            string pattern = "^" + Regex.Escape(name) + " — " + quantity + " шт\\. — "
                + price + "[.,]00 руб\\. — " + Regex.Escape(category) + "$";
            Wait(() =>
            {
                var rows = Items.Items;
                return rows.Length == 1 && Regex.IsMatch(rows[0].Name, pattern);
            }, "В списке ожидалась одна строка: " + name + ", количество " + quantity
                + ", цена " + price + ", категория " + category);
        }

        private void AddHammer()
        {
            Fill(Hammer, "5", "125", "Инструменты");
            Press("addItemButton");
            ExpectRow(Hammer, 5, 125, "Инструменты");
        }

        private void EditHammer()
        {
            AddHammer();
            Items.Items[0].Click();
            Fill(Hammer, "8", "150", "Ручной инструмент");
            Press("updateItemButton");
            ExpectRow(Hammer, 8, 150, "Ручной инструмент");
        }

        // Ищем стандартный диалог только в потоках запущенного экземпляра Lab1.
        private delegate bool WindowCallback(IntPtr handle, IntPtr parameter);

        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(uint threadId, WindowCallback callback, IntPtr parameter);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr handle, StringBuilder buffer, int length);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr handle);

        private Window CurrentDialog()
        {
            using (var process = Process.GetProcessById(app.ProcessId))
            {
                foreach (ProcessThread thread in process.Threads)
                {
                    IntPtr found = IntPtr.Zero;
                    EnumThreadWindows((uint)thread.Id, (handle, unused) =>
                    {
                        var name = new StringBuilder(64);
                        GetClassName(handle, name, name.Capacity);
                        if (!IsWindowVisible(handle) || name.ToString() != "#32770")
                            return true;
                        found = handle;
                        return false;
                    }, IntPtr.Zero);
                    if (found != IntPtr.Zero)
                        return automation.FromHandle(found).AsWindow();
                }
            }
            return null;
        }

        private Window WaitForDialog(TimeSpan timeout)
        {
            var dialog = Retry.WhileNull(CurrentDialog, timeout, ignoreException: true).Result;
            Assert.IsNotNull(dialog, "Не появилось ожидаемое окно сообщения.");
            return dialog;
        }

        private void Dismiss(string text, string buttonId = "1", string title = null,
            int timeoutSeconds = 8)
        {
            var dialog = WaitForDialog(TimeSpan.FromSeconds(timeoutSeconds));
            if (title != null) Assert.AreEqual(title, dialog.Name);
            var message = dialog.FindAllDescendants()
                .Any(e => e.ControlType == ControlType.Text && e.Name.Contains(text));
            Assert.IsTrue(message, "В окне отсутствует текст: " + text);

            var button = dialog.FindFirstDescendant(cf => cf.ByAutomationId(buttonId));
            if (button == null)
            {
                string[] names = buttonId == "6" ? new[] { "Да", "Yes" }
                    : buttonId == "7" ? new[] { "Нет", "No" }
                    : new[] { "OK", "ОК" };
                button = dialog.FindAllDescendants(cf => cf.ByControlType(ControlType.Button))
                    .FirstOrDefault(b => names.Contains(b.Name.Replace("&", ""),
                        StringComparer.OrdinalIgnoreCase));
            }
            Assert.IsNotNull(button, "В диалоге отсутствует кнопка с ID " + buttonId);
            button.AsButton().Invoke();
            Wait(() => CurrentDialog() == null, "Окно сообщения не закрылось.");
        }

        private void PressAndDismiss(string id, string text, string buttonId = "1",
            string title = null)
        {
            Find(id).AsButton().Click();
            Dismiss(text, buttonId, title);
        }

        private void ExpectNoAlert()
        {
            var clock = Stopwatch.StartNew();
            do
            {
                Assert.IsNull(CurrentDialog(), "Появилось неожиданное уведомление.");
                Thread.Sleep(200);
            } while (clock.Elapsed < TimeSpan.FromSeconds(12));
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-01. Добавление товара")]
        public void AddItem_ShowsNewItemInList()
        {
            Start();
            Assert.AreEqual(0, Items.Items.Length);
            AddHammer();
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-02. Выбор товара")]
        public void SelectItem_FillsAllFields()
        {
            Start();
            AddHammer();
            Fill("Гвозди", "2", "50", "Крепёж");
            Press("addItemButton");
            Wait(() => Items.Items.Length == 2, "Второй товар не появился.");
            Items.Items[1].Click();
            Items.Items[0].Click();
            Wait(() => Field("nameTextBox").Text == Hammer, "Название не загрузилось.");
            Assert.AreEqual("5", Field("quantityTextBox").Text);
            StringAssert.Matches(Field("priceTextBox").Text, new Regex(@"^125[.,]00$"));
            Assert.AreEqual("Инструменты", Field("categoryTextBox").Text);
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-03. Редактирование товара")]
        public void EditItem_UpdatesExistingRow()
        {
            Start();
            EditHammer();
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-04. Загрузка изменений")]
        public void Restart_RestoresEditedItem()
        {
            Start();
            EditHammer();
            Stop();
            Start();
            ExpectRow(Hammer, 8, 150, "Ручной инструмент");
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-05. Пустое название или категория")]
        public void Add_RejectsMissingNameAndCategory()
        {
            Start();
            Fill("", "5", "125", "Инструменты");
            PressAndDismiss("addItemButton", BadName);
            Assert.AreEqual(0, Items.Items.Length);
            Fill(Hammer, "5", "125", "");
            PressAndDismiss("addItemButton", BadName);
            Assert.AreEqual(0, Items.Items.Length);
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-06. Некорректное количество")]
        public void Add_RejectsInvalidQuantity()
        {
            Start();
            foreach (string invalid in new[] { "-1", "abc" })
            {
                Fill(Hammer, invalid, "125", "Инструменты");
                PressAndDismiss("addItemButton", BadQuantity);
                Assert.AreEqual(0, Items.Items.Length);
            }
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-07. Некорректная цена")]
        public void Edit_RejectsInvalidPriceAndKeepsOriginal()
        {
            Start();
            EditHammer();
            Items.Items[0].Click();
            foreach (string invalid in new[] { "-1", "abc" })
            {
                Field("priceTextBox").Text = invalid;
                PressAndDismiss("updateItemButton", BadPrice);
                ExpectRow(Hammer, 8, 150, "Ручной инструмент");
            }
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-08. Нулевые значения")]
        public void Add_AcceptsZeroQuantityAndPrice()
        {
            Start();
            Fill("ЛР3 — Нулевая позиция", "0", "0", "Тест");
            Press("addItemButton");
            ExpectRow("ЛР3 — Нулевая позиция", 0, 0, "Тест");
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-09. Действия без выбора")]
        public void NoSelection_ShowsMessagesAndKeepsList()
        {
            Start();
            AddHammer();
            Stop();
            Start();
            PressAndDismiss("updateItemButton", "Выберите товар в списке.");
            PressAndDismiss("removeItemButton", "Выберите товар для удаления.");
            ExpectRow(Hammer, 5, 125, "Инструменты");
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-10. Отмена и подтверждение удаления")]
        public void Delete_RespectsConfirmationAndPersists()
        {
            Start();
            AddHammer();
            Items.Items[0].Click();
            string prompt = "Удалить товар «" + Hammer + "»?";
            PressAndDismiss("removeItemButton", prompt, "7", "Удаление товара");
            ExpectRow(Hammer, 5, 125, "Инструменты");
            PressAndDismiss("removeItemButton", prompt, "6", "Удаление товара");
            Wait(() => Items.Items.Length == 0, "Строка не удалена.");
            Stop();
            Start();
            Assert.AreEqual(0, Items.Items.Length);
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-11. Файл отсутствует")]
        public void MissingXml_OpensWithEmptyInventory()
        {
            Assert.IsFalse(File.Exists(InventoryFile));
            Start();
            Assert.IsNull(CurrentDialog());
            Assert.AreEqual(0, Items.Items.Length);
            Assert.IsTrue(Find("addItemButton").IsEnabled);
        }

        [TestMethod, TestCategory("ЛР3"), Description("ЛР3-12. Повреждённый XML")]
        public void BrokenXml_ShowsErrorAndProtectsFile()
        {
            File.WriteAllText(InventoryFile, "<broken>");
            Start(false);
            Dismiss("Не удалось загрузить товары", title: "Ошибка загрузки");
            WaitForMainWindow();
            foreach (string id in new[] { "addItemButton", "updateItemButton", "removeItemButton" })
                Assert.IsFalse(Find(id).IsEnabled, id);
            Stop();
            Assert.AreEqual("<broken>", File.ReadAllText(InventoryFile));
        }

        private void AddBolts()
        {
            Fill(Bolts, "6", "10", "Крепёж");
            SetThreshold(5);
            Press("addItemButton");
            ExpectRow(Bolts, 6, 10, "Крепёж");
            Items.Items[0].Click();
        }

        private void UpdateBolts(int quantity, bool expectAlert)
        {
            Items.Items[0].Click();
            Field("quantityTextBox").Text = quantity.ToString(CultureInfo.InvariantCulture);
            if (expectAlert)
                PressAndDismiss("updateItemButton", AlertText(quantity), title: LowStock);
            else
                Press("updateItemButton");
            ExpectRow(Bolts, quantity, 10, "Крепёж");
        }

        private static string AlertText(int quantity) => Bolts + " — осталось " + quantity + " шт.";

        private void ReduceBelowThreshold()
        {
            AddBolts();
            UpdateBolts(5, false);
            UpdateBolts(4, true);
        }

        [TestMethod, TestCategory("ЛР5"), Description("ЛР5-01. Сохранение порога")]
        public void Threshold_IsEditableAndPersists()
        {
            Start();
            AddBolts();
            Assert.AreEqual(5, GetThreshold());
            SetThreshold(3);
            Press("updateItemButton");
            Stop();
            Start();
            ExpectRow(Bolts, 6, 10, "Крепёж");
            Items.Items[0].Click();
            Wait(() => GetThreshold() == 3, "Порог 3 не восстановился.");
        }

        [TestMethod, TestCategory("ЛР5"), Description("ЛР5-02. Снижение до порога")]
        public void AtThreshold_DoesNotStartAlert()
        {
            Start();
            AddBolts();
            UpdateBolts(5, false);
            ExpectNoAlert();
        }

        [TestMethod, TestCategory("ЛР5"), Description("ЛР5-03. Снижение ниже порога")]
        public void BelowThreshold_ShowsNameAndQuantity()
        {
            Start();
            ReduceBelowThreshold();
        }

        [TestMethod, TestCategory("ЛР5"), Description("ЛР5-04. Повтор уведомления")]
        public void LowStock_RepeatsAfterClosingNotification()
        {
            Start();
            ReduceBelowThreshold();
            Dismiss(AlertText(4), title: LowStock, timeoutSeconds: 15);
        }

        [TestMethod, TestCategory("ЛР5"), Description("ЛР5-05. Пополнение до порога")]
        public void RestockToThreshold_KeepsAlertWithNewQuantity()
        {
            Start();
            ReduceBelowThreshold();
            UpdateBolts(5, true);
            Dismiss(AlertText(5), title: LowStock, timeoutSeconds: 15);
        }

        [TestMethod, TestCategory("ЛР5"), Description("ЛР5-06. Пополнение выше порога")]
        public void RestockAboveThreshold_StopsAlerts()
        {
            Start();
            ReduceBelowThreshold();
            UpdateBolts(5, true);
            UpdateBolts(6, false);
            ExpectNoAlert();
        }
    }
}



