using System.Globalization;
using SystemX.Application.Abstractions.Localization;

namespace SystemX.Infrastructure.Localization;

/// <summary>
/// Provides access to the ambient culture and UI culture, and allows scoping culture changes.
/// </summary>
public sealed class CurrentCulture : ICurrentCulture
{
    /// <inheritdoc />
    public CultureInfo Culture => CultureInfo.CurrentCulture;

    /// <inheritdoc />
    public CultureInfo UICulture => CultureInfo.CurrentUICulture;

    /// <inheritdoc />
    public string Name => UICulture.Name;

    /// <inheritdoc />
    public IDisposable BeginScope(string cultureName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cultureName);

        CultureInfo culture;
        try
        {
            culture = CultureInfo.GetCultureInfo(cultureName);
        }
        catch (CultureNotFoundException ex)
        {
            throw new ArgumentException($"Culture '{cultureName}' is not valid.", nameof(cultureName), ex);
        }

        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        return new RestoreScope(previousCulture, previousUiCulture);
    }

    private sealed class RestoreScope : IDisposable
    {
        private CultureInfo? _previousCulture;
        private CultureInfo? _previousUiCulture;
        private bool _disposed;

        public RestoreScope(CultureInfo previousCulture, CultureInfo previousUiCulture)
        {
            _previousCulture = previousCulture;
            _previousUiCulture = previousUiCulture;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (_previousCulture != null)
            {
                CultureInfo.CurrentCulture = _previousCulture;
            }

            if (_previousUiCulture != null)
            {
                CultureInfo.CurrentUICulture = _previousUiCulture;
            }

            _previousCulture = null;
            _previousUiCulture = null;
        }
    }
}
