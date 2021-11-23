using Microwave.Classes.Boundary;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Unit
{
    [TestFixture]
    public class BuzzerTest
    {
        private Buzzer uut;
        private IOutput output;

        [SetUp]
        public void Setup()
        {
            output = Substitute.For<IOutput>();

            uut = new Buzzer(output);
        }

        [Test]
        public void TurnOn_WasOn()
        {
            uut.TurnOnBuzzer();
            uut.TurnOnBuzzer();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("1")));
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("2")));
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("3")));
        }

        [Test]
        public void TurnOn_WasOff()
        {
            uut.TurnOffBuzzer();
            uut.TurnOnBuzzer();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("1")));
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("2")));
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("3")));
        }

        [Test]
        public void TurnOff_WasOff()
        {
            uut.TurnOffBuzzer();
            output.DidNotReceive().OutputLine(Arg.Is<string>(str => str.Contains("off")));
        }

        [Test]
        public void TurnOff_WasOn()
        {
            uut.TurnOnBuzzer();
            uut.TurnOffBuzzer();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("off")));
        }
    }
}