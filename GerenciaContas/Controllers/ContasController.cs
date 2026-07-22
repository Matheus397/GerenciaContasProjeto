using GerenciaContas.Application.Abstractions;
using GerenciaContas.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GerenciaContas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ContasController : ControllerBase
{
    private readonly IContaService _service;

    public ContasController(IContaService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ContaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ContaResponse>>> Listar(CancellationToken ct)
        => Ok(await _service.ListarAsync(ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContaResponse>> ObterPorId(Guid id, CancellationToken ct)
    {
        var conta = await _service.ObterPorIdAsync(id, ct);
        return conta is null ? NotFound() : Ok(conta);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ContaResponse>> Criar(CriarContaRequest request, CancellationToken ct)
    {
        var conta = await _service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = conta.Id }, conta);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContaResponse>> Atualizar(Guid id, AtualizarContaRequest request, CancellationToken ct)
    {
        var conta = await _service.AtualizarAsync(id, request, ct);
        return conta is null ? NotFound() : Ok(conta);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        var removida = await _service.RemoverAsync(id, ct);
        return removida ? NoContent() : NotFound();
    }
}
