using System;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

namespace Awesome.Net.Workflows
{
    public class SecurityTokenService : ISecurityTokenService
    {
        private readonly ITimeLimitedDataProtector _dataProtector;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SecurityTokenService(IDataProtectionProvider dataProtectionProvider, IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _dataProtector = dataProtectionProvider.CreateProtector("Tokens").ToTimeLimitedDataProtector();
        }

        public string CreateToken<T>(T payload, TimeSpan lifetime)
        {
            var json = JsonConvert.SerializeObject(payload);

            return _dataProtector.Protect(json, _dateTimeProvider.Now.Add(lifetime));
        }

        public bool TryDecryptToken<T>(string token, out T payload)
        {
            payload = default;

            try
            {
                var json = _dataProtector.Unprotect(token, out var expiration);

                if (_dateTimeProvider.Now < expiration.ToUniversalTime())
                {
                    payload = JsonConvert.DeserializeObject<T>(json);
                    return true;
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return false;
        }
    }
}
