using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AITravelB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IMediator mediator;
        public RatingController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] Application.Ratings.Commands.CreateRating.CreateRatingCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
