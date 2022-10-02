﻿namespace MetaExchange.E2ETests.Core
{
    public class Order
    {
        public Guid? Id { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        public string Kind { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
    }
}