using System;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using Volo.Abp.Timing;

namespace Awesome.Net.Workflows.Services
{
    public class SecurityTokenService : ISecurityTokenService
    {
        private readonly ITimeLimitedDataProtector _dataProtector;
        private readonly IClock _clock;

        public SecurityTokenService(
            IDataProtectionProvider dataProtectionProvider,
            IClock clock)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Tokens").ToTimeLimitedDataProtector();
            _clock = clock;
        }

        public string CreateToken<T>(T payload, TimeSpan lifetime)
        {
            var json = JsonConvert.SerializeObject(payload);

            return _dataProtector.Protect(json, _clock.Now.Add(lifetime));
        }

        public bool TryDecryptToken<T>(string token, out T payload)
        {
            payload = default;

            try
            {
                var json = _dataProtector.Unprotect(token, out var expiration);

                if(_clock.Now < expiration.ToUniversalTime())
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