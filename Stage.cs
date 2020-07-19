namespace RoDSStar
{
    internal class Stage
    {
        public string Id { get; }
        
        public int[] MachineReady { get; }

        public int[] ProductMinutes { get; }

        public int NextMachineIdx { get; private set; }

        public int MachineCount { get; }

        public Stage(string id, int machineCount, int[] productMinutes)
        {
            Id = id;
            MachineCount = machineCount;
            MachineReady = new int[machineCount];
            ProductMinutes = productMinutes;
        }

        public void InitMachines()
        {
            for (int idx = 0; idx < MachineCount; idx++)
            {
                MachineReady[idx] = 0;
            }

            NextMachineIdx = 0;
        }

        public void NextMachine()
        {
            NextMachineIdx = (NextMachineIdx + 1) % MachineCount;
        }
    }
}
