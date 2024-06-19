using AutoMapper;
using FiorelloAPI.Data;
using FiorelloAPI.DTOs.Sliders;
using FiorelloAPI.Helpers.Extensions;
using FiorelloAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloAPI.Controllers
{

    public class SliderController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;


        public SliderController(AppDbContext context,
                                IWebHostEnvironment env,
                                IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var slider = await _context.Sliders.AsNoTracking().ToListAsync();

            var mappDatas = _mapper.Map<List<SliderDto>>(slider);

            return Ok(mappDatas);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var entity = await _context.Sliders.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<SliderDto>(entity));
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]SliderCreateDto form)
        {
            if (!ModelState.IsValid)
            {
                return Ok();
            }

            foreach (var item in form.Images)
            {
                if (!item.CheckFileType("image"))
                {
                    ModelState.AddModelError("Image", "Input can accept only image format");
                    return Ok();
                }
                if (!item.CheckFileSize(200))
                {
                    ModelState.AddModelError("Image", "Image size must be max 200 KB");
                    return Ok();
                }
            }

            foreach (var item in form.Images)
            {
                string fileName = Guid.NewGuid().ToString() + "-" + item.FileName;

                string path = Path.Combine(_env.WebRootPath, "image", fileName);

                await item.SaveFileToLocalAsync(path);

                await _context.Sliders.AddAsync(new Models.Slider { Image = fileName });

                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(Create), form);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery]int? id)
        {
            if (id == null)
                return BadRequest();
            var deletedSlider = await _context.Sliders.FindAsync(id);
            if (deletedSlider == null) return NotFound();

            string path = _env.GenerateFilePath("image", deletedSlider.Image);

            path.DeleteFileFromLocal();

            _context.Sliders.Remove(deletedSlider);

            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int? id, [FromForm] SliderEditDto slider)
        {
            if (id == null) return BadRequest();
            var entity = await _context.Sliders.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return NotFound();


            if (slider.NewImage is not null)
            {
                if (!slider.NewImage.CheckFileType("image"))
                {
                    ModelState.AddModelError("NewImage", "Input can accept only image format");
                    slider.Image = entity.Image;
                    return BadRequest();
                }

                if (!slider.NewImage.CheckFileSize(200))
                {
                    ModelState.AddModelError("NewImage", "Image size must be max 200 KB");
                    slider.Image = entity.Image;
                    return BadRequest();
                }
            }
            if (slider.NewImage is not null)
            {
                string oldPath = _env.GenerateFilePath("image", entity.Image);

                oldPath.DeleteFileFromLocal();

                string fileName = Guid.NewGuid().ToString() + "-" + slider.NewImage.FileName;

                string newPath = _env.GenerateFilePath("image", fileName);

                await slider.NewImage.SaveFileToLocalAsync(newPath);

                slider.Image = fileName;
            }

            _mapper.Map(slider, entity);
            _context.Sliders.Update(entity);
            await _context.SaveChangesAsync();
            return Ok();

        }
    }
}
