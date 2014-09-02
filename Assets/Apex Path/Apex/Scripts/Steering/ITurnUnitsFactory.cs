namespace Apex.Steering
{
    /// <summary>
    /// Interface for <see cref="ITurnUnits"/> factories
    /// </summary>
    public interface ITurnUnitsFactory
    {
        /// <summary>
        /// Creates the turner instance.
        /// </summary>
        /// <returns>The turner instance</returns>
        ITurnUnits Create();
    }
}
