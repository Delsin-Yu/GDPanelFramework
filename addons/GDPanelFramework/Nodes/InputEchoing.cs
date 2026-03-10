namespace GDPanelFramework;

/// <summary>
/// Provide access to configuration for input echoing, which controls the delay and interval for repeated key inputs.
/// </summary>
public static class InputEchoing
{
    /// <summary>
    /// The initial delay time (in milliseconds) after the first key press before repeated key inputs are triggered.
    /// </summary>
    public static uint InitialDelay { get; set; } = 250;

    /// <summary>
    /// The interval time (in milliseconds) between repeated key inputs after the initial delay has passed.
    /// </summary>
    public static uint RepeatInterval { get; set; } = 100;
}