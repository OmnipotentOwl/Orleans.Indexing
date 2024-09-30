using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Core;
using Orleans.Indexing.TestInjection;
using Orleans.Runtime;
using Orleans.Serialization.TypeSystem;
using Orleans.Services;

namespace Orleans.Indexing
{
    /// <summary>
    /// This class is instantiated internally only in the Silo.
    /// </summary>
    internal class SiloIndexManager : IndexManager, ILifecycleParticipant<ISiloLifecycle>
    {
        internal SiloAddress SiloAddress => this.Silo.SiloAddress;

        // Note: this.Silo must not be called until the Silo ctor has returned to the ServiceProvider which then
        // sets the Singleton; if called during the Silo ctor, the Singleton is not found so another Silo is
        // constructed. Thus we cannot have the Silo on the IndexManager ctor params or retrieve it during
        // IndexManager ctor, because ISiloLifecycle participants are constructed during the Silo ctor.
        internal Silo Silo => this._silo ??= this.ServiceProvider.GetRequiredService<Silo>();
        private Silo _silo;

        internal IInjectableCode InjectableCode { get; }

        internal IGrainReferenceRuntime GrainReferenceRuntime { get; }

        internal IGrainServiceFactory GrainServiceFactory { get; }


        public SiloIndexManager(IServiceProvider sp, IGrainFactory gf, ILoggerFactory lf, TypeResolver tr, IOptions<GrainTypeOptions> gtr)
            : base(sp, gf, lf, tr, gtr)
        {
            this.InjectableCode = this.ServiceProvider.GetService<IInjectableCode>() ?? new ProductionInjectableCode();
            this.GrainReferenceRuntime = this.ServiceProvider.GetRequiredService<IGrainReferenceRuntime>();
            this.GrainServiceFactory = this.ServiceProvider.GetRequiredService<IGrainServiceFactory>();
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(this.GetType().FullName, ServiceLifecycleStage.ApplicationServices, ct => base.OnStartAsync(ct), ct => base.OnStopAsync(ct));
        }

        internal Task<Dictionary<SiloAddress, SiloStatus>> GetSiloHosts(bool onlyActive = false)
            => this.GrainFactory.GetGrain<IManagementGrain>(0).GetHosts(onlyActive);

        public GrainReference MakeGrainServiceGrainReference(int typeData, string systemGrainId, SiloAddress siloAddress)
        {
            var grainId = SystemTargetGrainId.CreateGrainServiceGrainId(typeData, systemGrainId, siloAddress);
            return (GrainReference)this.GrainFactory.GetGrain(grainId);
            //    =>
            //this.GrainServiceFactory.MakeGrainServiceReference(typeData, systemGrainId, siloAddress);
        }
            

        internal T GetGrainService<T>(GrainReference grainReference) where T : IGrainService
            =>
                this.GrainServiceFactory.CastToGrainServiceReference<T>(grainReference);

        internal IStorage<TGrainState> GetStorageBridge<TGrainState>(Grain grain, string storageName) where TGrainState : class, new()
            => new StateStorageBridge<TGrainState>(grain.GetType().FullName, grain.GrainContext, IndexUtils.GetGrainStorage(this.ServiceProvider, storageName));
    }
}
