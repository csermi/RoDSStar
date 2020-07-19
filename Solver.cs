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
                    TotalPenalty = int.MaxValue,
                }
            };

            var totalProfitWithoutPenalty = _flowShop.GetTotalProfitWithoutPenalty();

            Common.MaxLotSize = 2;
            RunHeuristicNeh("NEH", job => -job.ProcessingTime);

            Common.MaxLotSize = 2;
            RunHeuristicNeh("NEHedd", job => job.DueDateMinutes);

            Common.MaxLotSize = 1;
            var result = RunSimulation(_bestSolution.Order);
            Console.WriteLine("");
            Console.WriteLine($"Best solution: {result}; Total profit - penalty: {totalProfitWithoutPenalty - result.TotalPenalty}  LotSize: {Common.MaxLotSize}; Order: {string.Join(" ", _bestSolution.Order)}");

            return _bestSolution.Order;
        }

        private void RunHeuristicNeh(string desc, Func<Job, double> initOrderFunc)
        {
            var swNeh = new Stopwatch();
            swNeh.Start();
            var nehInitialOrder = _flowShop.Jobs.OrderBy(initOrderFunc).Select(j => _flowShop.Jobs.ToList().IndexOf(j)).ToArray();
            var nehOrder = RunHeuristicNehByTotalPenalty(nehInitialOrder);
            swNeh.Stop();

            var swSim = new Stopwatch();
            swSim.Start();
            var result = RunSimulation(nehOrder);
            swSim.Stop();
      
            Console.WriteLine($"{desc}; {result}; LotSize: {Common.MaxLotSize}; Order: {string.Join(" ", nehOrder)}; NEH time: {swNeh.ElapsedMilliseconds}; Simulation time: {swSim.ElapsedMilliseconds}");

            if (result.TotalPenalty < _bestSolution.Result.TotalPenalty)
            {
                _bestSolution.Result = result;
                _bestSolution.Order = nehOrder;
            }
        }

        private int[] RunHeuristicNehByTotalPenalty(int[] order)
        {
            var resultGenerator = new List<int>();
            var currentOrder = new List<int>();

            foreach (var jobIdx in order)
            {
                int bestTotalPenalty = int.MaxValue;
                int bestIdx = -1;

                for (int idx = 0; idx <= currentOrder.Count; idx++)
                {
                    currentOrder.Insert(idx, jobIdx);
                    _flowShop.SetOrder(currentOrder.ToArray());
                    _flowShop.InitStages();
                    var totalPenalty = _flowShop.Calculate().TotalPenalty;
                    if (totalPenalty < bestTotalPenalty)
                    {
                        bestTotalPenalty = totalPenalty;
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
