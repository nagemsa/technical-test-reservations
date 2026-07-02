using Microsoft.AspNetCore.Mvc;
using TechnicalTest.Api.Contracts;
using TechnicalTest.Application.Common;
using TechnicalTest.Application.Reservations;
using TechnicalTest.Api.Controllers;

namespace TechnicalTest.Api.Mappings;

internal static class ReservationHttpMapper
{
    public static ActionResult<ReservationResponse> ToCreatedResponse(ControllerBase controller, ReservationDto dto)
    {
        return controller.CreatedAtAction(nameof(ReservationsController.GetById), new { id = dto.Id }, ReservationResponse.From(dto));
    }

    public static ActionResult<IReadOnlyList<ReservationResponse>> ToOkList(ControllerBase controller, IReadOnlyList<ReservationDto> items)
    {
        var response = items.Select(ReservationResponse.From).ToList();
        return controller.Ok(response);
    }

    public static ActionResult<ReservationResponse> ToOk(ControllerBase controller, ReservationDto dto)
    {
        return controller.Ok(ReservationResponse.From(dto));
    }

    public static ActionResult ToFailure(ControllerBase controller, IReadOnlyCollection<Error> errors)
    {
        if (errors.Count == 0)
        {
            return controller.BadRequest(CreateValidationProblem([Error.Validation("Unexpected error.") ]));
        }

        if (errors.Any(error => error.Code == "not_found"))
        {
            return controller.NotFound(CreateNotFoundProblem(errors));
        }

        return controller.BadRequest(CreateValidationProblem(errors));
    }

    private static ValidationProblemDetails CreateValidationProblem(IEnumerable<Error> errors)
    {
        return new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            ["errors"] = errors.Select(error => error.Message).ToArray()
        })
        {
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest
        };
    }

    private static ProblemDetails CreateNotFoundProblem(IEnumerable<Error> errors)
    {
        return new ProblemDetails
        {
            Title = "Reservation not found",
            Detail = string.Join(" ", errors.Select(error => error.Message)),
            Status = StatusCodes.Status404NotFound
        };
    }
}