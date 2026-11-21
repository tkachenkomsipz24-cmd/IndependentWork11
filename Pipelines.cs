namespace IndependentWork11;

public static class Pipelines
{
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
                 .ThenBy(x => x.Id) 
                 .Take(topN)
                 .Select(x => new ProductQuantity(x.Name, x.TotalQuantity))
                 .ToList()))
            .OrderBy(c => c.Category)
            .ToList();
    }

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
