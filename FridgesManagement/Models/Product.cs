using System;

namespace FridgesManagement.Models
{
    public class Product
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public int DefaultQuantity { get; init; }
    }
}
