using System;
using System.Collections.Generic;

namespace RoDSStar
{
    internal class Job
    {
        public string Id { get; }

        public Product Product { get; }

        public int Quantity { get; }

        public int DueDateMinutes { get; }

        public double ProcessingTime { get; private set; }

        public int ProfitPerPiece { get;} 

        public int PenaltyPerDay { get; }

        public Job(string id, Product product, int quantity, DateTime dueDate, int profitPerPiece, int penaltyPerDay)
        {
            Id = id; 
            Product = product;
            Quantity = quantity;
            DueDateMinutes = CommonTime.ToMinutes(dueDate);
            ProfitPerPiece = profitPerPiece;
            PenaltyPerDay = penaltyPerDay;
        }

        public void SetProcessingTimes(IEnumerable<Stage> stages)
        {
            ProcessingTime = 0.0f;
            foreach (var stage in stages)
            {
                ProcessingTime += (double)stage.ProductMinutes[(int)Product] * Quantity / stage.MachineCount;
            }
        }
    }
}
