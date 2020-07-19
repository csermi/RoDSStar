using System;

namespace RoDSStar
{
    public class Result
    {
        public int TotalTardiness { get; set; }

        public DateTime Makespan { get; set; }

        public override string ToString()
        {
            return $"Total weighted tardiness: {TotalTardiness}; Makespan: {Makespan}";
        }
    }
}
