﻿using System;
using System.Text.Json.Serialization;

namespace Beecow.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; }
        public DateTime TS { get; set; }
        public int CustomerId { get; set; }
        [JsonIgnore]
        public User Customer { get; set; }
    }
}
