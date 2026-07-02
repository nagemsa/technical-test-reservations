using Microsoft.AspNetCore.Mvc;
using TechnicalTest.Api.Contracts;
using TechnicalTest.Api.Mappings;
using TechnicalTest.Application.Reservations;

namespace TechnicalTest.Api.Controllers;

[ApiController]
[Route("reservations")]
public sealed class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReservationResponse>> Create([FromBody] CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var result = await _reservationService.CreateAsync(request.ToCommand(), cancellationToken);

        if (!result.IsSuccess)
        {
            return ReservationHttpMapper.ToFailure(this, result.Errors);
        }

        return ReservationHttpMapper.ToCreatedResponse(this, result.Value!);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ReservationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ReservationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _reservationService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            return ReservationHttpMapper.ToFailure(this, result.Errors);
        }

        return ReservationHttpMapper.ToOkList(this, result.Value!);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reservationService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ReservationHttpMapper.ToFailure(this, result.Errors);
        }

        return ReservationHttpMapper.ToOk(this, result.Value!);
    }

    [HttpPatch("{id:guid}/confirm")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationResponse>> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reservationService.ConfirmAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ReservationHttpMapper.ToFailure(this, result.Errors);
        }

        return ReservationHttpMapper.ToOk(this, result.Value!);
    }

    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationResponse>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reservationService.CancelAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ReservationHttpMapper.ToFailure(this, result.Errors);
        }

        return ReservationHttpMapper.ToOk(this, result.Value!);
    }
}