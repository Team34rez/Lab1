namespace Lab1
{
    public class InventoryItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }

        public InventoryItem()
        {
        }
        public InventoryItem(string name, int quantity, decimal price, string category)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
            Category = category;

        }
        public override string ToString()
        {
            return $"{Name} — {Quantity} шт. — {Price:0.00} руб. — {Category}";
        }
    }
}
