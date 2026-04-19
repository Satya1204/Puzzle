using UnityEngine;
using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;

namespace PuzzleApp.Features.MatchingPair
{
    public sealed class MatchingPairModule : IAppModule
    {
        public void Register(IServiceRegistry services)
        {
            Debug.Log("[MatchingPair] Module.Register");
            services.RegisterSingleton<IMatchingPairCatalog>(r =>
            {
                var config = r.Resolve<MatchingPairCatalogConfig>();
                Debug.Log($"[MatchingPair] Factory building catalog. config={(config != null ? config.name : "NULL")} variants.Length={(config != null && config.variants != null ? config.variants.Length : -1)}");
                return new MatchingPairCatalog(config != null ? config.variants : null);
            });
        }

        public void Initialize(IServiceRegistry services)
        {
            Debug.Log("[MatchingPair] Module.Initialize -> resolving catalog");
            services.Resolve<IMatchingPairCatalog>();
        }
    }
}
