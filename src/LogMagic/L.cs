﻿using System;
using System.Collections.Generic;
using LogMagic.Configuration;

namespace LogMagic
{
   /// <summary>
   /// Global public logging configuration and initialisation class
   /// </summary>
   public static class L
   {

      /// <summary>
      /// Gets logging library configuration
      /// </summary>
      public static ILogConfiguration Config { get; } = new LogConfiguration();
      /// <summary>
      /// Get logger for the specified type
      /// <typeparam name="T">Class type</typeparam>
      /// </summary>
      public static ILog G<T>()
      {
         return new LogClient(Config, typeof(T).FullName);
      }

      /// <summary>
      /// Get logger for the specified type
      /// </summary>
      public static ILog G(Type t)
      {
         return new LogClient(Config, t.FullName);
      }

      /// <summary>
      /// Gets logger by specified name. Use when you can't use more specific methods.
      /// </summary>
      public static ILog G(string name)
      {
         return new LogClient(Config, name);
      }

#if !NET45

      /// <summary>
      /// Adds one or more context properties.
      /// </summary>
      /// <param name="properties">
      /// Array or properties where even numbers are property names and odd numbers are property values.
      /// If you have an odd number of array elements the last one is discarded.
      /// </param>
      public static IDisposable Context(params string[] properties)
      {
         if (properties == null || properties.Length < 2) return null;

         var d = new Dictionary<string, string>();

         int maxLength = properties.Length - properties.Length % 2;
         for(int i = 0; i < maxLength; i += 2)
         {
            d[properties[i]] = properties[i + 1];
         }

         return Config.Context.Push(d);
      }

      /// <summary>
      /// Adds a context property
      /// </summary>
      public static IDisposable Context(Dictionary<string, string> properties)
      {
         if (properties == null || properties.Count == 0) return null;

         return Config.Context.Push(properties);
      }

      /// <summary>
      /// Gets a context property by name
      /// </summary>
      /// <param name="propertyName">Property name</param>
      /// <returns></returns>
      public static string GetContextValue(string propertyName)
      {
         if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

         return Config.Context.GetValueByName(propertyName);
      }

      /// <summary>
      /// Gets a dictionary of all current context values
      /// </summary>
      /// <returns></returns>
      public static Dictionary<string, string> GetContextValues()
      {
         return Config.Context.GetAllValues();
      }

#endif

   }
}
