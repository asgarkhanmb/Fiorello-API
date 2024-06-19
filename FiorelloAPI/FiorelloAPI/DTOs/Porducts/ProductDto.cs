﻿namespace FiorelloAPI.DTOs.Porducts
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public string CategoryName { get; set; }
        public string MainImage { get; set; }
    }
}
