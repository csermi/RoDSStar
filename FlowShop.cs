using System;
using System.Linq;

namespace RoDSStar
{
    public enum Product
    {
        GYB,
        FB,
        SB
    }

    internal class FlowShop
    {
        // product items can be packed to lots for faster calculation
        public static int MaxLotSize = 1;

        // define machine count / stage and product operation time / stage
        public Stage[] Stages { get; } =
        {
            new Stage("Vago", 6, new []{5, 8, 6}),
            new Stage("Hajlito", 2, new []{10, 16, 15}),
            new Stage("Hegeszto", 3, new []{8, 12, 10}),
            new Stage("Tesztelo", 1, new []{5, 5, 5}),
            new Stage("Festo", 4, new []{12, 20, 15}),
            new Stage("Csomagolo", 3, new []{10, 15, 12}),
        };

        public Job[] Jobs { get; }

        public int[] Order { get; private set; }

        public FlowShop(Job[] jobs)
        {
            Jobs = jobs;
            foreach (var job in jobs)
            {
                job.SetProcessingTimes(Stages);
            }
            Order = Enumerable.Range(0, Jobs.Length).ToArray();
        }

        public void InitStages()
        {
            foreach (var stage in Stages)
            {
                stage.InitMachines();
            }
        }

        public void SetOrder(int[] order)
        {
            Order = order;
        }

        public int GetTotalProfitWithoutPenalty()
        {
            var result = 0;
            foreach (var job in Jobs)
            {
                result += job.ProfitPerPiece * job.Quantity;
            }

            return result;
        }

        public Result Calculate()
        {
            var totalPenalty = 0;
            foreach (var jobIdx in Order)
            {
                var job = Jobs[jobIdx];
                var jobReady = -1;

                for (int lotIdx = 1; lotIdx < (job.Quantity - 1) / MaxLotSize + 2; lotIdx++)
                {
                    var lotSize = lotIdx * MaxLotSize > job.Quantity ? lotIdx * MaxLotSize - job.Quantity : MaxLotSize;
                    int lotReady = 0;
                    foreach (var stage in Stages)
                    {
                        var minutes = stage.ProductMinutes[(int)job.Product] * lotSize;
                        var machineReady = stage.MachineReady[stage.NextMachineIdx];
                        lotReady = Math.Max(machineReady, lotReady) + minutes;
                        stage.MachineReady[stage.NextMachineIdx] = lotReady;
                        stage.NextMachine();
                        if (lotReady > jobReady)
                        {
                            jobReady = lotReady;
                        }
                   }
                }

                var jobPenalty = CommonTime.GetNumberOfDelayDays(job.DueDateMinutes, jobReady) * job.PenaltyPerDay;
                totalPenalty = totalPenalty + jobPenalty;
            }

            var result = new Result()
            {
                TotalPenalty = totalPenalty,
                Makespan = CommonTime.ToDateTime(Stages.Last().MachineReady.Max(), false)
            };
            return result;
        }

        public Result Export(FlowShopExporter exporter)
        {
            var totalPenalty = 0;
            foreach (var jobIdx in Order)
            {
                var job = Jobs[jobIdx];
                var jobStart = -1;
                var jobReady = -1;

                for (int lotIdx = 1; lotIdx < (job.Quantity - 1) / MaxLotSize + 2; lotIdx++)
                {
                    var lotSize = lotIdx * MaxLotSize > job.Quantity ? lotIdx * MaxLotSize - job.Quantity : MaxLotSize;
                    var lotReady = 0;
                    foreach (var stage in Stages)
                    {
                        var minutes = stage.ProductMinutes[(int)job.Product] * lotSize;
                        var machineReady = stage.MachineReady[stage.NextMachineIdx];
                        lotReady = Math.Max(machineReady, lotReady) + minutes;
                        stage.MachineReady[stage.NextMachineIdx] = lotReady;

                        exporter.AddMachineExportLine(stage, job, lotReady, minutes);

                        stage.NextMachine();
                        if (lotReady > jobReady)
                        {
                            jobReady = lotReady;
                        }
                        if (jobStart == -1)
                        {
                            jobStart = lotReady - minutes;
                        }
                    }
                }

                var jobPenalty = CommonTime.GetNumberOfDelayDays(job.DueDateMinutes, jobReady) * job.PenaltyPerDay;
                totalPenalty = totalPenalty + jobPenalty;

                exporter.AddJobExportLine(job, jobPenalty, jobStart, jobReady);
            }

            var result = new Result()
            {
                TotalPenalty = totalPenalty,
                Makespan = CommonTime.ToDateTime(Stages.Last().MachineReady.Max(), false)
            };
            return result;
        }
    }
}
