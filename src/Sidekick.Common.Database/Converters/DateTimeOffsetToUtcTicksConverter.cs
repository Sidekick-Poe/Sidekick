using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Sidekick.Common.Database.Converters
{
    /// <summary>
    ///     Converts <see cref="DateTimeOffset" /> to and from a long representing UTC DateTime ticks.
    /// </summary>
    /// <remarks>
    ///     Check this out to view it in SQL: https://stackoverflow.com/questions/5855299/how-do-i-display-the-following-in-a-readable-datetime-format
    /// </remarks>
    public class DateTimeOffsetToUtcTicksConverter : ValueConverter<DateTimeOffset, long>
    {
        /// <summary>
        ///     Creates a new instance of this converter.
        /// </summary>
        /// <param name="mappingHints">
        ///     Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
        ///     facets for the converted data.
        /// </param>
        public DateTimeOffsetToUtcTicksConverter(ConverterMappingHints? mappingHints = null)
            : base(
                v => v.UtcDateTime.Ticks,
                v => new DateTimeOffset(v, new TimeSpan(0, 0, 0)),
                mappingHints)
        {
        }

        /// <summary>
        ///     A <see cref="ValueConverterInfo" /> for the default use of this converter.
        /// </summary>
        public static ValueConverterInfo DefaultInfo { get; }
            = new(typeof(DateTimeOffset), typeof(long), i => new DateTimeOffsetToUtcTicksConverter(i.MappingHints));
    }
}
