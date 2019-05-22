﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ShortURL.Controllers
{
    [Route("")]
    public class DefaultController : Controller
    {
        private readonly ILogger _logger;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _config;
        private readonly Data.Lookup _lookup;
        private readonly Data.Update _update;

        public DefaultController(ILogger<DefaultController> logger,
            IDistributedCache cache,
            IConfiguration config,
            Data.Lookup lookup,
            Data.Update update)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _lookup = lookup ?? throw new ArgumentNullException(nameof(lookup));
            _update = update ?? throw new ArgumentNullException(nameof(update));
        }

        [Route("")]
        [HttpGet("{stub}")]
        public async Task<ActionResult<string>> Get(string stub)
        {
            var fixedStub = string.IsNullOrEmpty(stub) ? null : stub.Trim();

            return Redirect(await GetRedirectAsync(Request?.Host.Host, fixedStub));
        }

        private async Task<string> GetRedirectAsync(string domainName, string stub)
        {
            string domainNameText = domainName?.Trim();
            string stubText = stub?.Trim();

            Model.IdAndLink recordIdLink = null;
            Model.IdAndLink groupIdLink = null;

            if (!string.IsNullOrEmpty(stubText))
            {
                if (!string.IsNullOrEmpty(domainNameText))
                {
                    // domain and stub provided, check group + stub
                    recordIdLink = await _lookup.GetGroupStubAsync(domainNameText, stubText);
                }

                // no match for group + stub or stub provided with no domain
                if (recordIdLink == null)
                {
                    recordIdLink = await _lookup.GetStubNoGroupAsync(stubText);
                }
            }
            else if (!string.IsNullOrEmpty(domainNameText))
            {
                // domain provided, no stub; check group default
                groupIdLink = await _lookup.GetGroupDefaultAsync(domainNameText);
            }

            string destination;

            if (recordIdLink == null)
            {
                if (groupIdLink == null)
                {
                    groupIdLink = await _lookup.GetSystemDefault();
                    if (groupIdLink != null)
                    {
                        _logger.LogWarning("Group not found for domain {DomainNameText}, using default group: {GroupLink}",
                            domainNameText, groupIdLink?.Link);
                    }
                }

                if (groupIdLink == null)
                {
                    destination = _config[Program.ConfigurationDefaultLink];
                    _logger.LogWarning("No default URL configured in the database, defaulting to {Destination} from configuration",
                        destination);
                }
                else
                {
                    if (!string.IsNullOrEmpty(stubText))
                    {
                        _logger.LogWarning("Stub not found for domain {DomainNameText}: {StubText}",
                            domainNameText,
                            stubText);
                    }
                    destination = groupIdLink.Link;
                    await _update.UpdateGroupVisitAsync((int)groupIdLink.Id);
                }
            }
            else
            {
                destination = recordIdLink.Link;
                await _update.UpdateRecordVisitAsync((int)recordIdLink.Id);
            }

            return destination;
        }

        [HttpDelete("{stub}")]
        public async Task<IActionResult> Delete(string stub)
        {
            string domainNameText = Request?.Host.Host?.Trim();
            string stubText = stub?.Trim();

            string key = _lookup.GetCacheKey(domainNameText, stubText);

            if (key != "default")
            {
                await _cache.RemoveAsync(key);
                _logger.LogInformation("Cache key {Key} purged upon request", key);
            }

            return Ok();
        }
    }
}