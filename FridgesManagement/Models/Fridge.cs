using System;

namespace FridgesManagement.Models
{
    public class Fridge
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public string OwnerName { get; init; }

        public Guid ModelId { get; init; }
    }
}
