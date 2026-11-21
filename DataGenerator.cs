namespace IndependentWork11;

/// <summary>
/// Псевдовипадкова генерація тестових даних із фіксованим Seed,
/// щоб результати можна було відтворити повторно.
/// Обсяги: 60 продуктів (5 категорій по 12), 50 клієнтів, 100 продажів.
/// </summary>
public static class DataGenerator
{
    private static readonly string[] Categories =
        { "Електроніка", "Одяг", "Книги", "Спорт", "Дім" };

    private static readonly string[] Cities =
        { "Київ", "Львів", "Одеса", "Харків", "Дніпро", "Рівне", "Вінниця", "Луцьк" };

    public static List<Product> GenerateProducts(Random rnd, int perCategory = 12)
    {
        var products = new List<Product>();
        int id = 1;

        foreach (var category in Categories)
        {
            for (int i = 1; i <= perCategory; i++)
            {
                // Ціна від 10.00 до 500.00
                decimal price = rnd.Next(1000, 50001) / 100m;
                products.Add(new Product(id, $"{category} #{i}", category, price));
                id++;
            }
        }

        return products;
    }

    public static List<Customer> GenerateCustomers(Random rnd, int count = 50)
    {
        var customers = new List<Customer>();

        for (int id = 1; id <= count; id++)
        {
            string city = Cities[rnd.Next(Cities.Length)];
            customers.Add(new Customer(id, $"Клієнт {id}", city));
        }

        return customers;
    }

    public static List<Sale> GenerateSales(
        Random rnd, IReadOnlyList<Product> products, IReadOnlyList<Customer> customers, int count = 100)
    {
        var sales = new List<Sale>();
        var start = new DateTime(2024, 1, 1);

        for (int id = 1; id <= count; id++)
        {
            int productId = products[rnd.Next(products.Count)].Id;
            int customerId = customers[rnd.Next(customers.Count)].Id;
            int quantity = rnd.Next(1, 11);
            DateTime date = start.AddDays(rnd.Next(0, 731)); // 2024-01-01 .. 2025-12-31

            sales.Add(new Sale(id, productId, quantity, date, customerId));
        }

        return sales;
    }
}
