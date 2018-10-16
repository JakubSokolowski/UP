using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;

namespace StepMotor
{
    enum Direction
    {
        Left,
        Right
    }
    enum MotorID
    {
        First,
        Second
    }
    enum ControlMode
    {
        HalfStep,
        FullStep
    }

    class MotorDriver : IDisposable
    {
        public ControlMode Mode { get; set; } = ControlMode.HalfStep;
        public Direction RotationDirection { get; set; } = Direction.Right;
        public MotorID ControlledMotor { get; set; } = MotorID.First;

        public MotorDriver(){ }
        public string Connect()
        {           
            string message = "Unable to Connect";
            if(!Connected)
            {
                try
                {
                    uint deviceCount = 0;
                    deviceWrapper.GetNumberOfDevices(ref deviceCount);
                    FTDI.FT_DEVICE_INFO_NODE[] deviceInfo = new FTDI.FT_DEVICE_INFO_NODE[deviceCount];
                    status = deviceWrapper.GetDeviceList(deviceInfo);
                    status = deviceWrapper.OpenByDescription(deviceInfo[0].Description);
                    status = deviceWrapper.SetBitMode(0xff, 1);
                    message = status.ToString();
                    Connected = true;
                }
                catch(Exception e)
                {
                    message = "Error while trying to connect to device." +
                        "\nStatus: "+ status.ToString() +
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
            }
        }
        public void Dispose()
        {
            Disconnect();
        }


        public void Rotate(int steps, int interval)
        {
            // Generate sequence that matches previously set parameters
            var sequence = GenerateControlSequence(Mode, RotationDirection, ControlledMotor);
            uint bytesWritten = 0;
            for(int i = 0; i < steps; i++)
            {
                int index = steps % sequence.Count;
                byte[] signal = { sequence.ElementAt(index) };
                status = deviceWrapper.Write(signal, 1, ref bytesWritten);
            }
            StopMotor();
        }

        private List<byte> GenerateControlSequence(ControlMode control, Direction dir, MotorID id)
        {
            return control == ControlMode.FullStep ?
                GenerateFullStepSequence(dir, id) : GenerateHalfStepSequence(dir, id);
        }
        private List<byte> GenerateHalfStepSequence(Direction direction, MotorID id)
        {
            // Default sequence, rotates first motor to the right
            List<byte> sequence = new List<byte>(){ 0x01, 0x08, 0x02, 0x04 }; ;
            // To change rotation direction, reverse the signal order
            if (direction == Direction.Left)
                sequence.Reverse();
            // Changing mothor requires moving the signals from younger to older bits
            if (id == MotorID.Second)
                sequence.ToList().ForEach(s => s = (byte)(s & 0x0F));
            return sequence;  
        }
        private List<byte> GenerateFullStepSequence(Direction direction, MotorID id)
        {
            // Default sequence, rotates first motor to the right in full step mode
            List<byte> sequence = new List<byte>() { 0x07, 0x0F, 0x0E, 0x0F, 0x0B, 0x0F, 0x0D, 0x0F };
            // To change rotation direction, reverse the signal order
            if (direction == Direction.Left)
                sequence.Reverse();
            // Changing mothor requires moving the signals from younger to older bits
            if (id == MotorID.Second)
                sequence.ToList().ForEach(s => s = (byte)(s & 0x0F));
            return sequence;
        }
        
 
        private void StopMotor()
        {
            uint bytesWritten = 0;
            status = deviceWrapper.Write(new byte[]{0x00}, 1, ref bytesWritten);
        }
      
        private bool Connected { get; set; } = false;
        private FTDI deviceWrapper;
        private FTDI.FT_STATUS status;
    }
}
