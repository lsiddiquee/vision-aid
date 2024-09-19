using System.Text;
using Azure.Maps.Routing.Models;

namespace VisionAid.Api.Services.Extensions
{
    public static class MapExtensions
    {
        public static string  ToGuidanceString(this IReadOnlyList<RouteInstruction>? instructions)
        {
            if (instructions == null)
            {
                return string.Empty;
            }
            StringBuilder instructionText = new StringBuilder();
            foreach (var step in instructions)
            {
                instructionText.Append(step.Message);
                instructionText.Append("/n");
            }
            return instructionText.ToString();
        }
        public static string ToSummaryString(this RouteSummary? summary)
        {
            if (summary == null)
            {
                return string.Empty;
            }
            StringBuilder summaryText = new StringBuilder();
            summaryText.Append($"Total Distance: {summary.LengthInMeters} meters");
            summaryText.Append($"Total Duration: {summary.TravelTimeInSeconds} seconds");
            return summaryText.ToString();
        }
    }
}