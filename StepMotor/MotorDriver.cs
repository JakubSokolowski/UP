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
        public ControlMode Mode { get; set; } = ControlMode.FullStep;
        public Direction RotationDirection { get; set; } = Direction.Left;
        public MotorID ControlledMotor { get; set; } = MotorID.First;
        public RotationUnit Unit { get; set; } = RotationUnit.Step;
        public int Interval { get; set; } = 100;
        public int nextIndex { get; set; } = 0;
        public Direction LastDirectoin { get; set; } = Direction.Left;

        public int nextFullStepIndex = 0;
        public int nextHalfStepIndex = 0;


        public MotorDriver(){ }
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
        public void Dispose()
        {
            Disconnect();
        }


        public void Rotate(int rotationValue)
        {
            if(Connected)
            {
                // Generate sequence that matches previously set parameters
                LastDirectoin = RotationDirection;
                var sequence = ControlSequence.Create(Mode, RotationDirection, ControlledMotor);
                var steps = CalcualteSteps(rotationValue);
                uint bytesWritten = 0;
               // var shift = (LastDirectoin == RotationDirection ? nextIndex : sequence.Count - nextIndex+1);
                for (int i = nextIndex; i < steps; i++)
                {
                    // Find the next signal in sequence
                    //// Prevent going out of bounds with modulo
                    //int index = i % sequence.Count;
                    //nextIndex = (index + 1 )% sequence.Count;
                    //byte[] signal = { sequence.ElementAt(index) };
                    //status = deviceWrapper.Write(signal, 1, ref bytesWritten);
                    Thread.Sleep(Interval);
                }
            }
          
        }
        public int CalcualteSteps(int value)
        {
            int steps = value;
            if(Unit == RotationUnit.Degree)            
                steps = (int)(ControlledMotor == MotorID.First ? steps/0.75 : steps / 7.5);            
            return steps;           

        }
        

   
       

    

        private string ReadEprom()
        {
            string str = "";
            if(Connected)
            {
                status = deviceWrapper.ReadFT232BEEPROM(new FTDI.FT232B_EEPROM_STRUCTURE());
            }
            return str;
        }


        private void StopMotor()
        {
            uint bytesWritten = 0;
            status = deviceWrapper.Write(new byte[]{0x00}, 1, ref bytesWritten);
        }
      
        private bool Connected { get; set; } = false;
        private FTDI deviceWrapper = new FTDI();
        private FTDI.FT_STATUS status;
    }
}
