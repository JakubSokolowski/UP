using System.Collections.Generic;
using System.Linq;

namespace StepMotor
{
    public class ControlSequence
    {
        public static ControlSequence Create(ControlMode control, Direction dir, MotorID id)
        {
            switch (control)
            {
                case ControlMode.HalfStep:
                    return new ControlSequence(GenerateHalfStepSequence(dir, id), dir);
                case ControlMode.FullStep:
                    return new ControlSequence(GenerateFullStepSequence(dir, id), dir);
                case ControlMode.WaveStep:
                    return new ControlSequence(GenerateWaveStep(dir, id), dir);

            }
            return new ControlSequence(GenerateHalfStepSequence(dir, id), dir);
        }

        private ControlSequence(List<byte> sequence, Direction direction)
        {
            signalSequence = sequence;
            prevDirection = direction;
        }
        private static List<byte> GenerateHalfStepSequence(Direction direction, MotorID id)
        {
            // Default sequence, rotates first motor to the right
            List<byte> sequence = new List<byte>() { 0x05, 0x01, 0x09, 0x08, 0x0A, 0x02, 0x06, 0x04 };
            // To change rotation direction, reverse the signal order
            if (direction == Direction.Left)
                sequence.Reverse();
            // Changing mothor requires moving the signals from younger to older bits
            if (id == MotorID.Second)
                sequence.ToList().ForEach(s => s = (byte)(s & 0x0F));
            return sequence;
        }
        private static List<byte> GenerateFullStepSequence(Direction direction, MotorID id)
        {
            // Default sequence, rotates first motor to the right in full step mode
            // List<byte> sequence = new List<byte>() { 0x07, 0x0F, 0x0E, 0x0F, 0x0B, 0x0F, 0x0D, 0x0F };
            List<byte> sequence = new List<byte>() { 0x01, 0x08, 0x02, 0x04 };
            // To change rotation direction, reverse the signal order
            if (direction == Direction.Left)
                sequence.Reverse();
            // Changing mothor requires moving the signals from younger to older bits
            if (id == MotorID.Second)
                sequence.ToList().ForEach(s => s = (byte)(s & 0x0F));
            return sequence;
        }
        private static List<byte> GenerateWaveStep(Direction direction, MotorID id)
        {
            // Default sequence, rotates first motor to the right in full step mode
            // List<byte> sequence = new List<byte>() { 0x07, 0x0F, 0x0E, 0x0F, 0x0B, 0x0F, 0x0D, 0x0F };
            List<byte> sequence = new List<byte>() { 0x05, 0x09, 0x0A, 0x06 };
            // To change rotation direction, reverse the signal order
            if (direction == Direction.Left)
                sequence.Reverse();
            // Changing mothor requires moving the signals from younger to older bits
            if (id == MotorID.Second)
                sequence.ToList().ForEach(s => s = (byte)(s & 0x0F));
            return sequence;
        }

    
        public byte NextSignal(Direction direction)
        {
            if(direction != prevDirection)
            {
                prevDirection = direction;
                shift *= -1;
            }
            index += shift;
            if (index >= signalSequence.Count)
                index %= signalSequence.Count;
            if (index < 0)
                index = signalSequence.Count - 1;
            return signalSequence[index];
        }

        public List<byte> signalSequence;
        private int index = 0;
        private int shift = 1;
        private Direction prevDirection = Direction.Right;
    }
}
