﻿namespace BikeSparesInventorySystem.Data.Models;

public class Miners : IModel, ICloneable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Code { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
    public Guid ProductId { get; set; }
    public Guid ActedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime DateUpdated { get; set; }

    public object Clone()
    {
        return new Miners()
        {
            Id = Id,
            Name = Name,
            Code = Code,
            Price = Price,
            ProductId = ProductId,
            ActedBy = ActedBy,
            CreatedAt = CreatedAt,
            DateUpdated = DateUpdated
        };
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
