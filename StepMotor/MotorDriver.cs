using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FTD2XX_NET;

namespace StepMotor
{
    public enum Direction
    {
        Left,
        Right
    }
    public enum MotorID
    {
        First,
        Second
    }
    public enum ControlMode
    {
        HalfStep,
        FullStep,
        WaveStep
    }
    public enum RotationUnit
    {
        Step,
        Degree
    }

    public class MotorDriver : IDisposable
    {
        // Properties that control motor rotation
        
        public ControlMode Mode { get; set; } = ControlMode.FullStep;
        public Direction RotationDirection { get; set; } = Direction.Left;
        public MotorID ControlledMotor { get; set; } = MotorID.First;
        public RotationUnit Unit { get; set; } = RotationUnit.Step;
        public int Interval { get; set; } = 100;
        public int RotationValue { get; set; } = 0;

        public MotorDriver(){ }
        public void Dispose()
        {
            Disconnect();
        }
        public string Connect()
        {           
            string message = "Unable to Connect";
            if(!Connected)
            {
                try
                {
                    deviceWrapper = new FTDI();
                    uint deviceCount = 0;
                    deviceWrapper.GetNumberOfDevices(ref deviceCount);
                    FTDI.FT_DEVICE_INFO_NODE[] deviceInfo = new FTDI.FT_DEVICE_INFO_NODE[deviceCount];
                    status = deviceWrapper.GetDeviceList(deviceInfo);
                    status = deviceWrapper.OpenByDescription(deviceInfo[0].Description);
                    status = deviceWrapper.SetBitMode(0xff, 1);
                    message = status.ToString();
                    Connected = true;
                }

                catch (Exception e)
                {
                    message = "Error while trying to connect to device." +
                        "\nStatus: " + status.ToString() +
                        "\nError Message: " + e.Message;
                }

            }
            return message;
        }
        public void Disconnect()
        {
            if(Connected)
            {
                deviceWrapper = null;
                Connected = false;
            }
        }      

        
        public void Rotate()
        {
            if(Connected)
            {
                var steps = CalcualteSteps();
                for (int i = 0; i < steps; i++)
                {
                    // Generate next signal for current rotation parameters
                    byte[] signal = { NextSignal() };
                    uint bytesWritten = 0;
                    status = deviceWrapper.Write(signal, 1, ref bytesWritten);
                    Thread.Sleep(Interval);
                }
            }
          
        }
        private byte NextSignal()
        {
            switch (Mode)
            {
                case ControlMode.HalfStep:
                    return HS.NextSignal(RotationDirection);
                case ControlMode.FullStep:
                    return FS.NextSignal(RotationDirection);
                case ControlMode.WaveStep:
                    return WS.NextSignal(RotationDirection);
                default:
                    return HS.NextSignal(RotationDirection);
            }
        }
        private void StopMotor()
        {
            uint bytesWritten = 0;
            status = deviceWrapper.Write(new byte[] { 0x00 }, 1, ref bytesWritten);
        }
        private int CalcualteSteps()
        {
            int steps = RotationValue;
            if(Unit == RotationUnit.Degree)            
                steps = (int)(ControlledMotor == MotorID.First ? steps/0.75 : steps / 7.5);            
            return steps;           

        }
        private string ReadEprom()
        {
            string str = "";
            if (Connected)
            {
                var deviceInfo = new FTDI.FT232B_EEPROM_STRUCTURE();
                status = deviceWrapper.ReadFT232BEEPROM(deviceInfo);
                return deviceInfo.Description;
            }
            return str;
        }
   
        // Signal Sequences that need to be written to motor to rotate it in specific way
        // dictated by parameters

        private ControlSequence HS = ControlSequence.Create(ControlMode.HalfStep, Direction.Right, MotorID.First);
        private ControlSequence FS = ControlSequence.Create(ControlMode.FullStep, Direction.Right, MotorID.First);
        private ControlSequence WS = ControlSequence.Create(ControlMode.WaveStep, Direction.Right, MotorID.First);      
      
        private bool Connected { get; set; } = false;
        private FTDI deviceWrapper = new FTDI();
        private FTDI.FT_STATUS status;
    }
}
