using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RoDSStar
{
    internal class Solver
    {
        private FlowShop _flowShop;

        private Solution _bestSolution;

        public Solver(FlowShop flowShop)
        {
            _flowShop = flowShop;
        }

        public int[] Solve()
        {
            _bestSolution = new Solution()
            {
                Result = new Result()
                {
                    TotalTardiness = int.MaxValue,
                }
            };

            Common.MaxLotSize = 2;
            RunHeuristicNeh("NEH", job => -job.ProcessingTime);

            Common.MaxLotSize = 10;
            RunHeuristicNeh("NEH", job => -job.ProcessingTime);


            Common.MaxLotSize = 2;
            RunHeuristicNeh("NEHedd", job => job.DueDateMinutes);

            Common.MaxLotSize = 10;
            RunHeuristicNeh("NEHedd", job => job.DueDateMinutes);
 
            
            Common.MaxLotSize = 1;
            var swSim = new Stopwatch();
            swSim.Start();
            var result = RunSimulation(_bestSolution.Order);
            swSim.Stop();
            Console.WriteLine("");
            Console.WriteLine($"Best solution with LotSize = 1: {result}; LotSize: {Common.MaxLotSize}; Order: {string.Join(" ", _bestSolution.Order)}; Simulation time: {swSim.ElapsedMilliseconds}");

            return _bestSolution.Order;
        }

        private void RunHeuristicNeh(string desc, Func<Job, double> initOrderFunc)
        {
            var swNeh = new Stopwatch();
            swNeh.Start();
            var nehInitialOrder = _flowShop.Jobs.OrderBy(initOrderFunc).Select(j => _flowShop.Jobs.ToList().IndexOf(j)).ToArray();
            var nehOrder = RunHeuristicNehByTotalTardiness(nehInitialOrder);
            swNeh.Stop();

            var swSim = new Stopwatch();
            swSim.Start();
            var result = RunSimulation(nehOrder);
            swSim.Stop();
      
            Console.WriteLine($"{desc}; {result}; LotSize: {Common.MaxLotSize}; Order: {string.Join(" ", nehOrder)}; NEH time: {swNeh.ElapsedMilliseconds}; Simulation time: {swSim.ElapsedMilliseconds}");

            if (result.TotalTardiness < _bestSolution.Result.TotalTardiness)
            {
                _bestSolution.Result = result;
                _bestSolution.Order = nehOrder;
            }
        }

        private int[] RunHeuristicNehByTotalTardiness(int[] order)
        {
            var resultGenerator = new List<int>();
            var currentOrder = new List<int>();

            foreach (var jobIdx in order)
            {
                int bestTotalTardiness = int.MaxValue;
                int bestIdx = -1;

                for (int idx = 0; idx <= currentOrder.Count; idx++)
                {
                    currentOrder.Insert(idx, jobIdx);
                    _flowShop.SetOrder(currentOrder.ToArray());
                    _flowShop.InitStages();
                    var totalTardiness = _flowShop.Calculate().TotalTardiness;
                    if (totalTardiness < bestTotalTardiness)
                    {
                        bestTotalTardiness = totalTardiness;
                        bestIdx = idx;
                    }

                    currentOrder.RemoveAt(idx);
                }
                currentOrder.Insert(bestIdx, jobIdx);
                resultGenerator.Add(bestIdx);
            }

            var finalOrder = new List<int>();
            for (int idx = 0; idx < order.Length; idx++)
            {
                finalOrder.Insert(resultGenerator[idx], order[idx]);
            }

            return finalOrder.ToArray();
        }

        private Result RunSimulation(int[] nehOrder)
        {
            _flowShop.SetOrder(nehOrder);
            _flowShop.InitStages();
            return _flowShop.Calculate();
        }
    }
}
