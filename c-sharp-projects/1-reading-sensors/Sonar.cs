using RoboTx.Api;

namespace c_sharp_projects._1_reading_sensors
{
    internal class Sonar
    {
        /// <summary>
        /// Demonstrates how to use to use the sonar sensor for measuring distance
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
                Console.WriteLine("Press Esc to stop the program.");

                var sonar = all_in_one_kit.Sonar;

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    sonar.Ping();

                    if (sonar.DistanceAcquired)
                    {
                        // Get distance reported in centimeters.
                        int distanceCm = sonar.GetDistance();
                        Console.WriteLine($"{distanceCm} cm");
                    }

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(50);
                }
            }
        }
    }
}
