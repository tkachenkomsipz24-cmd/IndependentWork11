namespace IndependentWork11;

// ---------- Сутності сценарію «Продажі» ----------

/// <summary>Продукт: Id, Name, Category, UnitPrice.</summary>
public record Product(int Id, string Name, string Category, decimal UnitPrice);

/// <summary>Клієнт: Id, Name, City.</summary>
public record Customer(int Id, string Name, string City);

/// <summary>Продаж: Id, ProductId, Quantity, SaleDate, CustomerId.</summary>
public record Sale(int Id, int ProductId, int Quantity, DateTime SaleDate, int CustomerId);

// ---------- Типи результатів пайплайнів ----------

public record ProductQuantity(string Name, int Quantity);

public record CategoryTop(string Category, List<ProductQuantity> Top);

public record CityRevenue(string City, decimal Revenue);

public record MonthSales(int Year, int Month, int Units, decimal Revenue);

public record CustomerSpend(string Name, string City, decimal Total);
