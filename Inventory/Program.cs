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

    public override string ToString()
    {
        return String.Format("{0} {1} {2} {3}", _id, _name, _price, _quantity);
    }

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
    private static readonly string _path = @"C:\Users\pedro\source\repos\PedroYanezE\ProductInventoryApp\Inventory\inventory.txt";
    private static readonly List<Product> _products = ReadInventoryFile();
    public static List<Product> Products
    {
        get { return _products; }
    }

    private static List<Product> ReadInventoryFile()
    {
        List<Product> products = new();

        if(File.Exists(_path))
        {
            foreach (string line in File.ReadLines(_path))
            {
                string[] splittedLine = line.Split(' ');

                Product currentProduct = new(Int32.Parse(splittedLine[0]), splittedLine[1], Int32.Parse(splittedLine[2]), Int32.Parse(splittedLine[3]));
                products.Add(currentProduct);
            }
        }

        return products;
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

    public static void AddProduct(Product newProduct)
    {
        // Check if product already exists
        if(_products.Find(prod => prod._name == newProduct._name) != null)
        {
            return;
        }

        // Add file to class' products
        _products.Add(newProduct);

        // Add product to inventory file
        if (!File.Exists(_path))
        {
            Console.WriteLine("Product doesn't exist");
            using StreamWriter sw = File.CreateText(_path);
            string newLine = String.Format(
                "{0} {1} {2} {3}",
                newProduct._id, newProduct._name, newProduct._price, newProduct._quantity
            );
            sw.WriteLine(newLine);
        } else
        {
            Console.WriteLine("Product exists");
            using StreamWriter sw = File.AppendText(_path);
            string newLine = String.Format(
                "{0} {1} {2} {3}",
                newProduct._id, newProduct._name, newProduct._price, newProduct._quantity
            );
            sw.WriteLine(newLine);
        }
    }

    public static string? GetProduct(InventoryField searchField, string searchValue)
    {
        if (searchField == InventoryField.ID)
        {
            Product? foundProduct = _products.Find(prod => prod._id.ToString() == searchValue);
            if (foundProduct != null)
            {
                return foundProduct.ToString();
            }
        } else if(searchField == InventoryField.NAME)
        {
            Product? foundProduct = _products.Find(prod => prod._name == searchValue);
            if (foundProduct != null)
            {
                return foundProduct.ToString();
            }
        }

        return null;
    }

    public static void UpdateProductStock(InventoryField searchField, string searchValue, InventoryField fieldToUpdate, string updatedField)
    {
        // Create a new file that will contain the updated product, then replace the original file
        // with the new one.
        // This implementation should be suitable for large files, according to:
        // https://stackoverflow.com/a/1971052/12221075
        using (StreamWriter sw = File.CreateText(_path + "-temp"))
        {
            foreach (string line in File.ReadLines(_path))
            {
                string[] splittedLine = line.Split(' ');
                string newLine = line;

                if (splittedLine[(int) searchField] == searchValue)
                {
                    if (fieldToUpdate == InventoryField.PRICE)
                    {
                        newLine = String.Format(
                            "{0} {1} {2} {3}",
                            splittedLine[0], splittedLine[1], updatedField, splittedLine[3]
                        );
                    }
                    else if (fieldToUpdate == InventoryField.QUANTITY)
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

        File.Delete(_path);
        File.Move(_path + "-temp", _path);

        // Update product in products list
        Product? productToUpdate = null;
        if(searchField == (int)InventoryField.ID)
        {
            productToUpdate = _products.Find(prod => prod._id == Int32.Parse(searchValue));
        } else if(searchField == InventoryField.NAME)
        {
            productToUpdate = _products.Find(prod => prod._name == searchValue);
        }
        
        if(productToUpdate!= null )
        {
            if(fieldToUpdate == InventoryField.PRICE)
            {
                productToUpdate._price = Int32.Parse(updatedField);
            } else if(fieldToUpdate == InventoryField.QUANTITY)
            {
                productToUpdate._quantity = Int32.Parse(updatedField);
            }
        }
    }
}

class Program
{
    static void Main()
    {
        bool keepRunning = true;

        while (keepRunning)
        {
            Console.Write(@"Welcome to inventory system.
    1. Search for a product
    2. Add a product
    3. Update a product
    4. Exit

Your option: ");

            string? option = Console.ReadLine();

            Console.WriteLine("");

            switch (option)
            {
                case "1":
                    SearchProduct();
                    break;
                case "2":
                    AddProduct();
                    break;
                case "4":
                    keepRunning = false;
                    break;
                default:
                    Console.WriteLine("Please enter a valid option.");
                    break;
            }
        }
    }

    static void SearchProduct()
    {
        Console.Write(@"Search By
    1. ID
    2. Name

Your option: ");

        bool keepAsking = true;

        while(keepAsking)
        {
            string? searchBy = Console.ReadLine();
            Console.WriteLine("");

            string? searchValue;

            switch (searchBy)
            {
                case "1":
                    Console.Write("Enter ID: ");
                    searchValue = Console.ReadLine();
                    Console.WriteLine("\nResult: " + Inventory.GetProduct(InventoryField.ID, searchValue) + "\n");
                    keepAsking = false;
                    break;
                case "2":
                    Console.Write("Enter name: ");
                    searchValue = Console.ReadLine();
                    Console.WriteLine("\nResult: " + Inventory.GetProduct(InventoryField.NAME, searchValue) + "\n");
                    keepAsking = false;
                    break;
                default:
                    Console.WriteLine("Please enter 1 to search by ID or 2 to search by name.");
                    break;
            }
        }
    }

    static void AddProduct()
    {
        Console.Write("Enter the product's name: ");
        string? name = Console.ReadLine();

        while (!StringIsValid(name))
        {
            Console.Write("Please, enter a valid name: ");
            name = Console.ReadLine();
        }

        Console.Write("Enter the product's price: ");
        string? priceInput = Console.ReadLine();
        bool isPriceNumeric = int.TryParse(priceInput, out int priceValue);

        while (!isPriceNumeric)
        {
            Console.Write("Please, enter a valid price value: ");
            priceInput = Console.ReadLine();
            isPriceNumeric = int.TryParse(priceInput, out priceValue);
        }

        Console.Write("Enter the product's current stock: ");
        string? stockInput = Console.ReadLine();
        bool isStockNumeric = int.TryParse(stockInput, out int stockValue);

        while (!isStockNumeric)
        {
            Console.Write("Please, enter a valid stock value: ");
            stockInput = Console.ReadLine();
            isStockNumeric = int.TryParse(stockInput, out stockValue);
        }

        Product newProduct = new(Inventory.GenerateId(), name, priceValue, stockValue);
        Inventory.AddProduct(newProduct);

        Console.WriteLine("Created product:");
        Console.WriteLine(newProduct.ToString());
    }
    
    static bool StringIsValid(string? input)
    {
        return !String.IsNullOrWhiteSpace(input);
    }
}
