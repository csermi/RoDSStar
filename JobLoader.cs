using System;
using System.Collections.Generic;
using System.IO;

namespace RoDSStar
{
    internal static class JobLoader
    {
        private const int CSV_JobId = 0;
        private const int CSV_ProductId = 1;
        private const int CSV_Quantity = 2;
        private const int CSV_DueDate = 3;
        private const int CSV_Profit = 4;
        private const int CSV_Penalty = 5;

        public static Job[] Load(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception($"File does not exist [{path}]");
            }

            var jobs = new List<Job>();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var items = line.Split(',');

                if (items.Length < 6)
                {
                    throw new Exception($"Input file {path} must contain lines with at least 6 items.");
                }

                if (!Enum.TryParse(items[CSV_ProductId], out Product product))
                {
                    throw new Exception($"Input file {path} contains invalid product id: {items[CSV_ProductId]}");
                }
                if (!int.TryParse(items[CSV_Quantity], out var quantity))
                {
                    throw new Exception($"Input file {path} contains invalid number in quantity field: {items[CSV_Quantity]}");
                }
                if (!DateTime.TryParse(items[CSV_DueDate], out var dueDate))
                {
                    throw new Exception($"Input file {path} contains invalid date in due date field: {items[CSV_DueDate]}");
                }
                if (!int.TryParse(items[CSV_Profit], out var profitPerPiece))
                {
                    throw new Exception($"Input file {path} contains invalid number in profit field: {items[CSV_Profit]}");
                }
                if (!int.TryParse(items[CSV_Penalty], out var penaltyPerDay))
                {
                    throw new Exception($"Input file {path} contains invalid number in penalty field: {items[CSV_Penalty]}");
                }

                var job = new Job(items[CSV_JobId], product, quantity, dueDate, profitPerPiece, penaltyPerDay);
                jobs.Add(job);
            }
            return jobs.ToArray();
        }
    }
}
