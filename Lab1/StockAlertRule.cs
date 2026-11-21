namespace Lab1
{
    public static class StockAlertRule
    {
        public static bool ShouldNotify(int quantity, int threshold, bool wasActive)
        {
            if (threshold <= 0)
            {
                return false;
            }

            if (quantity == threshold)
            {
                return wasActive;
            }

            return quantity < threshold;
        }
    }
}