namespace VisionAid.Api.Services;

using Azure.Core.GeoJson;
using Azure.Maps.Routing;
using Azure.Maps.Search;
using Azure.Maps.Search.Models.Queries;
using VisionAid.Api.Models;
using VisionAid.Api.Services.Extensions;
public class RoutingService(MapsRoutingClient _mapsRoutingClient, MapsSearchClient _mapsSearchClient)
{
    public async Task<RoutingResponse> GetDirectionRouteAsync(string startLocation, string endLocation,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(startLocation) || string.IsNullOrWhiteSpace(endLocation))
        {
            throw new ArgumentException("Start and End location must be provided");
        }
        GeoPosition start = await GetGeoPositionAsync(startLocation, cancellationToken);
        GeoPosition end = await GetGeoPositionAsync(endLocation, cancellationToken);
        
        List<GeoPosition> routePoints = new List<GeoPosition>()
                                        {
                                            start,
                                            end
                                        };
        var options = new RouteDirectionOptions {
                InstructionsType= RouteInstructionsType.Tagged,
                RouteType = RouteType.Shortest,
                TravelMode = TravelMode.Pedestrian
        };
        // Create Route direction query object
        RouteDirectionQuery query = new RouteDirectionQuery(routePoints, options);

        var response = await _mapsRoutingClient.GetDirectionsAsync(query, cancellationToken);
        
        var instructions = response.Value.Routes.First().Guidance.Instructions;
        var summary = response.Value.Routes.First().Summary;
        var routingResponse = new RoutingResponse{
            Guidance = instructions.ToGuidanceString(),
            Summary = summary.ToSummaryString()
        };
        
        return routingResponse;
    }
    private async Task<GeoPosition> GetGeoPositionAsync(string location, CancellationToken cancellationToken = default)
    {
        var response = await _mapsSearchClient.GetGeocodingAsync(location,new GeocodingQuery {
            Top = 1
        },  cancellationToken);
        var geometry = response.Value.Features.First().Geometry;
        return new GeoPosition( geometry.Coordinates[0],  geometry.Coordinates[1] );
    }
}