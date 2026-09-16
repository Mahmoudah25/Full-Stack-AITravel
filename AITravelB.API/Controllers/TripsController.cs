using AITravelB.Application.Trips.Commands;
using AITravelB.Application.Trips.Commands.GenerateItinerary;
using AITravelB.Application.Trips.Queries.GrtTripById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace AITravelB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly IMediator mediator;
        public TripsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripCommand command)
        {
            var tripId = await mediator.Send(command);
            return Ok(tripId);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripById(Guid id)
        {
            var trip = await mediator.Send(new GetTripByIdQuery(id));
            if (trip == null)
            {
                return NotFound();
            }
            return Ok(trip);
        }

        [HttpPost("generate-itinerary")]
        public async Task<IActionResult> GenerateItinerary([FromBody] GenerateItinerary command)
        {
            var itinerary = await mediator.Send(command);
            return Ok(itinerary);
        }

    }
}
