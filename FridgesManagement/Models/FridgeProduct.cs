using System;

namespace FridgesManagement.Models
{
    public class FridgeProduct
    {
        public Guid Id { get; init; }

        public Guid ProductId { get; init; }

        public string ProductName { get; init; }

        public int DefaultQuantity { get; init; }

        public int Quantity { get; init; }
    }
}
