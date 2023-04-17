using System;
using ElectronNET.API.Entities;

namespace Sidekick.Electron
{
    /// <summary>
    /// Service that holds the cookie name and value for <see cref="ElectronCookieProtectionMiddleware"/>.
    /// </summary>
    public class ElectronCookieProtection
    {
        public string Name { get; init; }
        public string Value { get; init; }
        public CookieDetails Cookie { get; init; }

        public ElectronCookieProtection()
        {
            Name = "ElectronCookieProtection";
            Value = Guid.NewGuid().ToString();
            Cookie = new CookieDetails()
            {
                Name = Name,
                Value = Value,
                Url = "http://localhost",
            };
        }
    }
}
