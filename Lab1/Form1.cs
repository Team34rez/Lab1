using System;
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
        public Form1()
        {
            InitializeComponent();
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
            }
        }
    }
}
