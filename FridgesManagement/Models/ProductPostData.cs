using System;
using System.ComponentModel.DataAnnotations;

namespace FridgesManagement.Models
{
    public class ProductPostData
    {
        [Required]
        public Guid FridgeId { get; init; }

        [Required]
        public Guid ProductId { get; init; }

        [Required]
        [Range(1, 20)]
        public int Quantity { get; init; }
    }
}
