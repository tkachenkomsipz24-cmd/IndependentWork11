namespace IndependentWork11;

public record Product(int Id, string Name, string Category, decimal UnitPrice);

public record Customer(int Id, string Name, string City);

public record Sale(int Id, int ProductId, int Quantity, DateTime SaleDate, int CustomerId);

public record ProductQuantity(string Name, int Quantity);

public record CategoryTop(string Category, List<ProductQuantity> Top);

public record CityRevenue(string City, decimal Revenue);

public record MonthSales(int Year, int Month, int Units, decimal Revenue);

public record CustomerSpend(string Name, string City, decimal Total);
