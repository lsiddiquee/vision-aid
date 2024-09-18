namespace VisionAid.MobileApp.Services;

internal class GeoLocationService
{
    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;

    public async Task<Location> GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            return await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }

    public void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }
}
