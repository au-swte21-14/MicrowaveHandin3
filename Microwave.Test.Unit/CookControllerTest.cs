using System;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace Microwave.Test.Unit
{
    [TestFixture]
    public class CookControllerTest
    {
        private CookController uut;

        private IUserInterface ui;
        private ITimer timer;
        private IDisplay display;
        private IPowerTube powerTube;

        [SetUp]
        public void Setup()
        {
            ui = Substitute.For<IUserInterface>();
            timer = Substitute.For<ITimer>();
            display = Substitute.For<IDisplay>();
            powerTube = Substitute.For<IPowerTube>();

            uut = new CookController(timer, display, powerTube, ui);
        }

        [Test]
        public void StartCooking_ValidParameters_TimerStarted()
        {
            uut.StartCooking(50, 60);

            timer.Received().Start(60);
        }

        [Test]
        public void StartCooking_ValidParameters_PowerTubeStarted()
        {
            uut.StartCooking(50, 60);

            powerTube.Received().TurnOn(50);
        }

        [Test]
        public void Cooking_TimerTick_DisplayCalled()
        {
            uut.StartCooking(50, 60);

            timer.TimeRemaining.Returns(115);
            timer.TimerTick += Raise.EventWith(this, EventArgs.Empty);

            display.Received().ShowTime(1, 55);
        }

        [Test]
        public void Cooking_TimerExpired_PowerTubeOff()
        {
            uut.StartCooking(50, 60);

            timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            powerTube.Received().TurnOff();
        }

        [Test]
        public void Cooking_TimerExpired_UICalled()
        {
            uut.StartCooking(50, 60);

            timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            ui.Received().CookingIsDone();
        }

        [Test]
        public void Cooking_Stop_PowerTubeOff()
        {
            uut.StartCooking(50, 60);
            uut.Stop();

            powerTube.Received().TurnOff();
        }

        [Test]
        public void Cooking_ChangeTime()
        {
            uut.StartCooking(50, 60);
            uut.ChangeTime(10);
            
            // Verify that timer has been updated
            timer.Received().Set(10);
        }
        
        [Test]
        public void Cooking_OffsetTime()
        {
            uut.StartCooking(50, 60);
            timer.TimeRemaining.Returns(60);
            uut.OffsetTime(10);
            
            // Verify that timer has been updated
            timer.Received().Set(70);
        }
        [Test]
        public void Cooking_PowerTubeMaxPower()
        {
            powerTube.Configure().MaxPower.Returns(600);
            Assert.AreEqual(600, uut.MaxPower);
        }
    }
}