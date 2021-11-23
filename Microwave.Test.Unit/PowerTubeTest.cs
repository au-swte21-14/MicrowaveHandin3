using Microwave.Classes.Boundary;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NUnit.Framework;

namespace Microwave.Test.Unit
{
    [TestFixture]
    public class PowerTubeTest
    {
        private PowerTube uut600, uut700, uut1000;
        private IOutput output;

        [SetUp]
        public void Setup()
        {
            output = Substitute.For<IOutput>();
            uut600 = new PowerTube(output, 600);
            uut700 = new PowerTube(output, 700);
            uut1000 = new PowerTube(output, 1000);
        }

        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        [TestCase(599)]
        [TestCase(600)]
        public void TurnOn_WasOffCorrectPower_CorrectOutput(int power)
        {
            uut600.TurnOn(power);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"{power}")));
            uut700.TurnOn(power);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"{power}")));
            uut1000.TurnOn(power);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"{power}")));
        }
        
        [TestCase(699)]
        [TestCase(700)]
        public void TurnOn_WasOffCorrectPower_CorrectOutput_700W(int power)
        {
            uut700.TurnOn(power);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"{power}")));
            uut1000.TurnOn(power);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"{power}")));
        }
        
        [TestCase(999)]
        [TestCase(1000)]
        public void TurnOn_WasOffCorrectPower_CorrectOutput_1000W(int power)
        {
            uut1000.TurnOn(power);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"{power}")));
        }

        [TestCase(-5)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(701)]
        [TestCase(750)]
        public void TurnOn_WasOffOutOfRangePower_ThrowsException(int power)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => uut700.TurnOn(power));
        }
        
        [TestCase(601)]
        [TestCase(650)]
        public void TurnOn_WasOffOutOfRangePower_ThrowsException_600W(int power)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => uut600.TurnOn(power));
        }
        
        [TestCase(1001)]
        [TestCase(1050)]
        public void TurnOn_WasOffOutOfRangePower_ThrowsException_1000W(int power)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => uut1000.TurnOn(power));
        }

        [Test]
        public void TurnOff_WasOn_CorrectOutput()
        {
            uut700.TurnOn(50);
            uut700.TurnOff();
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("off")));
        }

        [Test]
        public void TurnOff_WasOff_NoOutput()
        {
            uut700.TurnOff();
            output.DidNotReceive().OutputLine(Arg.Any<string>());
        }

        [Test]
        public void TurnOn_WasOn_ThrowsException()
        {
            uut700.TurnOn(50);
            Assert.Throws<System.ApplicationException>(() => uut700.TurnOn(60));
        }

        [Test]
        public void MaxPower()
        {
            Assert.AreEqual(600, uut600.MaxPower);
            Assert.AreEqual(700, uut700.MaxPower);
            Assert.AreEqual(1000, uut1000.MaxPower);
        }
    }
}