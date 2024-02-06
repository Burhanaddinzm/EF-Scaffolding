using ConsoleApp2.Contexts;
using ConsoleApp2.Models;
using Microsoft.EntityFrameworkCore;

// Commands for Scaffolding:
// .NET 6.0 SDK is Required
// dotnet tool install --global --add-source https://nuget.sitecore.com/resources/v3/index.json ch-cli
// dotnet tool install --global dotnet-ef --version 6.0.26
// cd .\ConsoleApp2
// dotnet ef dbcontext scaffold "Server=DESKTOP-6QH0HT5;Database=Shop;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer --context-dir Contexts --output-dir Models

ShopContext shopDbContext = new ShopContext();

//Category methods
IEnumerable<Category> GetAllCategory()
{
    IQueryable<Category> query = shopDbContext.Categories
        .Where(x => !x.IsDeleted)
        .AsNoTracking();

    IEnumerable<Category> categories = query.ToList();

    foreach (Category category in categories)
        Console.WriteLine($"Id:{category.Id} Name:{category.Name} CreatedAt:{category.CreatedAt} UpdatedAt:{category.UpdatedAt}");

    return categories;
}

void GetByIdCategory()
{
    Console.WriteLine("Input Id:");
    int.TryParse(Console.ReadLine(), out int id);

    Category? category = shopDbContext.Categories
        .Where(x => !x.IsDeleted)
        .AsNoTracking()
        .FirstOrDefault(x => x.Id == id);

    if (category != null)
        Console.WriteLine($"Id:{category.Id} Name:{category.Name} CreatedAt:{category.CreatedAt} UpdatedAt:{category.UpdatedAt}");
    else
        Console.WriteLine("This category doesn't exist!");
}

void CreateCategory()
{
    Category category = new Category()
    {
        Name = CheckName(),
        CreatedAt = DateTime.UtcNow.AddHours(4)
    };

    shopDbContext.Categories.Add(category);
    int result = shopDbContext.SaveChanges();

    Console.WriteLine(result == 0 ? "Failed to save changes!" : "Successfully added");
}

void UpdateCategory()
{
    Console.WriteLine("Input Id:");
    int.TryParse(Console.ReadLine(), out int Id);

    Category? category = shopDbContext.Categories
        .Where(x => x.Id == Id)
        .FirstOrDefault();

    if (category == null)
    {
        Console.WriteLine("Not found!");
        return;
    }

    category.Name = CheckName();
    category.UpdatedAt = DateTime.UtcNow.AddHours(4);

    shopDbContext.SaveChanges();
}

void RemoveCategory()
{
    Console.WriteLine("Input Id:");
    int.TryParse(Console.ReadLine(), out int Id);

    Category? category = shopDbContext.Categories
        .Where(x => x.Id == Id)
        .FirstOrDefault();

    if (category == null)
    {
        Console.WriteLine("Not found!");
        return;
    }

    category.IsDeleted = true;
    shopDbContext.SaveChanges();
}

//Product methods
void GetAllProduct()
{
    IQueryable<Product> query = shopDbContext.Products
        .Where(x => !x.IsDeleted)
        .Include(x => x.Category)
        .AsNoTracking()
        .Select(x => new Product
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            CategoryId = x.CategoryId,
            Category = new Category { Name = x.Category.Name },
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
        });

    IEnumerable<Product> products = query.ToList();

    foreach (Product product in products)
        Console.WriteLine($"Id:{product.Id} Name:{product.Name} Price:{product.Price} CategoryId:{product.CategoryId} Category:{product.Category.Name} CreatedAt:{product.CreatedAt} UpdatedAt:{product.UpdatedAt}");
}

void GetByIdProduct()
{
    Console.WriteLine("Input Id:");
    int.TryParse(Console.ReadLine(), out int id);

    Product? product = shopDbContext.Products
        .Where(x => !x.IsDeleted)
        .Include(x => x.Category)
        .AsNoTracking()
        .Select(x => new Product
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            CategoryId = x.CategoryId,
            Category = new Category { Name = x.Category.Name },
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
        })
        .FirstOrDefault(x => x.Id == id);

    if (product != null)
        Console.WriteLine($"Id:{product.Id} Name:{product.Name} Price:{product.Price} CategoryId:{product.CategoryId} Category:{product.Category.Name} CreatedAt:{product.CreatedAt} UpdatedAt:{product.UpdatedAt}");
    else
        Console.WriteLine("This product doesn't exist!");
}

void CreateProduct()
{
    Product product = new Product()
    {
        Name = CheckName(),
        Price = CheckPrice(),
        CategoryId = CheckCategoryId(),
        CreatedAt = DateTime.UtcNow.AddHours(4)
    };

    shopDbContext.Products.Add(product);
    int result = shopDbContext.SaveChanges();

    Console.WriteLine(result == 0 ? "Failed to save changes!" : "Successfully added");
}

void UpdateProduct()
{
    Console.WriteLine("Input Id:");
    int.TryParse(Console.ReadLine(), out int Id);

    Product? product = shopDbContext.Products
        .Where(x => x.Id == Id)
        .FirstOrDefault();

    if (product == null)
    {
        Console.WriteLine("Not found!");
        return;
    }

    product.Name = CheckName();
    product.Price = CheckPrice();
    product.CategoryId = CheckCategoryId();
    product.UpdatedAt = DateTime.UtcNow.AddHours(4);

    shopDbContext.SaveChanges();
}

void RemoveProduct()
{
    Console.WriteLine("Input Id:");
    int.TryParse(Console.ReadLine(), out int Id);

    Product? product = shopDbContext.Products
        .Where(x => x.Id == Id)
        .FirstOrDefault();

    if (product == null)
    {
        Console.WriteLine("Not found!");
        return;
    }

    product.IsDeleted = true;
    shopDbContext.SaveChanges();
}

//Checks
string CheckName()
{
    Console.WriteLine("Input Name:");
    string? name = Console.ReadLine();

    while (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name can't be empty!");
        name = Console.ReadLine();
    }

    return name;
}

double CheckPrice()
{
    Console.WriteLine("Input Price:");
    double.TryParse(Console.ReadLine(), out double price);

    if (price == 0)
    {
        Console.WriteLine("Price can't be 0");
        double.TryParse(Console.ReadLine(), out price);
    }

    return price;
}

int CheckCategoryId()
{
    IEnumerable<Category> categories = GetAllCategory();

    bool isExist = false;

    Console.WriteLine("Input Product CategoryId:");
    int.TryParse(Console.ReadLine(), out int categoryId);

    while (!isExist)
    {
        foreach (Category category in categories)
            if (category.Id == categoryId) isExist = true;

        if (!isExist)
        {
            Console.WriteLine("Input Product CategoryId:");
            int.TryParse(Console.ReadLine(), out categoryId);
        }
    }

    return categoryId;
}

//App methods
void ShowMenu()
{
    Console.WriteLine("1.GetAll Categories");
    Console.WriteLine("2.GetById Category");
    Console.WriteLine("3.Create Category");
    Console.WriteLine("4.Update Category");
    Console.WriteLine("5.Remove Category");
    Console.WriteLine("-----------------");
    Console.WriteLine("6.GetAll Products");
    Console.WriteLine("7.GetById Product");
    Console.WriteLine("8.Create Product");
    Console.WriteLine("9.Update Product");
    Console.WriteLine("10.Remove Product");
    Console.WriteLine("-----------------");
    Console.WriteLine("0.Close\n");
}

void App()
{
    ShowMenu();
    int.TryParse(Console.ReadLine(), out int request);

    while (request != 0)
    {
        switch (request)
        {
            case 1:
                GetAllCategory();
                break;

            case 2:
                GetByIdCategory();
                break;

            case 3:
                CreateCategory();
                break;

            case 4:
                UpdateCategory();
                break;

            case 5:
                RemoveCategory();
                break;

            case 6:
                GetAllProduct();
                break;

            case 7:
                GetByIdProduct();
                break;

            case 8:
                CreateProduct();
                break;

            case 9:
                UpdateProduct();
                break;

            case 10:
                RemoveProduct();
                break;

            default:
                Console.WriteLine("Invalid request!");
                break;
        }

        ShowMenu();
        int.TryParse(Console.ReadLine(), out request);
    }
}

App();