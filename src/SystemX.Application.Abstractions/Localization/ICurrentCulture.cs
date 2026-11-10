namespace SystemX.Application.Abstractions.Localization;

using System;
using System.Globalization;

/// <summary>
/// Provides access to the ambient culture and UI culture, and allows scoping culture changes.
/// </summary>
public interface ICurrentCulture
{
    /// <summary>
    /// Gets the current culture.
    /// </summary>
    CultureInfo Culture { get; }

    /// <summary>
    /// Gets the current UI culture.
    /// </summary>
    CultureInfo UICulture { get; }

    /// <summary>
    /// Gets the name of the current UI culture.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Begins a culture scope that will be restored when disposed.
    /// </summary>
    /// <param name="cultureName">The culture name to set for the ambient context.</param>
    /// <returns>A disposable handle that restores the previous culture upon disposal.</returns>
    IDisposable BeginScope(string cultureName);
}
