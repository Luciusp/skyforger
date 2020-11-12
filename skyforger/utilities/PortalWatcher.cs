using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using skyforger.models;

namespace skyforger.Utilities
{
    public class PortalWatcher: IHostedService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PortalWatcher> _logger;
        private readonly IHttpClientFactory _httpfactory;
        private readonly IServiceScopeFactory _scopefactory;
        private Timer _portalwatchtimer;
        
        public PortalWatcher(IConfiguration config, ILogger<PortalWatcher> logger, IHttpClientFactory httpfactory,
            IServiceScopeFactory scopefactory)
        {
            _config = config;
            _logger = logger;
            _httpfactory = httpfactory;
            _scopefactory = scopefactory;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed portal watcher is starting...");
            _portalwatchtimer = new Timer(async (e) => { await UpdateSync(); },null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed portal watcher is stopping...");
            _portalwatchtimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _portalwatchtimer.Dispose();
        }
        
        //performs sync of updated spells. List fetched from Obsidian Portal's Stream
        public async Task UpdateSync()
        {
            try
            {

                using var scope = _scopefactory.CreateScope();
                var sfc = scope.ServiceProvider.GetRequiredService<SpellsContext>();

                //fetch home page html
                using var request = new HttpRequestMessage(HttpMethod.Get,
                    _config["ObsidianPortal:BaseURI"]);
                using var client = _httpfactory.CreateClient();

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                //find entries in Stream where a wiki page was updated
                var updatedpages = Regex.Matches(content, @"\<a .*?<\/a> updated the wiki page .*<\/a>");
                //TODO: handle brand new spells

                //pages can be updated multiple times. Remove duplicates by casting to hashset, normalize wiki_pages because obsidian portal allows either
                var spelluris = updatedpages.Select(t => Regex.Match(t.Value, @"https:\/\/skies-.*"">").Value
                    .Replace("\">", "").Replace("wiki_pages", "wikis").ToLower()).ToHashSet();

                foreach (var spelluri in spelluris)
                {
                    //find the spell that needs to be updated in the source db
                    var spelltoreplace = sfc.Spells.FirstOrDefault(t => t.SpellUri.ToLower() == spelluri);

                    //if nothing is found, assume that this is either a new page or unrelated
                    if (spelltoreplace == null)
                        continue;

                    //fetch spell html
                    using var spellreq = new HttpRequestMessage(HttpMethod.Get, spelluri);
                    using var spellclient = _httpfactory.CreateClient();

                    var spellres = await spellclient.SendAsync(spellreq);
                    var spellcontent = await spellres.Content.ReadAsStringAsync();

                    //transpose the spell as normal to pick up the changes
                    var spellscraperesult = await SpellScraper.TransposeSpell(spellcontent, spelluri);

                    if (spellscraperesult.spell.Valid || !spellscraperesult.errors.Any())
                    {
                        try
                        {
                            //remove the old spell
                            sfc.Spells.Remove(spelltoreplace);
                            //add the new one
                            sfc.Spells.Add(spellscraperesult.spell);
                            //save it
                            await sfc.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError("Unable to remove or save updated spell", e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to complete watcher request", e);
                Console.WriteLine(e.Message);
            }
        }
    }
}