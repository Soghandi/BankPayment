using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Adin.BankPayment.Controllers
{
    public class BaseController : Controller
    {
        private readonly IRepository<Application> _applicationRepository;
        private readonly ILogger _logger;
        protected readonly IMemoryCache MemoryCache;

        public BaseController(IMemoryCache memCaches,
            ILogger logger,
            IRepository<Application> applicationRepository)
        {
            MemoryCache = memCaches;
            _logger = logger;
            _applicationRepository = applicationRepository;
        }


        protected async Task<Application> GetApplicationAsync()
        {
            var claimsPrincipal = User;
            var applicationId = Guid.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value);
            var keyName = "App-" + applicationId;
            Application currentApp;
            MemoryCache.TryGetValue(keyName, out currentApp);
            if (currentApp == null)
            {
                currentApp = await _applicationRepository.GetFirstBy(x => x.Id == applicationId);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
                MemoryCache.Set(keyName, currentApp, cacheEntryOptions);
            }

            return currentApp;
        }
    }
}