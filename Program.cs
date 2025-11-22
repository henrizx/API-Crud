using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/products", (Product product) =>
{
    if (ProductRepository.GetBy(product.Code) != null)
        return Results.Conflict("Product already exists");

    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product.Code);
});

app.MapGet("/products/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.GetBy(code);
    return product is not null
        ? Results.Ok(product)
        : Results.NotFound();
});

app.MapPut("/products/{code}", (string code, Product product) =>
{
    var productSaved = ProductRepository.GetBy(code);

    if (productSaved is null)
        return Results.NotFound("Product not found");

    productSaved.Name = product.Name;
    productSaved.Code = product.Code;

    return Results.Ok(productSaved);
});

app.MapDelete("/products/{code}", ([FromRoute] string code) =>
{
    var productSaved = ProductRepository.GetBy(code);

    if (productSaved is null)
        return Results.NotFound("Product not found");

    ProductRepository.Remove(productSaved);
    return Results.Ok();
});

app.Run();

public class Product
{
    public string Code { get; set; }
    public string Name { get; set; }
}

public static class ProductRepository
{
    public static List<Product> Products { get; set; } = new();

    public static void Add(Product product)
    {
        Products.Add(product);
    }

    public static Product GetBy(string code)
    {
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product)
    {
        Products.Remove(product);
    }
}
