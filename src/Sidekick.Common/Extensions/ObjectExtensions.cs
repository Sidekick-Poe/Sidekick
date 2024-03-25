namespace Sidekick.Common.Extensions;

/// <summary>
///     Extensions for objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    ///     Copies the values of the common properties
    /// </summary>
    /// <param name="source">The source object</param>
    /// <param name="destination">The destination object</param>
    public static void CopyValuesTo(
        this object source,
        object destination)
    {
        var destinationProperties = destination
                                    .GetType()
                                    .GetProperties();

        source
            .GetType()
            .GetProperties()
            .Where(x => destinationProperties.Any(y => y.Name == x.Name))
            .ToList()
            .ForEach(
                sourceProperty =>
                {
                    var destinationProperty = destinationProperties.FirstOrDefault(y => y.Name == sourceProperty.Name);
                    if (destinationProperty != null && destinationProperty.PropertyType == sourceProperty.PropertyType)
                    {
                        destinationProperty.SetValue(destination, value: sourceProperty.GetValue(source));
                    }
                });
    }
}
