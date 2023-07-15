namespace InventoryProject;

enum InventoryField
{
    // Enum values correspond to field position in a line of the inventory file
    ID = 0,
    NAME = 1,
    PRICE = 2,
    QUANTITY = 3
}

class Product
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

class Inventory
{
    private static List<Product> _products = new();
    private static string? _path;
    public List<Product> products
    {
        get { return _products; }
    }

    public Inventory(string path)
    {
        // Populate products list
        _path = path;
        ReadInventoryFile();
    }

    private static void ReadInventoryFile()
    {
        if(File.Exists(_path))
        {
            foreach (string line in File.ReadLines(_path))
            {
                string[] splittedLine = line.Split(' ');

                Product currentProduct = new Product(Int32.Parse(splittedLine[0]), splittedLine[1], Int32.Parse(splittedLine[2]), Int32.Parse(splittedLine[3]));
                _products.Add(currentProduct);
            }
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

        return maxId + 1;
    }

    public void AddProduct(Product newProduct, string path)
    {
        // Check if product already exists
        if(_products.Find(prod => prod._name == newProduct._name) != null)
        {
            return;
        }

        // Add file to class' products
        _products.Add(newProduct);

        // Add product to inventory file
        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                    string newLine = String.Format(
                        "{0} {1} {2} {3}",
                        newProduct._id, newProduct._name, newProduct._price, newProduct._quantity
                    );
                    sw.WriteLine(newLine);
            }
        } else
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                string newLine = String.Format(
                    "{0} {1} {2} {3}",
                    newProduct._id, newProduct._name, newProduct._price, newProduct._quantity
                );
                sw.WriteLine(newLine);
            }
        }
    }

    public void UpdateProductStock(string path, int searchField, string searchValue, int fieldToUpdate, string updatedField)
    {
        // Create a new file that will contain the updated product, then replace the original file
        // with the new one.
        // This implementation should be suitable for large files, according to:
        // https://stackoverflow.com/a/1971052/12221075
        using (StreamWriter sw = File.CreateText(path + "-temp"))
        {
            foreach (string line in File.ReadLines(path))
            {
                string[] splittedLine = line.Split(' ');
                string newLine = line;

                if (splittedLine[searchField] == searchValue)
                {
                    if (fieldToUpdate == (int)InventoryField.PRICE)
                    {
                        newLine = String.Format(
                            "{0} {1} {2} {3}",
                            splittedLine[0], splittedLine[1], updatedField, splittedLine[3]
                        );
                    }
                    else if (fieldToUpdate == (int)InventoryField.QUANTITY)
                    {
                        newLine = String.Format(
                            "{0} {1} {2} {3}",
                            splittedLine[0], splittedLine[1], splittedLine[2], updatedField
                        );
                    }
                }

                sw.WriteLine(newLine);
            }
        }

        File.Delete(path);
        File.Move(path + "-temp", path);

        // Update product in products list
        Product? productToUpdate = null;
        if(searchField == (int)InventoryField.ID)
        {
            productToUpdate = _products.Find(prod => prod._id == Int32.Parse(searchValue));
        } else if(searchField == (int)InventoryField.NAME)
        {
            productToUpdate = _products.Find(prod => prod._name == searchValue);
        }
        
        if(productToUpdate!= null )
        {
            if(fieldToUpdate == (int)InventoryField.PRICE)
            {
                productToUpdate._price = Int32.Parse(updatedField);
            } else if(fieldToUpdate == (int)InventoryField.QUANTITY)
            {
                productToUpdate._quantity = Int32.Parse(updatedField);
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string path = @"C:\Users\pedro\source\repos\Inventory\Inventory\inventory.txt";

        // "Testing"
        Console.WriteLine("Starting products:");
        Inventory inv = new(path);
        for(int i = 0; i < inv.products.Count; i++)
        {
            Console.WriteLine(inv.products[i]._id.ToString() + " " + inv.products[i]._name + " " + inv.products[i]._price + " " + inv.products[i]._quantity);
        }

        Console.WriteLine("\nAdding new product...");
        int newProductId = Inventory.GenerateId();
        Product newProduct = new(newProductId, "zapatillas_adidas", 90000, 100);
        inv.AddProduct(newProduct, path);

        Console.WriteLine("\nProducts now:");
        for (int i = 0; i < inv.products.Count; i++)
        {
            Console.WriteLine(inv.products[i]._id.ToString() + " " + inv.products[i]._name + " " + inv.products[i]._price + " " + inv.products[i]._quantity);
        }

        Console.WriteLine("\nTrying to add the same product again...");
        inv.AddProduct(newProduct, path);

        Console.WriteLine("\nProducts now:");
        for (int i = 0; i < inv.products.Count; i++)
        {
            Console.WriteLine(inv.products[i]._id.ToString() + " " + inv.products[i]._name + " " + inv.products[i]._price + " " + inv.products[i]._quantity);
        }

        Console.WriteLine("\nUpdating new product stock by name...");
        inv.UpdateProductStock(path, (int)InventoryField.NAME, "zapatillas_adidas", (int)InventoryField.QUANTITY, "50");
        Console.WriteLine("\nProducts now:");
        for (int i = 0; i < inv.products.Count; i++)
        {
            Console.WriteLine(inv.products[i]._id.ToString() + " " + inv.products[i]._name + " " + inv.products[i]._price + " " + inv.products[i]._quantity);
        }

        Console.WriteLine("\nUpdating new product stock by id...");
        inv.UpdateProductStock(path, (int)InventoryField.ID, newProductId.ToString(), (int)InventoryField.QUANTITY, "30");

        Console.WriteLine("\nProducts now:");
        for (int i = 0; i < inv.products.Count; i++)
        {
            Console.WriteLine(inv.products[i]._id.ToString() + " " + inv.products[i]._name + " " + inv.products[i]._price + " " + inv.products[i]._quantity);
        }

        Console.WriteLine("\nUpdating new product price by name...");
        inv.UpdateProductStock(path, (int)InventoryField.NAME, "zapatillas_adidas", (int)InventoryField.PRICE, "60000");

        Console.WriteLine("\nProducts now:");
        for (int i = 0; i < inv.products.Count; i++)
        {
            Console.WriteLine(inv.products[i]._id.ToString() + " " + inv.products[i]._name + " " + inv.products[i]._price + " " + inv.products[i]._quantity);
        }

        Console.WriteLine("\nUpdating new product price by ID...");
        inv.UpdateProductStock(path, (int)InventoryField.ID, newProductId.ToString(), (int)InventoryField.PRICE, "30000");

        Console.WriteLine("\nProducts now:");
        for (int i = 0; i < inv.products.Count; i++)
        {
            Console.WriteLine(inv.products[i]._id.ToString() + " " + inv.products[i]._name + " " + inv.products[i]._price + " " + inv.products[i]._quantity);
        }
    }
}