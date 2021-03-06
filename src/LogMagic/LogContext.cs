﻿using System;
using System.Collections.Concurrent;
using LogMagic.Enrichers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LogMagic
{
   static class LogContext
   {
      private static readonly AsyncLocal<ConcurrentDictionary<string, IEnricher>> Data =
         new AsyncLocal<ConcurrentDictionary<string, IEnricher>>();

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

      public static ConcurrentDictionary<string, IEnricher> Enrichers
      {
         get => Data.Value;
         set => Data.Value = value;
      }

      public static string GetValueByName(string name)
      {
         var enrichers = Enrichers;

         if (enrichers == null) return null;

         ConstantEnricher enricher = enrichers
            .Values
            .Cast<ConstantEnricher>()
            .FirstOrDefault(e => e.Name == name);

         if (enricher == null) return null;

         return enricher.Value;
      }

      public static Dictionary<string, string> GetAllValues()
      {
         var enrichers = Enrichers;

         if (enrichers == null) return new Dictionary<string, string>();

         return enrichers
            .Values
            .Cast<ConstantEnricher>()
            .ToDictionary(c => c.Name, c => c.Value);
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