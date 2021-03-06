﻿using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogMagic.Microsoft.Azure.ServiceFabric.Remoting
{
   public class CorrelatingServiceProxyFactory : IServiceProxyFactory
   {
      private readonly ServiceProxyFactory _serviceProxyFactory;

      public CorrelatingServiceProxyFactory(Func<IServiceRemotingCallbackMessageHandler, IServiceRemotingClientFactory> createServiceRemotingClientFactory = null, OperationRetrySettings retrySettings = null)
      {
         _serviceProxyFactory = new ServiceProxyFactory(
            (callbackClient) =>
            {
               if (createServiceRemotingClientFactory == null)
               {
                  return new CorrelatingFabricTransportServiceRemotingClientFactory(inner: null);
               }

               IServiceRemotingClientFactory innerClientFactory = createServiceRemotingClientFactory(callbackClient);

               if (innerClientFactory is CorrelatingFabricTransportServiceRemotingClientFactory) return innerClientFactory;

               return new CorrelatingFabricTransportServiceRemotingClientFactory(inner: innerClientFactory);

            },
            retrySettings);
      }

      public TServiceInterface CreateServiceProxy<TServiceInterface>(Uri serviceUri, ServicePartitionKey partitionKey = null, TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, string listenerName = null) where TServiceInterface : IService
      {
         TServiceInterface proxy =
            _serviceProxyFactory.CreateServiceProxy<TServiceInterface>(serviceUri, partitionKey, targetReplicaSelector,
               listenerName);

         MethodResolver.AddMethodsForProxyOrService(proxy.GetType().GetInterfaces(), typeof(IService));

         return proxy;
      }
   }
}
