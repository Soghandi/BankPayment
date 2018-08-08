using Adin.BankPayment.Domain.Context;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Adin.BankPayment.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IMemoryCache MemoryCache;
        private readonly ILogger _logger;
        private IRepository<Application> _applicationRepository;
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
            var claimsPrincipal = this.User as ClaimsPrincipal;
            Guid applicationId = Guid.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value);
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
