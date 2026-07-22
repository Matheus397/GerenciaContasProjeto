using GerenciaContas.Application.Abstractions;
using GerenciaContas.Application.Common;
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

    [HttpGet("{cpf}")]
    [ProducesResponseType(typeof(ContaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContaResponse>> ObterPorCpf([FromRoute] string cpf, CancellationToken ct)
    {
        var conta = await _service.ObterPorCpfAsync(cpf, ct);
        return conta is null ? NotFound() : Ok(conta);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ContaResponse>> Criar([FromBody] CriarContaRequest request, CancellationToken ct)
    {
        var resultado = await _service.CriarAsync(request, ct);

        if (resultado.Status == ResultStatus.Conflito)
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "CPF já cadastrado",
                Detail = resultado.Erro
            });

        var conta = resultado.Valor!;
        return CreatedAtAction(nameof(ObterPorCpf), new { cpf = conta.Cpf.SomenteDigitos() }, conta);
    }

    [HttpPut("{cpf}")]
    [ProducesResponseType(typeof(ContaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContaResponse>> Atualizar(
        [FromRoute] string cpf, [FromBody] AtualizarContaRequest request, CancellationToken ct)
    {
        var conta = await _service.AtualizarAsync(cpf, request, ct);
        return conta is null ? NotFound() : Ok(conta);
    }

    [HttpDelete("{cpf}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover([FromRoute] string cpf, CancellationToken ct)
    {
        var removida = await _service.RemoverAsync(cpf, ct);
        return removida ? NoContent() : NotFound();
    }
}
