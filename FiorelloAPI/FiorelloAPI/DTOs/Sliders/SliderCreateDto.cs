using System.ComponentModel.DataAnnotations;

namespace FiorelloAPI.DTOs.Sliders
{
    public class SliderCreateDto
    {
        [Required]
        public List<IFormFile> Images { get; set; }
    }
}
