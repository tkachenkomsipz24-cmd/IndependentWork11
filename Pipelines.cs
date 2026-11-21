namespace IndependentWork11;

/// <summary>
/// LINQ-пайплайни для сценарію «Продажі». Кожен пайплайн виконується повністю
/// (завершується ToList), тому вимірювання часу охоплює реальну обробку даних.
/// </summary>
public static class Pipelines
{
    /// <summary>
    /// Пайплайн 1. Найпопулярніші продукти за категоріями.
    /// GroupBy (за продуктом) -> Sum -> Join (з продуктами) -> GroupBy (за категорією)
    /// -> OrderByDescending + Take(3) всередині кожної категорії.
    /// </summary>
    public static List<CategoryTop> TopProductsByCategory(
        IEnumerable<Sale> sales, IEnumerable<Product> products, int topN = 3)
    {
        return sales
            .GroupBy(s => s.ProductId)
            .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(s => s.Quantity) })
            .Join(products,
                x => x.ProductId,
                p => p.Id,
                (x, p) => new { p.Id, p.Name, p.Category, x.TotalQuantity })
            .GroupBy(x => x.Category)
            .Select(g => new CategoryTop(
                g.Key,
                g.OrderByDescending(x => x.TotalQuantity)
                 .ThenBy(x => x.Id) // однозначний порядок при однаковій кількості
                 .Take(topN)
                 .Select(x => new ProductQuantity(x.Name, x.TotalQuantity))
                 .ToList()))
            .OrderBy(c => c.Category)
            .ToList();
    }

    /// <summary>
    /// Пайплайн 2. Загальний дохід за містами клієнтів.
    /// Join (продажі + продукти) -> Join (+ клієнти) -> GroupBy (за містом)
    /// -> Sum -> OrderByDescending.
    /// </summary>
    public static List<CityRevenue> RevenueByCity(
        IEnumerable<Sale> sales, IEnumerable<Product> products, IEnumerable<Customer> customers)
    {
        return sales
            .Join(products,
                s => s.ProductId,
                p => p.Id,
                (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
            .Join(customers,
                x => x.CustomerId,
                c => c.Id,
                (x, c) => new { c.City, x.Revenue })
            .GroupBy(x => x.City)
            .Select(g => new CityRevenue(g.Key, g.Sum(x => x.Revenue)))
            .OrderByDescending(r => r.Revenue)
            .ToList();
    }

    /// <summary>
    /// Пайплайн 3. Продажі за місяцями та роками.
    /// Join (продажі + продукти) -> GroupBy (рік, місяць) -> Sum (одиниці, дохід)
    /// -> OrderBy (рік) -> ThenBy (місяць).
    /// </summary>
    public static List<MonthSales> SalesByMonth(
        IEnumerable<Sale> sales, IEnumerable<Product> products)
    {
        return sales
            .Join(products,
                s => s.ProductId,
                p => p.Id,
                (s, p) => new
                {
                    s.SaleDate.Year,
                    s.SaleDate.Month,
                    s.Quantity,
                    Revenue = s.Quantity * p.UnitPrice
                })
            .GroupBy(x => new { x.Year, x.Month })
            .Select(g => new MonthSales(
                g.Key.Year,
                g.Key.Month,
                g.Sum(x => x.Quantity),
                g.Sum(x => x.Revenue)))
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();
    }

    /// <summary>
    /// Пайплайн 4 (додатковий). Топ клієнтів за сумою покупок.
    /// Join -> GroupBy (за клієнтом) -> Sum -> Join (з клієнтами)
    /// -> OrderByDescending -> Take.
    /// </summary>
    public static List<CustomerSpend> TopCustomers(
        IEnumerable<Sale> sales, IEnumerable<Product> products, IEnumerable<Customer> customers, int count = 5)
    {
        return sales
            .Join(products,
                s => s.ProductId,
                p => p.Id,
                (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => x.CustomerId)
            .Select(g => new { CustomerId = g.Key, Total = g.Sum(x => x.Revenue) })
            .Join(customers,
                x => x.CustomerId,
                c => c.Id,
                (x, c) => new CustomerSpend(c.Name, c.City, x.Total))
            .OrderByDescending(c => c.Total)
            .Take(count)
            .ToList();
    }
}
