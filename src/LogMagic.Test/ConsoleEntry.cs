﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogMagic.Test
{
   public static class ConsoleEntry
   {
      private static ILog _log = L.G(typeof(ConsoleEntry));

      public static void Main()
      {
         L.Config
            .WriteTo.PoshConsole()
            .WriteTo.Seq(new Uri("http://192.168.137.1:5341"))
            .EnrichWith.ThreadId()
            .EnrichWith.Constant("client", "testcon");

         _log.D("test");

         Console.ReadLine();
      }
   }
}
