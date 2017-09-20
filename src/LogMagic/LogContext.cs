﻿using System;
using System.Collections.Concurrent;
using LogMagic.Enrichers;
using System.Collections.Generic;
using System.Linq;
#if NET45
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
#else
using System.Threading;
#endif

namespace LogMagic
{
   static class LogContext
   {
#if NET45
      static readonly string DataSlotName = typeof(LogContext).FullName + "@" + Guid.NewGuid();
#else
      private static readonly AsyncLocal<ConcurrentDictionary<string, IEnricher>> Data =
         new AsyncLocal<ConcurrentDictionary<string, IEnricher>>();
#endif

      public static IDisposable Push(IEnumerable<KeyValuePair<string, string>> properties)
      {
         ConcurrentDictionary<string, IEnricher> stack = GetOrCreateEnricherStack();
         var bookmark = new StackBookmark(Clone(stack));

         if (properties != null)
         {
            foreach (var pair in properties)
            {
               stack[pair.Key] = new ConstantEnricher(pair);
            }
         }

         Enrichers = stack;

         return bookmark;
      }

      private static ConcurrentDictionary<string, IEnricher> Clone(ConcurrentDictionary<string, IEnricher> stack)
      {
         return new ConcurrentDictionary<string, IEnricher>(stack);
      }

      private static ConcurrentDictionary<string, IEnricher> GetOrCreateEnricherStack()
      {
         var enrichers = Enrichers;

         if (enrichers == null)
         {
            enrichers = new ConcurrentDictionary<string, IEnricher>();
            Enrichers = enrichers;
         }

         return enrichers;
      }

#if NET45

      public static ConcurrentDictionary<string, IEnricher> Enrichers
      {
         get
         {
            var objectHandle = CallContext.LogicalGetData(DataSlotName) as ObjectHandle;

            return objectHandle?.Unwrap() as ConcurrentDictionary<string, IEnricher>;
         }

         set
         {
            if (CallContext.LogicalGetData(DataSlotName) is IDisposable oldHandle)
            {
               oldHandle.Dispose();
            }

            CallContext.LogicalSetData(DataSlotName, new DisposableObjectHandle(value));
         }
      }



      sealed class DisposableObjectHandle : ObjectHandle, IDisposable
      {
         static readonly ISponsor LifeTimeSponsor = new ClientSponsor();

         public DisposableObjectHandle(object o) : base(o)
         {
         }

         public override object InitializeLifetimeService()
         {
            var lease = (ILease)base.InitializeLifetimeService();
            lease.Register(LifeTimeSponsor);
            return lease;
         }

         public void Dispose()
         {
            if (GetLifetimeService() is ILease lease)
            {
               lease.Unregister(LifeTimeSponsor);
            }
         }
      }
#else
      public static ConcurrentDictionary<string, IEnricher> Enrichers
      {
         get => Data.Value;
         set => Data.Value = value;
      }
#endif

      public static string GetValueByName(string name)
      {
         var enrichers = Enrichers;

         ConstantEnricher enricher = Enrichers
            .Values
            .Cast<ConstantEnricher>()
            .FirstOrDefault(e => e.Name == name);

         if (enricher == null) return null;

         return enricher.Value;
      }

      sealed class StackBookmark : IDisposable
      {
         private readonly ConcurrentDictionary<string, IEnricher> _enrichers;

         public StackBookmark(ConcurrentDictionary<string, IEnricher> bookmark)
         {
            _enrichers = bookmark;
         }

         public void Dispose()
         {
            Enrichers = _enrichers;
         }
      }
   }
}