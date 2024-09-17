using System.ComponentModel.DataAnnotations;

namespace VisionAid.Api.Options
{
    public class AzureMapsOptions
    {
        [Required]
        public required  string SubscriptionKey { get; set; }
    }
}