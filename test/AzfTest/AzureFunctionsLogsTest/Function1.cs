using System;
using LogMagic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsLogsTest
{
   [LogMagicInvocationFilter]
   public static class Function1
   {
      private static readonly ILog log = L.G(typeof(Function1));

      static Function1()
      {
         L.Config
            .WriteTo.PoshConsole();

         string aiKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
         if (aiKey != null)
         {
            L.Config.WriteTo.AzureApplicationInsights(aiKey);
         }
      }

      [FunctionName("Function1")]
      public static void Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger logger)
      {
         string aiKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");

         logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}, ai key: {aiKey}");

         log.Trace("logmagic traces hello");

         log.Event("logmagic custom event");
      }
   }
}