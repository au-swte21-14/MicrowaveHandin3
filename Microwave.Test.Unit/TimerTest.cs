using System.Threading;
using NUnit.Framework;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave.Test.Unit
{
    [TestFixture]
    public class TimerTest
    {
        private Timer uut;

        [SetUp]
        public void Setup()
        {
            uut = new Timer();
        }

        [Test]
        public void Start_TimerTick_ShortEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.TimerTick += (sender, args) => pause.Set();
            uut.Start(2);

            // wait for a tick, but no longer
            Assert.That(pause.WaitOne(1100));
        }

        [Test]
        public void Start_TimerTick_LongEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.TimerTick += (sender, args) => pause.Set();
            uut.Start(2);

            // wait shorter than a tick, shouldn't come
            Assert.That(!pause.WaitOne(900));
        }

        [Test]
        public void Start_TimerExpires_ShortEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();
            uut.Start(2);

            // wait for expiration, but not much longer, should come
            Assert.That(pause.WaitOne(2100));
        }

        [Test]
        public void Start_TimerExpires_LongEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();
            uut.Start(2);

            // wait shorter than expiration, shouldn't come
            Assert.That(!pause.WaitOne(1900));
        }

        [Test]
        public void Start_TimerTick_CorrectNumber()
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            int notifications = 0;

            uut.Expired += (sender, args) => pause.Set();
            uut.TimerTick += (sender, args) => notifications++;

            uut.Start(2);

            // wait longer than expiration
            Assert.That(pause.WaitOne(2100));

            Assert.That(notifications, Is.EqualTo(2));
        }

        [Test]
        public void Stop_NotStarted_NoThrow()
        {
            Assert.That( () => uut.Stop(), Throws.Nothing);
        }

        [Test]
        public void Stop_Started_NoTickTriggered()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.TimerTick += (sender, args) => pause.Set();

            uut.Start(2000);
            uut.Stop();

            Assert.That(!pause.WaitOne(1100));
        }

        [Test]
        public void Stop_Started_NoExpiredTriggered()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();

            uut.Start(2000);
            uut.Stop();

            Assert.That(!pause.WaitOne(2100));
        }

        [Test]
        public void Stop_StartedOneTick_NoExpiredTriggered()
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            int notifications = 0;

            uut.Expired += (sender, args) => pause.Set();
            uut.TimerTick += (sender, args) => uut.Stop();

            uut.Start(2000);

            Assert.That(!pause.WaitOne(2100));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Tick_Started_TimeRemainingCorrect(int ticks)
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            int ticksGone = 0;
            uut.TimerTick += (sender, args) =>
            {
                ticksGone++;
                if (ticksGone >= ticks)
                    pause.Set();
            };
            uut.Start(5);

            // wait for ticks, only a little longer
            pause.WaitOne(ticks * 1000 + 100);

            Assert.That(uut.TimeRemaining, Is.EqualTo(5-ticks*1));
        }
        
        [Test]
        public void Start_TimerExpires_ShortEnough_IncrementWhileRunning()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();
            uut.Start(2);
            
            // Wait some time before incrementing
            Assert.That(!pause.WaitOne(1000));
            uut.Set(uut.TimeRemaining + 1);

            // Wait for original wait time
            Assert.That(!pause.WaitOne(1100));
            // wait for expiration, but not much longer, should come
            Assert.That(pause.WaitOne(1100));
        }
        
        [Test]
        public void Start_TimerExpires_ShortEnough_DecrementWhileRunning()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();
            uut.Start(3);
            
            // Wait some time before decrementing
            Assert.That(!pause.WaitOne(1000));
            uut.Set(uut.TimeRemaining - 1);
            // Validate that it hasn't timed out yet
            Assert.That(!pause.WaitOne(100));

            // wait for expiration, but not much longer, should come
            Assert.That(pause.WaitOne(1000));
        }
        
        [Test]
        public void Start_TimerExpires_ShortEnough_DecrementToExpirationWhileRunning()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();
            uut.Start(2);
            
            // Wait some time before incrementing
            Assert.That(!pause.WaitOne(1000));
            uut.Set(uut.TimeRemaining - 2);
            // Validate that it has timed out
            Assert.That(pause.WaitOne(0));
        }
    }
}