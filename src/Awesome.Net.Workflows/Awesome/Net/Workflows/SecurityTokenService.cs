using System;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

namespace Awesome.Net.Workflows
{
    public class SecurityTokenService : ISecurityTokenService
    {
        private readonly ITimeLimitedDataProtector _dataProtector;

        public SecurityTokenService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Tokens").ToTimeLimitedDataProtector();
        }

        public string CreateToken<T>(T payload, TimeSpan lifetime)
        {
            var json = JsonConvert.SerializeObject(payload);

            return _dataProtector.Protect(json, DateTimeOffset.UtcNow.Add(lifetime));
        }

        public bool TryDecryptToken<T>(string token, out T payload)
        {
            payload = default;

            try
            {
                var json = _dataProtector.Unprotect(token, out var expiration);

                if(DateTimeOffset.UtcNow < expiration.ToUniversalTime())
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