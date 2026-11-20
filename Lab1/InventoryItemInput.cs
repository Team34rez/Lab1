namespace Lab1
{
    public static class InventoryItemInput
    {
        public static bool TryCreate(
            string nameText, string quantityText, string priceText, string categoryText,
            out InventoryItem item, out string error)
        {
            item = null;
            string name = nameText.Trim();
            string category = categoryText.Trim();

            if (name.Length == 0 || category.Length == 0)
            {
                error = "Введите название и категорию товара.";
                return false;
            }

            if (!int.TryParse(quantityText, out int quantity) || quantity < 0)
            {
                error = "Количество должно быть целым числом от нуля.";
                return false;
            }

            if (!decimal.TryParse(priceText, out decimal price) || price < 0)
            {
                error = "Введите неотрицательную цену.";
                return false;
            }

            item = new InventoryItem(name, quantity, price, category);
            error = null;
            return true;
        }
    }
}
