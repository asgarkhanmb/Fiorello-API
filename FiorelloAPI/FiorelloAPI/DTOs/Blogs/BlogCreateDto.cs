﻿namespace FiorelloAPI.DTOs.Blogs
{
    public class BlogCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }
        public IFormFile Images { get; set; }
    }
}