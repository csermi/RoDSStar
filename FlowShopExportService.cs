using System;
using System.IO;

namespace RoDSStar
{
    internal static class FlowShopExportService
    {
        public static void Export(FlowShop flowShop, int[] solution, string jobExportPath, string machineExportPath)
        {
            flowShop.SetOrder(solution);
            flowShop.InitStages();

            var exporter = new FlowShopExporter();
            flowShop.Export(exporter);

            File.WriteAllText(jobExportPath, exporter.JobExportContent.ToString()); 
            File.WriteAllText(machineExportPath, exporter.MachineExportContent.ToString());

            Console.WriteLine($"Best solution exported to files {jobExportPath} and {machineExportPath}");
        }
    }
}
