using ProductService.Domain.Common;
using ProductService.Domain.Constants;

namespace ProductService.Domain.Entities;

public sealed class Product : Entity<Guid>, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public int BookedQuantity { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public int AvailableQuantity => StockQuantity - BookedQuantity;
    public bool IsInStock => AvailableQuantity > 0;
    public bool IsOutOfStock => AvailableQuantity <= 0;

    private Product()
        : base(Guid.Empty) { }

    public Product(Guid id, string name, decimal price, int stockQuantity, int bookedQuantity)
        : base(id)
    {
        Validate(name, price, stockQuantity, bookedQuantity);

        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
        BookedQuantity = bookedQuantity;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Product Create(string name, decimal price, int initialStock = 0)
    {
        var product = new Product(
            id: Guid.NewGuid(),
            name: name,
            price: price,
            stockQuantity: initialStock,
            bookedQuantity: 0
        );

        return product;
    }

    public void UpdateDetails(string name, decimal price)
    {
        ValidateName(name);
        ValidatePrice(price);

        Name = name;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");
        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool Book(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Booking quantity must be positive");

        if (AvailableQuantity < quantity)
            throw new ArgumentException(
                $"Cannot book {quantity} items, only {AvailableQuantity} available."
            );

        BookedQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public void CancelBooking(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Cancel quantity must be positive");

        if (BookedQuantity < quantity)
            throw new ArgumentException(
                $"Cannot cancel more than booked items. Available: {BookedQuantity}."
            );

        BookedQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Ship(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Ship quantity must be positive");

        if (BookedQuantity < quantity)
            throw new ArgumentException($"Cannot ship more than booked. Booked: {BookedQuantity}.");

        BookedQuantity -= quantity;
        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Add(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Add quantity must be positive");

        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string name, decimal price, int available, int booked)
    {
        ValidateName(name);
        ValidatePrice(price);
        ValidateQuantity(available, nameof(available));
        ValidateQuantity(booked, nameof(booked));

        if (available < booked)
            throw new ArgumentException(
                "Available quantity must be greater than or equal to booked quantity"
            );
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty");

        if (name.Length > ProductConstants.MAX_PRODUCT_NAME_LENGTH)
            throw new ArgumentException(
                $"Product name cannot exceed {ProductConstants.MAX_PRODUCT_NAME_LENGTH} characters"
            );
    }

    private static void ValidatePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative");
    }

    private static void ValidateQuantity(int quantity, string paramName)
    {
        if (quantity < 0)
            throw new ArgumentException($"{paramName} quantity cannot be negative");
    }
}
