using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CPUTempBigPicture
{
    public partial class GetCPUGPUInfo
    {
        public float cpuTemp = 0.0f;
        public float gpuTemp = 0.0f;
        public float gpuClock = 0.0f;
        public float cpuMax = 0.0f;
        public float cpuPow = 0.0f;
        public float gpuPow = 0.0f;

        [GeneratedRegex(".*UHD.*")]
        private static partial Regex UHD_Regex();

        [GeneratedRegex(".*CPU Package.*")]
        private static partial Regex CPUPackage_Regex();

        [GeneratedRegex(".*CPU Core #.*")]
        private static partial Regex CPUCoreNum_Regex();

        [GeneratedRegex(".*GPU Core.*")]
        private static partial Regex GPUCore_Regex();

        [GeneratedRegex(".*GPU Package.*")]
        private static partial Regex GPUPackage_Regex();

        // CPUGPUだけ表示する版
        public void DispCPUGPU()
        {
            bool cpuTempFlg = false;
            bool gpuTempFlg = false;
            float[] cpuClocks = new float[64];
            int cpuCoreCnt = 0;

            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            foreach (IHardware hardware in computer.Hardware)
            {
                // UHD Graphicsは表示しない
                if (UHD_Regex().IsMatch(hardware.Name)) break;

                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (CPUPackage_Regex().IsMatch(sensor.Name))
                    {
                        if (!cpuTempFlg && (sensor.SensorType == SensorType.Temperature))
                        {
                            cpuTemp = (float)sensor.Value;
                            cpuTempFlg = true;
                        }
                        else if(sensor.SensorType == SensorType.Power)
                        {
                            cpuPow = (float)sensor.Value;
                        }

                    }
                    else if (CPUCoreNum_Regex().IsMatch(sensor.Name))
                    {
                        if (sensor.SensorType == SensorType.Clock)
                        {
                            cpuClocks[cpuCoreCnt] = (float)sensor.Value;
                            cpuCoreCnt++;
                        }
                    }
                    else if (GPUPackage_Regex().IsMatch(sensor.Name))
                    {
                        if (sensor.SensorType == SensorType.Power)
                        {
                            gpuPow = (float)sensor.Value;
                        }
                    }
                    else if (GPUCore_Regex().IsMatch(sensor.Name))
                    {
                        if (!gpuTempFlg && (sensor.SensorType == SensorType.Temperature))
                        {
                            gpuTemp = (float)sensor.Value;
                            gpuTempFlg = true;
                        }
                        else if (sensor.SensorType == SensorType.Clock)
                        {
                            gpuClock = (float)sensor.Value;
                        }
                    }
                }
            }

            // CPUのクロック(最大値)
            for (int i = 0; i < cpuCoreCnt; i++)
            {
                if (cpuMax < cpuClocks[i])
                {
                    cpuMax = cpuClocks[i];
                }

            }

            computer.Close();

        }
        // 全部表示する版
        private string AllMonitor()
        {
            string monitorOutput = "";

            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            foreach (IHardware hardware in computer.Hardware)
            {
                monitorOutput += "Hardware: " + hardware.Name + "\n";

                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    monitorOutput += "\tSubhardware: " + subhardware.Name + "\n";

                    foreach (ISensor sensor in subhardware.Sensors)
                    {
                        monitorOutput += "\t\tSensor: " + sensor.Name + ", value: " + sensor.Value + "\n";
                    }
                }

                foreach (ISensor sensor in hardware.Sensors)
                {
                    monitorOutput += "\tSensor: " + sensor.Name + ", value: " + sensor.Value + "\n";
                }
            }

            computer.Close();
            return monitorOutput;
        }

        //Disposeメソッドを実装
        public void Dispose()
        {

        }
    }
}
