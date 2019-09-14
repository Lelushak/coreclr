using System;
using System.Threading;

namespace ConsoleApp9
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var thr = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        Console.WriteLine(".");
                        Thread.Sleep(500);
                    }
                }
                catch (ThreadAbortException)
                {
                    Console.WriteLine("Thread aborted!");
                    throw;
                }
            });
            
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Console.WriteLine(asm.Location);
            }

            thr.Start();
            Thread.Sleep(2000);
            thr.Abort();
        }
    }
}
