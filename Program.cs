using System;
using System.Diagnostics;

namespace RoDSStar
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine($"One arguments is expected (input file path), but {args.Length} received.");
                return;
            }

            var swTotal = new Stopwatch();
            swTotal.Start();
            try
            {
                var jobs = JobLoader.Load(args[0]);

                var flowShop = new FlowShop(jobs);
                var solver = new Solver(flowShop);
                var solution = solver.Solve();

                FlowShopExportService.Export(flowShop, solution, "jobExport.txt", "machineExport.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            swTotal.Stop();

            Console.WriteLine("");
            Console.WriteLine($"Total ellapsed time: {swTotal.ElapsedMilliseconds} ms");
        }
    }
}
