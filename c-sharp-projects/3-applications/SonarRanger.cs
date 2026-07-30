using RoboTx.Api;

namespace c_sharp_projects._3_applications
{
    internal class SonarRanger
    {
        /// <summary>
        /// Sonar ranging application that uses sonar to measure distance of object from sensor.
        /// Sound is emmited from the buzzer with an interval that corresponds to the distance.
        /// 
        /// Coding challenge:
        /// Modify the source code to generate a string of length 16 characters consisting of
        /// hyphens (-), where the number of hyphens is proportional to the distance measured.
        /// Pad the remainder of the string with spaces. Display the string on the second row
        /// of the LCD.
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
                Console.WriteLine("Press Esc to stop the program.");

                var beeper = all_in_one_kit.Trigger;
                // Start beeper repeating indefinitely: 50ms beep, 950ms interval.
                beeper.Repeat(50, 950);

                var sonar = all_in_one_kit.Sonar;

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    if (!sonar.DistanceAcquired)
                    {
                        // Emit ping from sonar module.
                        sonar.Ping();
                    }
                    else // Proceed only if a distance measurement is available
                    {
                        // Get distance reported in centimeters.
                        var distance_cm = sonar.GetDistance();

                        // Calculate beep interval (in milliseconds) to correspond to distance.
                        var beep_interval = distance_cm * 10;

                        // If measured distance is less than 3 cm, set beep interval to 0 to emit continuous sound.
                        if (distance_cm < 3)
                        {
                            beep_interval = 0;
                        }
                        // Set beeper interval.
                        beeper.SetOffPeriod(beep_interval);

                        // Show the distance on the display, padded to three digits.
                        all_in_one_kit.Display.PrintAt(10, 0, $"{distance_cm:000} cm");
                    }

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(100);
                }
            }
        }
    }
}
