using AutoMapper;
using FiorelloAPI.Data;
using FiorelloAPI.DTOs.Categories;
using FiorelloAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloAPI.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoryController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var category = await _context.Categories.AsNoTracking().ToListAsync();

            var mappDatas = _mapper.Map<List<CategoryDto>>(category);

            return Ok(mappDatas);
        }

        [HttpPost]

        public async Task<IActionResult> Create([FromBody] CategoryCreateDto category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _context.Categories.AddAsync(_mapper.Map<Category>(category));
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Create), category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] CategoryEditDto category)
        {
            var entity = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return NotFound();
            _mapper.Map(category, entity);
            _context.Categories.Update(entity);
            await _context.SaveChangesAsync();
            return Ok();

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var entity = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<CategoryDto>(entity));
        }

        [HttpDelete]

        public async Task<IActionResult> Delete([FromQuery] int? id)
        {
            if (id == null) return BadRequest();
            var entity = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return NotFound();
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? search)
        {
            return Ok(search == null ? await _context.Categories.ToListAsync() : await _context.Categories.Where(m => m.Name.Contains(search)).ToListAsync());
        }
    }
}
