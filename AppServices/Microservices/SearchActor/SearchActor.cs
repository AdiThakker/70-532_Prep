using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SearchActor.Interfaces;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace SearchActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class SearchActor : Actor, ISearchActor
    {
        /// <summary>
        /// Initializes a new instance of SearchActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public SearchActor(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            httpClient = new HttpClient();

            return Task.FromResult(true);
        }

        public async Task<Tuple<string, int>> SearchForKeyword(Uri uri, string keyword)
        {
            int count = 0;

            try
            {
                var httpResponse = await httpClient.GetAsync(uri);
                var pageContent = await httpResponse.Content.ReadAsStringAsync();
                var matches = Regex.Matches(pageContent, keyword, RegexOptions.IgnoreCase);
                count = matches.Count;
            }
            catch (Exception ex)
            {
                ActorEventSource.Current.ActorMessage(this, $"Exception: {ex}");
            }

            return Tuple.Create(uri.ToString(), count);
        }

        HttpClient httpClient { get; set; }
    }
}
