using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Serialization.TypeSystem;

namespace Orleans.Indexing
{
    /// <summary>
    /// This class may be instantiated internally in the ClusterClient as well as in the Silo.
    /// </summary>
    internal class IndexManager : ILifecycleParticipant<IClusterClientLifecycle>
    {
        internal HashSet<Type> RegisteredGrainClassTypes { get; }
        internal TypeResolver CachedTypeResolver { get; }

        internal IndexRegistry IndexRegistry { get; private set; }

        // Explicit dependency on ServiceProvider is needed so we can retrieve SiloIndexManager.__silo after ctor returns; see comments there.
        // Also, in some cases this is passed through non-injected interfaces such as Hash classes.
        internal IServiceProvider ServiceProvider { get; }

        internal IGrainFactory GrainFactory { get; }

        // Note: For similar reasons as SiloIndexManager.__silo, __indexFactory relies on 'this' to have returned from its ctor.
        internal IndexFactory IndexFactory => this.__indexFactory ??= this.ServiceProvider.GetRequiredService<IndexFactory>();
        private IndexFactory __indexFactory;

        internal ILoggerFactory LoggerFactory { get; }

        public IndexManager(IServiceProvider sp, IGrainFactory gf, ILoggerFactory lf, TypeResolver typeResolver, IOptions<GrainTypeOptions> grainTypeOptions)
        {
            this.ServiceProvider = sp;
            this.GrainFactory = gf;
            this.LoggerFactory = lf;
            this.CachedTypeResolver = typeResolver;
            this.RegisteredGrainClassTypes = grainTypeOptions.Value.Classes;
            this.IndexingOptions = this.ServiceProvider.GetOptionsByName<IndexingOptions>(IndexingConstants.INDEXING_OPTIONS_NAME);
        }

        public void Participate(IClusterClientLifecycle lifecycle)
        {
            if (this is not SiloIndexManager)
            {
                lifecycle.Subscribe(this.GetType().FullName, ServiceLifecycleStage.ApplicationServices, this.OnStartAsync, this.OnStopAsync);
            }
        }

        /// <summary>
        /// This method must be called after all application parts have been loaded.
        /// </summary>
        public virtual Task OnStartAsync(CancellationToken ct)
        {
            if (this.IndexRegistry == null)
            {
                this.IndexRegistry = new ApplicationPartsIndexableGrainLoader(this).CreateIndexRegistry();
            }
            return Task.CompletedTask;
        }

        public IndexingOptions IndexingOptions { get; }

        internal int NumWorkflowQueuesPerInterface => this.IndexingOptions.NumWorkflowQueuesPerInterface;

        /// <summary>
        /// This method is called at the begining of the process of uninitializing runtime services.
        /// </summary>
        public virtual Task OnStopAsync(CancellationToken ct) => Task.CompletedTask;

        internal static IndexManager GetIndexManager(ref IndexManager indexManager, IServiceProvider serviceProvider)
            => indexManager ??= GetIndexManager(serviceProvider);

        internal static IndexManager GetIndexManager(IServiceProvider serviceProvider)
            => serviceProvider.GetRequiredService<IndexManager>();

        internal static SiloIndexManager GetSiloIndexManager(ref SiloIndexManager siloIndexManager, IServiceProvider serviceProvider)
            => siloIndexManager ??= GetSiloIndexManager(serviceProvider);

        internal static SiloIndexManager GetSiloIndexManager(IServiceProvider serviceProvider)
            => (SiloIndexManager)serviceProvider.GetRequiredService<IndexManager>();    // Throws an invalid cast operation if we're not on a Silo
    }
}
