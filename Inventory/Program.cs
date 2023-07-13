using System.IO;
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
    private static void ReadInventoryFile()
    {
        foreach (string line in File.ReadLines(@"C:\Users\pedro\source\repos\Inventory\Inventory\inventory.txt"))
        {
            string[] splittedLine = line.Split(' ');

            Product currentProduct = new Product(Int32.Parse(splittedLine[0]), splittedLine[1], Int32.Parse(splittedLine[2]), Int32.Parse(splittedLine[3]));
            _products.Add(currentProduct);
        }
    }

    public static int GenerateId()
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

    public static void AddProduct(Product newProduct)
    {
        // Add file to class' products
        _products.Add(newProduct);

        // Add product to inventory file
        if (!File.Exists(@"C:\Users\pedro\source\repos\Inventory\Inventory\inventory.txt"))
        {
            using (StreamWriter sw = File.CreateText(@"C:\Users\pedro\source\repos\Inventory\Inventory\inventory.txt"))
            {
                    string newLine = String.Format(
                        "{0} {1} {2} {3}",
                        newProduct._id, newProduct._name, newProduct._price, newProduct._quantity
                    );
                    sw.WriteLine(newLine);
            }
        } else
        {
            using (StreamWriter sw = File.AppendText(@"C:\Users\pedro\source\repos\Inventory\Inventory\inventory.txt"))
            {
                string newLine = String.Format(
                    "{0} {1} {2} {3}",
                    newProduct._id, newProduct._name, newProduct._price, newProduct._quantity
                );
                sw.WriteLine(newLine);
            }
        }
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