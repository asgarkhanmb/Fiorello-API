using AutoMapper;
using FiorelloAPI.Data;
using FiorelloAPI.DTOs.Blogs;
using FiorelloAPI.Helpers.Extensions;
using FiorelloAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloAPI.Controllers
{
    public class BlogController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BlogCreateDto blog)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!blog.Images.CheckFileType("image"))
            {
                ModelState.AddModelError("Image", "Input can accept only image format");
                return BadRequest();
            }

            if (!blog.Images.CheckFileSize(200))
            {
                ModelState.AddModelError("Image", "Image size must be max 200 KB");
                return BadRequest();
            }
            string fileName = Guid.NewGuid().ToString() + "-" + blog.Images.FileName;

            string path = _env.GenerateFilePath("image", fileName);

            await blog.Images.SaveFileToLocalAsync(path);
            blog.Image = fileName;

            await _context.Blogs.AddAsync(_mapper.Map<Blog>(blog));
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Create), blog);
        }

        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var result = await _context.Blogs.AsNoTracking().ToListAsync();

            var mappDatas = _mapper.Map<List<BlogDto>>(result);

            return Ok(mappDatas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var entity = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<BlogDto>(entity));
        }

        [HttpDelete]

        public async Task<IActionResult> Delete([FromQuery] int? id)
        {
            if (id == null) return BadRequest();
            var entity = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return NotFound();
            string path = _env.GenerateFilePath("image", entity.Image);
            path.DeleteFileFromLocal();
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? search)
        {
            return Ok(search == null ? await _context.Blogs.ToListAsync() : await _context.Blogs.Where(m => m.Title.Contains(search)).ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] BlogEditDto blog)
        {
            if (id == null) return BadRequest();
            var entity = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return NotFound();


            if (blog.Images is not null)
            {
                if (!blog.Images.CheckFileType("image"))
                {
                    ModelState.AddModelError("NewImage", "Input can accept only image format");
                    blog.Image = entity.Image;
                    return BadRequest();
                }

                if (!blog.Images.CheckFileSize(200))
                {
                    ModelState.AddModelError("NewImage", "Image size must be max 200 KB");
                    blog.Image = entity.Image;
                    return BadRequest();
                }
            }
            if (blog.Images is not null)
            {
                string oldPath = _env.GenerateFilePath("image", entity.Image);

                oldPath.DeleteFileFromLocal();

                string fileName = Guid.NewGuid().ToString() + "-" + blog.Images.FileName;

                string newPath = _env.GenerateFilePath("image", fileName);

                await blog.Images.SaveFileToLocalAsync(newPath);

                blog.Image = fileName;
            }

            _mapper.Map(blog, entity);
            _context.Blogs.Update(entity);
            await _context.SaveChangesAsync();
            return Ok();

        }
    }
}
