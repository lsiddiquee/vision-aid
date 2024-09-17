using System.Diagnostics;
using Azure.Maps.Routing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisionAid.Api.Models;
using VisionAid.Api.Services;

namespace VisionAid.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoutingController : ControllerBase
    {
        private readonly RoutingService _routingService;

        public RoutingController(RoutingService routingService)
        {
            _routingService = routingService;
        }

        [HttpPost("Directions")]
        public async Task<ActionResult<RouteDirections>> GetDirections(string startLocation, string endLocation,
            CancellationToken cancellationToken = default)
        {
            var response = await _routingService.GetDirectionRouteAsync(startLocation, endLocation,cancellationToken);

            return Ok(response);
        }
    }
}