using System.Text;

class Inventory
{
    private static List<Product> _products = new();
    public List<Product> products
    {
        get { return _products; }
    }

    public Inventory()
    {
        // Populate products list
        ReadInventoryFile();
    }

    public class Product
    {
        public int _id;
        public string? _name;
        public int _price;
        public int _quantity;

        public Product(int id, string? name, int price, int quantity)
        {
            _id = id;
            _name = name;
            _price = price;
            _quantity = quantity;
        }
    }

    protected static int GenerateId()
    {
        int maxId = 0;
        for (int i = 0; i < _products.Count; i++)
        {
            if (_products[i]._id > maxId)
            {
                maxId = _products[i]._id;
            }
        }

        return maxId;
    }

    public static void ReadInventoryFile()
    {
        Console.WriteLine("READING INVENTORY");
        foreach (string line in File.ReadLines(@"C:\Users\pedro\source\repos\Inventory\Inventory\inventory.txt"))
        {
            string[] splittedLine = line.Split(' ');

            Product currentProduct = new Product(Int32.Parse(splittedLine[0]), splittedLine[1], Int32.Parse(splittedLine[2]), Int32.Parse(splittedLine[3]));
            _products.Add(currentProduct);
        }
        Console.WriteLine("FINISHED READING INVENTORY");
    }
}


class Program
{
    static void Main(string[] args)
    {
        // Check that products is being initialized correctly
        Inventory inv = new Inventory();
        for(int i = 0; i < inv.products.Count(); i++)
        {
            Console.WriteLine(inv.products[i]._id.ToString() + " " + inv.products[i]._name + " " + inv.products[i]._price + " " + inv.products[i]._quantity);
        }
    }
}