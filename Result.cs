using System;

namespace RoDSStar
{
    public class Result
    {
        public int TotalPenalty { get; set; }

        public DateTime Makespan { get; set; }

        public override string ToString()
        {
            return $"Total penalty: {TotalPenalty}; Makespan: {Makespan}";
        }
    }
}
