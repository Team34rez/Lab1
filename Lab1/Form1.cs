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
        private readonly string inventoryFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Lab1",
            "inventory.xml");

        private readonly XmlSerializer serializer =
            new XmlSerializer(typeof(List<InventoryItem>));

        public Form1()
        {
            InitializeComponent();
            LoadInventory();
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

                MessageBox.Show(
                    "Не удалось загрузить товары. Операции изменения отключены.\n\n" +
                    inventoryFile + "\n\n" + ex.Message,
                    "Ошибка загрузки");
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
                MessageBox.Show(
                    "Изменения есть в окне программы, но сохранить их в файл не удалось.\n\n" +
                    ex.Message,
                    "Ошибка сохранения");
            }
        }

        private void addItemButton_Click(object sender, EventArgs e)
        {
            string name = nameTextBox.Text.Trim();
            string category = categoryTextBox.Text.Trim();

            if (name.Length == 0 || category.Length == 0)
            {
                MessageBox.Show("Введите название и категорию товара.");
                return;
            }

            if (!int.TryParse(quantityTextBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Количество должно быть целым числом от нуля.");
                return;
            }

            if (!decimal.TryParse(priceTextBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите неотрицательную цену.");
                return;
            }

            var item = new InventoryItem(name, quantity, price, category);
            itemsListBox.Items.Add(item);
            SaveInventory();
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
            priceTextBox.Text = item.Price.ToString("0.00");
            categoryTextBox.Text = item.Category;
        }

        private void updateItemButton_Click(object sender, EventArgs e)
        {
            int index = itemsListBox.SelectedIndex;

            if (index < 0)
            {
                MessageBox.Show("Выберите товар в списке.");
                return;
            }

            string name = nameTextBox.Text.Trim();
            string category = categoryTextBox.Text.Trim();

            if (name.Length == 0 || category.Length == 0)
            {
                MessageBox.Show("Введите название и категорию товара.");
                return;
            }

            if (!int.TryParse(quantityTextBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Количество должно быть целым числом от нуля.");
                return;
            }

            if (!decimal.TryParse(priceTextBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите неотрицательную цену.");
                return;
            }

            itemsListBox.Items[index] = new InventoryItem(name, quantity, price, category);
            SaveInventory();
        }

        private void removeItemButton_Click(object sender, EventArgs e)
        {
            int index = itemsListBox.SelectedIndex;

            if (index < 0)
            {
                MessageBox.Show("Выберите товар для удаления.");
                return;
            }

            var item = (InventoryItem)itemsListBox.Items[index];

            var answer = MessageBox.Show(
                $"Удалить товар «{item.Name}»?",
                "Удаление товара",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (answer == DialogResult.Yes)
            {
                itemsListBox.Items.RemoveAt(index);
                SaveInventory();
            }
        }
    }
}
