using System.Text;

namespace RoDSStar
{
    internal class FlowShopExporter
    {
        private const string DateTimeFormatFull = "yyyy.MM.dd HH:mm";
        private const string DateTimeFormatOnlyDate = "yyyy.MM.dd";
        private const string DateTimeFormatOnlyTime = "HH:mm";

        public StringBuilder JobExportContent { get; } = new StringBuilder();
        public StringBuilder MachineExportContent { get; } = new StringBuilder();

        public FlowShopExporter()
        {
            // add headers
            JobExportContent.AppendLine(string.Join(",", new[]
              {
                  "Megrendelesszam",
                  "Profit osszesen",
                  "Levont kotber",
                  "Munka megkezdese",
                  "Keszre jelentes ideje",
                  "Megrendeles eredeti hatarideje"
            }));

            MachineExportContent.AppendLine(string.Join(",", new[]
            {
                "Datum",
                "Gep",
                "Kezdo idopont",
                "Zaro idopont",
                "Megrendelesszam"
           }));
        }

        public void AddMachineExportLine(Stage stage, Job job, int lotReady, int minutes)
        {
            var machineId = $"{stage.Id}-{stage.NextMachineIdx + 1}";
            var lotStart = lotReady - minutes;
            var startDay = lotStart / Common.TotalMinutesInADay;
            var readyDay = lotReady / Common.TotalMinutesInADay;

            if (startDay == readyDay)
            {
                AddMachineExportLine(lotStart, lotReady, machineId, job.Id);
            }
            else
            {
                // job ends on other day
                var dayEnding = (startDay + 1) * Common.TotalMinutesInADay;
                AddMachineExportLine(lotStart, dayEnding, machineId, job.Id );
                AddMachineExportLine(dayEnding, lotReady, machineId, job.Id );
            }
        }
        
        public void AddJobExportLine(Job job, int tardiness, int jobBegin, int jobReady)
        {
            JobExportContent.AppendLine(string.Join(",", new[]
            {
                job.Id,
                (job.Quantity * job.ProfitPerPiece).ToString(),
                tardiness.ToString(),
                Common.ToDateTime(jobBegin, true).ToString(DateTimeFormatFull),
                Common.ToDateTime(jobReady, false).ToString(DateTimeFormatFull),
                Common.ToDateTime(job.DueDateMinutes, false).ToString(DateTimeFormatFull)
            }));
        }

        private void AddMachineExportLine(int lotStart, int lotReady, string machineId, string jobId)
        {
            MachineExportContent.AppendLine(string.Join(",", new[]
            {
                Common.ToDateTime(lotStart, true).ToString(DateTimeFormatOnlyDate),
                machineId,
                Common.ToDateTime(lotStart, true).ToString(DateTimeFormatOnlyTime),
                Common.ToDateTime(lotReady, false).ToString(DateTimeFormatOnlyTime),
                jobId
            }));
        }

    }
}
