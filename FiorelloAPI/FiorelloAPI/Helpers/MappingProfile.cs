using AutoMapper;
using FiorelloAPI.DTOs.Blogs;
using FiorelloAPI.DTOs.Categories;
using FiorelloAPI.DTOs.Porducts;
using FiorelloAPI.DTOs.Sliders;
using FiorelloAPI.Models;
using System.Reflection.Metadata;

namespace FiorelloAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Slider, SliderDto>();

            CreateMap<SliderCreateDto, Slider>();

            CreateMap<SliderEditDto, Slider>();
            CreateMap<Blog, BlogDto>();

            CreateMap<BlogCreateDto, Blog>();

            CreateMap<BlogEditDto, Blog>();
            CreateMap<Category, CategoryDto>();

            CreateMap<CategoryCreateDto, Category>();

            CreateMap<CategoryEditDto, Category>();
        }
    }
}
