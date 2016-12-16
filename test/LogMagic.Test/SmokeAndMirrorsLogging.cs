﻿using Xunit;
using System.IO;
using System;

namespace LogMagic.Test
{
   public class AzureBlobWriterTest : SmokeAndMirrorsLogging
   {
      public AzureBlobWriterTest() : base("azure-blob")
      {

      }
   }

   public class AzureTableWriterTest : SmokeAndMirrorsLogging
   {
      public AzureTableWriterTest() : base("azure-table")
      {

      }
   }

   public class FilesWriterTest : SmokeAndMirrorsLogging
   {
      public FilesWriterTest() : base("files")
      {

      }
   }

   public class ConsoleWriterTest : SmokeAndMirrorsLogging
   {
      public ConsoleWriterTest() : base("console")
      {

      }
   }

   public class PoshConsoleWriterTest : SmokeAndMirrorsLogging
   {
      public PoshConsoleWriterTest() : base("posh-console")
      {

      }
   }

   public class TraceWriterTest : SmokeAndMirrorsLogging
   {
      public TraceWriterTest() : base("trace")
      {

      }
   }

   public abstract class SmokeAndMirrorsLogging : AbstractTestFixture
   {
      private readonly string _receiverName;

      protected SmokeAndMirrorsLogging(string receiverName)
      {
         L.Config.ClearWriters();
         var settings = new TestSettings();

         switch (_receiverName)
         {
            case "azure-blob":
               L.Config.WriteTo.AzureAppendBlob(settings.AzureStorageName, settings.AzureStorageKey,
                  "logs-integration", "smokeandmirrors");
               break;
            case "azure-table":
               L.Config.WriteTo.AzureTable(settings.AzureStorageName, settings.AzureStorageKey,
                  "logsintegration");
               break;
            case "files":
               L.Config.WriteTo.File(Path.Combine(TestDir.FullName, "subdir", "testlog.txt"));
               break;
            case "console":
               L.Config.WriteTo.Console();
               break;
            case "posh-console":
               L.Config.WriteTo.PoshConsole();
               break;
            case "trace":
               L.Config.WriteTo.Trace();
               break;
         }   
      }

      [Fact]
      public void Smoke_SomethingSimple_DoestCrash()
      {
         ILog log = L.G();
         ILog _log = L.G();

         log.D("hello!");
         log.D("exception is here!", new Exception("test exception"));

         L.Flush();
         //Thread.Sleep(TimeSpan.FromMinutes(1));
      }
   }
}