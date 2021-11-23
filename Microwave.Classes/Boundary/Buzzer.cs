using System.Threading;
using Microwave.Classes.Interfaces;

namespace Microwave.Classes.Boundary
{
    public class Buzzer : IBuzzer
    {
        private IOutput myOutput;
        private bool isOn = false;
        
        public Buzzer(IOutput output)
        {
            myOutput = output;
        }

        public void TurnOnBuzzer()
        {
            if (!isOn)
            {
                myOutput.OutputLine("BEEP 1");
                Thread.Sleep(1000);
                myOutput.OutputLine("BEEP 2");
                Thread.Sleep(1000);
                myOutput.OutputLine("BEEP 3");
                Thread.Sleep(1000);
                isOn = true;
            }
        }

        public void TurnOffBuzzer()
        {
            if (isOn)
            {
                myOutput.OutputLine("Buzzer is turned off");
                isOn = false;
            }
        }

    }
}