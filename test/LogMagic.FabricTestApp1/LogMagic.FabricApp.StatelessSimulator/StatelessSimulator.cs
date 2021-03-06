﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogMagic.FabricApp.ActorSimulator.Interfaces;
using LogMagic.FabricApp.Interfaces;
using LogMagic.Microsoft.Azure.ServiceFabric;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace LogMagic.FabricApp.StatelessSimulator
{
   /// <summary>
   /// An instance of this class is created for each service instance by the Service Fabric runtime.
   /// </summary>
   internal sealed class StatelessSimulator : StatelessService
   {
      public StatelessSimulator(StatelessServiceContext context)
          : base(context)
      { }

      /// <summary>
      /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
      /// </summary>
      /// <returns>A collection of listeners.</returns>
      protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
      {
         return new ServiceInstanceListener[0];
      }

      /// <summary>
      /// This is the main entry point for your service instance.
      /// </summary>
      /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
      protected override async Task RunAsync(CancellationToken cancellationToken)
      {
         using (L.Operation())
         {
            using (L.Context("p1".PairedWith("v1"), "p2".PairedWith("v2")))
            {
               IStatefulSimulatorService proxy = CorrelatingServiceProxy.Create<IStatefulSimulatorService>(
                  new Uri("fabric:/LogMagic.FabricTestApp/LogMagic.FabricApp.StatefulSimulator"), new ServicePartitionKey(0));

               IActorSimulator actorProxy = CorrelatingActorProxy.Create<IActorSimulator>(
                  ActorId.CreateRandom(),
                  new Uri("fabric:/LogMagic.FabricTestApp/ActorSimulatorActorService"));

               await proxy.InvokeTest();

               await actorProxy.SetCountAsync(5, cancellationToken);
            }
         }
      }
   }
}
