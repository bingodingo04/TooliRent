using Application.Services.Interfaces;
using Application;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace TooliRent.Api.Controllers
{

    [ApiController]
    [Route("api/tools")]
    public class ToolsController : ControllerBase
    {
        private readonly IToolService _svc;
        public ToolsController(IToolService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ToolReadDto>>> Get([FromQuery] Guid? categoryId, [FromQuery] ToolStatus? status,
            [FromQuery] bool? available, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? search, CancellationToken ct)
            => Ok(await _svc.GetAsync(categoryId, status, available, from, to, search, ct));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ToolReadDto>> GetById(Guid id, CancellationToken ct)
        {
            var res = await _svc.GetByIdAsync(id, ct);
            return res is null ? NotFound() : Ok(res);
        }

        [HttpPost, Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ToolReadDto>> Create(ToolCreateUpdateDto dto, IValidator<ToolCreateUpdateDto> validator, CancellationToken ct)
        {
            var result = await validator.ValidateAsync(dto, ct);
            if (!result.IsValid)
                return BadRequest(result.Errors);
            return Ok(await _svc.CreateAsync(dto, ct));
        }

        [HttpPut("{id:guid}"), Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ToolReadDto>> Update(Guid id, ToolCreateUpdateDto dto, IValidator<ToolCreateUpdateDto> validator, CancellationToken ct)
        {
            var result = await validator.ValidateAsync(dto, ct);
            if (!result.IsValid)
                return BadRequest(result.Errors);
            var res = await _svc.UpdateAsync(id, dto, ct);
            return res is null ? NotFound() : Ok(res);
        }

        [HttpDelete("{id:guid}"), Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
            await _svc.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}
