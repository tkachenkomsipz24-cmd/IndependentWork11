using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace IndependentWork11;

public static class Program
{
    private const int Seed = 42; 
    private const int Repeats = 1000;

    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var rnd = new Random(Seed);
        var products = DataGenerator.GenerateProducts(rnd);
        var customers = DataGenerator.GenerateCustomers(rnd);
        var sales = DataGenerator.GenerateSales(rnd, products, customers);

        Console.WriteLine("=== Самостійна робота №17: LINQ-пайплайни (сценарій «Продажі») ===");
        Console.WriteLine($"Продуктів: {products.Count}, клієнтів: {customers.Count}, продажів: {sales.Count}");

        var (top, topFirst, topAvg) = Measure(() => Pipelines.TopProductsByCategory(sales, products));
        PrintHeader("Пайплайн 1: найпопулярніші продукти за категоріями (топ-3)");
        foreach (var category in top)
        {
            Console.WriteLine($"  {category.Category}:");
            for (int i = 0; i < category.Top.Count; i++)
            {
                Console.WriteLine($"    {i + 1}. {category.Top[i].Name}: {category.Top[i].Quantity} шт.");
            }
        }
        PrintTime(topFirst, topAvg);

        var (byCity, cityFirst, cityAvg) = Measure(() => Pipelines.RevenueByCity(sales, products, customers));
        PrintHeader("Пайплайн 2: загальний дохід за містами клієнтів (за спаданням)");
        foreach (var row in byCity)
        {
            Console.WriteLine($"  {row.City,-10} {Money(row.Revenue),12}");
        }
        PrintTime(cityFirst, cityAvg);

        var (byMonth, monthFirst, monthAvg) = Measure(() => Pipelines.SalesByMonth(sales, products));
        PrintHeader("Пайплайн 3: продажі за роками та місяцями");
        foreach (var row in byMonth)
        {
            Console.WriteLine($"  {row.Year}-{row.Month:D2}: {row.Units,4} шт., дохід {Money(row.Revenue),12}");
        }
        PrintTime(monthFirst, monthAvg);

        var (topCustomers, custFirst, custAvg) = Measure(() => Pipelines.TopCustomers(sales, products, customers));
        PrintHeader("Пайплайн 4 (додатковий): топ-5 клієнтів за сумою покупок");
        for (int i = 0; i < topCustomers.Count; i++)
        {
            var c = topCustomers[i];
            Console.WriteLine($"  {i + 1}. {c.Name} ({c.City}): {Money(c.Total)}");
        }
        PrintTime(custFirst, custAvg);
    }

    private static (T Result, double FirstMs, double AvgMs) Measure<T>(Func<T> pipeline)
    {
        var firstWatch = Stopwatch.StartNew();
        T result = pipeline();
        firstWatch.Stop();

        var avgWatch = Stopwatch.StartNew();
        for (int i = 0; i < Repeats; i++)
        {
            pipeline();
        }
        avgWatch.Stop();

        return (result, firstWatch.Elapsed.TotalMilliseconds, avgWatch.Elapsed.TotalMilliseconds / Repeats);
    }

    private static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"--- {title} ---");
    }

    private static void PrintTime(double firstMs, double avgMs)
    {
        Console.WriteLine(
            $"  Час: перший запуск {firstMs.ToString("F3", CultureInfo.InvariantCulture)} мс, " +
            $"середній за {Repeats} запусків {avgMs.ToString("F4", CultureInfo.InvariantCulture)} мс");
    }

    private static string Money(decimal value)
    {
        return value.ToString("F2", CultureInfo.InvariantCulture);
    }
}
