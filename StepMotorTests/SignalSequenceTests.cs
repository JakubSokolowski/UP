using System;
using StepMotor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace StepMotorTests
{
    [TestClass]
    public class SignalSequenceTests
    {
        [TestMethod]
        public void Create_GeneratesValidHalfStepRightSequence()
        {
            var sequence = ControlSequence.Create(ControlMode.HalfStep, Direction.Right, MotorID.First);
            var expected = new List<byte>() { 0x05, 0x01, 0x09, 0x08, 0x0A, 0x02, 0x06, 0x04 };
            CollectionAssert.AreEqual(expected, sequence.signalSequence);
        }
        [TestMethod]
        public void Create_GeneratesValidHalfStepLeftSequence()
        {
            var sequence = ControlSequence.Create(ControlMode.HalfStep, Direction.Left, MotorID.First);
            var expected = new List<byte>() { 0x05, 0x01, 0x09, 0x08, 0x0A, 0x02, 0x06, 0x04 };
            expected.Reverse();
            CollectionAssert.AreEqual(expected, sequence.signalSequence);
        }
        [TestMethod]
        public void Create_GeneratesValidFullStepRightSequence()
        {
            var sequence = ControlSequence.Create(ControlMode.FullStep, Direction.Right, MotorID.First);
            var expected = new List<byte>() { 0x01, 0x08, 0x02, 0x04 };
            CollectionAssert.AreEqual(expected, sequence.signalSequence);
        }
        [TestMethod]
        public void Create_GeneratesValidFullStepLeftSequence()
        {
            var sequence = ControlSequence.Create(ControlMode.FullStep, Direction.Left, MotorID.First);
            var expected = new List<byte>() { 0x01, 0x08, 0x02, 0x04 };
            expected.Reverse();
            CollectionAssert.AreEqual(expected, sequence.signalSequence);
        }

        [TestMethod]
        public void NextSignal_ReturnsValidSignalAfterRotation()
        {
            var sequence = ControlSequence.Create(ControlMode.HalfStep, Direction.Right, MotorID.First);
            byte expected = 0x01;
            byte actual = sequence.NextSignal(Direction.Right);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void NextSignal_ReturnsValidSignalAfterMultipleRotations()
        {
            var sequence = ControlSequence.Create(ControlMode.HalfStep, Direction.Right, MotorID.First);
            byte expected = 0x09;
            byte actual = 0x05;
            for(int i = 0; i < 10; i++)
                 actual = sequence.NextSignal(Direction.Right);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void NextSignal_ReturnsValidSignalAfterChangingRotationDirection()
        {
            var sequence = ControlSequence.Create(ControlMode.HalfStep, Direction.Right, MotorID.First);
            byte expected = 0x01;
            byte actual = sequence.NextSignal(Direction.Right);
            Assert.AreEqual(expected, actual);
            expected = 0x04;
            actual = sequence.NextSignal(Direction.Left);
            actual = sequence.NextSignal(Direction.Left);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void NextSignal_ReturnsValidSignalAfterChangingRotationDirectionAndMultipleRotations()
        {
            var sequence = ControlSequence.Create(ControlMode.HalfStep, Direction.Right, MotorID.First);
            byte expected = 0x02;
            byte actual = 0x01;
            for (int i = 0; i < 5; i++)
                actual = sequence.NextSignal(Direction.Right);
            Assert.AreEqual(expected, actual);

            expected = 0x05;
            for (int i = 0; i < 5; i++)
                actual = sequence.NextSignal(Direction.Left);
            Assert.AreEqual(expected, actual);
        }


    }
}
