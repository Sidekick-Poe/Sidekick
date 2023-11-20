using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Sidekick.Modules.Wealth.Models;

#nullable disable

namespace Sidekick.Modules.Wealth.Migrations
{
    /// <inheritdoc />
    public partial class DateTimeOffsetToTicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var (Name, DateTimeOffsetProperties) in GetAllDateTimeOffsetEntityProperties())
            {
                foreach (var propertyName in DateTimeOffsetProperties)
                {
                    // Reversing the implementation at https://github.com/dotnet/efcore/blob/main/src/EFCore/Storage/ValueConversion/DateTimeOffsetToBinaryConverter.cs and turning these into DateTime.Ticks fields (https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.ticks?view=net-5.0)
                    migrationBuilder.Sql($"UPDATE {Name} SET {propertyName} = (({propertyName} >> 11) * 1000) - ((({propertyName} << 53) >> 53) * 60 * 10000000)");
                }
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var (Name, DateTimeOffsetProperties) in GetAllDateTimeOffsetEntityProperties())
            {
                foreach (var propertyName in DateTimeOffsetProperties)
                {
                    // To put them back we can do the opposite but just ignore the timezone as it is already part of the date. Technically still the same time, just a different time zone which we don't really care about.
                    migrationBuilder.Sql($"UPDATE {Name} SET {propertyName} = ({propertyName} / 1000) << 11");
                }
            }
        }

        /// <summary>
        /// Identify all the columns in the database that have a DateTimeOffset or DateTimeOffset? type
        /// </summary>
        /// <returns></returns>
        private static List<(string Name, List<string> DateTimeOffsetProperties)> GetAllDateTimeOffsetEntityProperties()
        {            var entityTypes = typeof(WealthDbContext)
                .GetProperties()
                .Where(x => x.PropertyType.IsGenericType && (typeof(DbSet<>).IsAssignableFrom(x.PropertyType.GetGenericTypeDefinition())))
                .Select(x => (
                    Name: x.Name,
                    DateTimeOffsetProperties: x.PropertyType.GetGenericArguments()[0]
                        .GetProperties()
                        .Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?))
                        .Select(x => x.Name)
                        .ToList()))
                .ToList();

            return entityTypes;
        }
    }
}
