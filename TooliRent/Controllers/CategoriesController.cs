using Application.Services.Interfaces;
using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TooliRent.Api.Controllers
{

    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _svc;
        public CategoriesController(ICategoryService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryReadDto>>> GetAll(CancellationToken ct) =>
            Ok(await _svc.GetAllAsync(ct));

        [HttpPost, Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CategoryReadDto>> Create(CategoryCreateDto dto, CancellationToken ct) =>
            Ok(await _svc.CreateAsync(dto, ct));

        [HttpPut("{id:guid}"), Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(Guid id, CategoryUpdateDto dto, CancellationToken ct) =>
            await _svc.UpdateAsync(id, dto, ct) ? NoContent() : NotFound();

        [HttpDelete("{id:guid}"), Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
            await _svc.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}