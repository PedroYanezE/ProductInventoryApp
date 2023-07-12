public class Inventory
{
    private static Product[] products = Array.Empty<Product>();

    public Inventory()
    {
        // products = ReadFile();
    }

    class Product
    {
        public int _id;
        private string? _name;
        private int _price;
        private int _quantity;

        public Product(string? name, int price, int quantity)
        {
            _id = GenerateId();
            _name = name;
            _price = price;
            _quantity = quantity;
        }
    }

    protected static int GenerateId()
    {
        int maxId = 0;
        for (int i = 0; i < products.Length; i++)
        {
            if (products[i]._id > maxId)
            {
                maxId = products[i]._id;
            }
        }

        return maxId;
    }

    // TODO: Reads the "inventory.txt", then updates de products array accordingly.
    private static Product[] ReadFile()
    {
        return Array.Empty<Product>();
    }
}