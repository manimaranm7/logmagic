﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace LogMagic.Receivers
{
   /// <summary>
   /// Base class for async logger where writing procedure doesn't blobk the call and sends messages in
   /// a separate thread.
   /// </summary>
   public abstract class AsyncReceiver : ILogReceiver
   {
      private readonly ConcurrentQueue<LogChunk> _messageQueue = new ConcurrentQueue<LogChunk>();
      private readonly Thread _dispatchThread;
      private bool _disposed;

      /// <summary>
      /// Creates class instance
      /// </summary>
      protected AsyncReceiver()
      {
         _dispatchThread = new Thread(DispatchThreadEntry) {IsBackground = true, Priority = ThreadPriority.Lowest};
         _dispatchThread.Start();
      }

      /// <summary>
      /// Enqueues the chunk
      /// </summary>
      /// <param name="chunk">Chunk</param>
      public virtual void Send(LogChunk chunk)
      {
         _messageQueue.Enqueue(chunk);
      }

      /// <summary>
      /// Disposes the class
      /// </summary>
      public virtual void Dispose()
      {
         _disposed = true;
      }

      private void DispatchThreadEntry(object state)
      {
         var container = new List<LogChunk>(50);

         while (!_disposed)
         {
            container.Clear();

            LogChunk chunk;
            while(_messageQueue.TryDequeue(out chunk))
            {
               container.Add(chunk);

               if (container.Count == container.Capacity) break;
            }

            if (container.Count > 0)
            {
               SendChunks(container);
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));
         }
      }

      /// <summary>
      /// Sends accumulated chunks to the destination
      /// </summary>
      /// <param name="chunks">Chunks accumulated</param>
      protected abstract void SendChunks(IEnumerable<LogChunk> chunks);

   }
}