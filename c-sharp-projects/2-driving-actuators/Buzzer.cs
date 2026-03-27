using RoboTx.Api;

namespace c_sharp_projects._2_driving_actuators
{
    internal class Buzzer
    {
        /// <summary>
        /// Demonstrates how to sound the buzzer using different repeating patterns
        /// using All-in-one kit for Arduino.
        ///
        /// Robo-Tx firmware must be deployed to the Arduino. Before doing so, make sure
        /// SELECTED_PROFILE is set to PROFILE_ALL_IN_ONE_KIT_ARDU in file Settings.h
        ///
        /// https://github.com/kashif-baig/RoboTx_Firmware
        ///
        /// Robo-Tx API online help: https://help.cohesivecomputing.co.uk/Robo-Tx
        ///
        /// Check settings in file AppConfig.cs before running the code.
        /// All examples are provided as is and at user's own risk.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            using (RobotIO all_in_one_kit = new RobotIO(AppConfig.SerialPortName))
            {
                all_in_one_kit.Connect();

                // Buzzer uses trigger logic.
                var buzzer = all_in_one_kit.Trigger;

                Console.WriteLine("Smoke alarm pattern.");
                buzzer.RunPattern(
                    500,    // on for 500ms.
                    500,    // off for 500ms.
                    3,      // repeat on/off cycle 3 times.
                    3,      // repeat above pattern 3 times
                    1000    // with interval of 1000ms.
                    );
                Thread.Sleep(8000);

                Console.WriteLine("Alarm clock pattern.");
                buzzer.RunPattern(50, 50, 4, 3, 500);
                Thread.Sleep(3000);

                Console.WriteLine("Reversing vehicle pattern.");
                buzzer.RunPattern(500, 500, 10);

                Console.WriteLine("Press Esc to stop the program.");

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(50);
                }
            }
        }
    }
}
