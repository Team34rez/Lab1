using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Lab1
{
    public partial class Form1 : Form
    {
        private readonly System.Windows.Forms.Timer stockAlertTimer;
        private readonly string inventoryFile;
        private readonly Func<InventoryItem, bool> confirmRemoval;
        private readonly Action<string, string> showNotice;

        private readonly XmlSerializer serializer =
            new XmlSerializer(typeof(List<InventoryItem>));

        public Form1()
            : this(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Lab1",
                    "inventory.xml"),
                ConfirmRemoval,
                ShowNotice)
        {
        }

        public Form1(
            string inventoryFile,
            Func<InventoryItem, bool> confirmRemoval,
            Action<string, string> showNotice)
        {
            this.inventoryFile = inventoryFile;
            this.confirmRemoval = confirmRemoval;
            this.showNotice = showNotice;
            InitializeComponent();
            LoadInventory();
            if (components == null)
            {
                components = new System.ComponentModel.Container();
            }

            stockAlertTimer = new System.Windows.Forms.Timer(components);
            stockAlertTimer.Interval = 10000;
            stockAlertTimer.Tick += (sender, e) => ShowStockAlerts();
            Shown += (sender, e) => ShowStockAlerts();
        }

        private void LoadInventory()
        {
            // При первом запуске файла ещё нет — список пуст.
            if (!File.Exists(inventoryFile))
            {
                return;
            }

            try
            {
                using (var stream = File.OpenRead(inventoryFile))
                {
                    var items = (List<InventoryItem>)serializer.Deserialize(stream);
                    itemsListBox.Items.AddRange(items.ToArray());
                }
            }
            catch (Exception ex)
            {
                // Защищаем старый файл от перезаписи пустым списком.
                addItemButton.Enabled = false;
                updateItemButton.Enabled = false;
                removeItemButton.Enabled = false;

                showNotice(
                    "Не удалось загрузить товары. Операции изменения отключены.\n\n" +
                    inventoryFile + "\n\n" + ex.Message,
                    "Ошибка загрузки");
            }
        }
        private void ShowStockAlerts()
        {
            stockAlertTimer.Stop();

            try
            {
                var alerts = new List<string>();

                foreach (InventoryItem item in itemsListBox.Items)
                {
                    item.StockAlertActive = StockAlertRule.ShouldNotify(
                        item.Quantity, item.StockThreshold, item.StockAlertActive);

                    if (item.StockAlertActive)
                    {
                        alerts.Add($"{item.Name} — осталось {item.Quantity} шт.");
                    }
                }

                if (alerts.Count > 0)
                {
                    showNotice(string.Join(Environment.NewLine, alerts), "Низкий запас");
                }
            }
            finally
            {
                stockAlertTimer.Start();
            }
        }
        private void SaveInventory()
        {
            string tempFile = inventoryFile + ".tmp";

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(inventoryFile));
                var items = itemsListBox.Items.Cast<InventoryItem>().ToList();

                using (var stream = File.Create(tempFile))
                {
                    serializer.Serialize(stream, items);
                }

                if (File.Exists(inventoryFile))
                {
                    File.Replace(tempFile, inventoryFile, null);
                }
                else
                {
                    File.Move(tempFile, inventoryFile);
                }
            }
            catch (Exception ex)
            {
                showNotice(
                    "Изменения есть в окне программы, но сохранить их в файл не удалось.\n\n" +
                    ex.Message,
                    "Ошибка сохранения");
            }
        }

        private void addItemButton_Click(object sender, EventArgs e)
        {
            if (!InventoryItemInput.TryCreate(
                nameTextBox.Text, quantityTextBox.Text, priceTextBox.Text, categoryTextBox.Text,
                out InventoryItem item, out string error))
            {
                showNotice(error, null);
                return;
            }
            item.StockThreshold = (int)stockThresholdInput.Value;
            item.StockAlertActive = StockAlertRule.ShouldNotify(
            item.Quantity, item.StockThreshold, false);
            itemsListBox.Items.Add(item);
            SaveInventory();
            if (item.StockAlertActive)
            {
                ShowStockAlerts();
            }
        }

        private void itemsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = itemsListBox.SelectedItem as InventoryItem;
            if (item == null)
            {
                return;
            }

            nameTextBox.Text = item.Name;
            quantityTextBox.Text = item.Quantity.ToString();
            priceTextBox.Text = item.Price.ToString("0.00##########################");
            categoryTextBox.Text = item.Category;
            stockThresholdInput.Value = item.StockThreshold;
        }

        private void updateItemButton_Click(object sender, EventArgs e)
        {
            int index = itemsListBox.SelectedIndex;

            if (index < 0)
            {
                showNotice("Выберите товар в списке.", null);
                return;
            }

            if (!InventoryItemInput.TryCreate(
                nameTextBox.Text, quantityTextBox.Text, priceTextBox.Text, categoryTextBox.Text,
                out InventoryItem item, out string error))
            {
                showNotice(error, null);
                return;
            }
            item.StockThreshold = (int)stockThresholdInput.Value;
            var previousItem = (InventoryItem)itemsListBox.Items[index];
            item.StockAlertActive = StockAlertRule.ShouldNotify(
                item.Quantity, item.StockThreshold, previousItem.StockAlertActive);
            itemsListBox.Items[index] = item;
            SaveInventory();
            if (item.StockAlertActive)
            {
                ShowStockAlerts();
            }
        }

        private void removeItemButton_Click(object sender, EventArgs e)
        {
            int index = itemsListBox.SelectedIndex;

            if (index < 0)
            {
                showNotice("Выберите товар для удаления.", null);
                return;
            }

            var item = (InventoryItem)itemsListBox.Items[index];

            if (confirmRemoval(item))
            {
                itemsListBox.Items.RemoveAt(index);
                SaveInventory();
            }
        }

        private static bool ConfirmRemoval(InventoryItem item)
        {
            return MessageBox.Show(
                $"Удалить товар «{item.Name}»?",
                "Удаление товара",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        private static void ShowNotice(string message, string title)
        {
            if (title == null)
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show(message, title);
            }
        }
    }
}
