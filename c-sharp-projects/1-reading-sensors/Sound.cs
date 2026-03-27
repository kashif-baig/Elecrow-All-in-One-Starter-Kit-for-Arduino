using RoboTx.Api;

namespace c_sharp_projects._1_reading_sensors
{
    internal class Sound
    {
        /// <summary>
        /// Demonstrates how to read values from the sound sensor of All-in-one kit for Arduino.
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
                Console.WriteLine("Make a clapping or clicking sound near the sound sensor.");
                Console.WriteLine("Press Esc to stop the program.");

                var sound_sensor = all_in_one_kit.Analog.A1;

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    if (sound_sensor.Value > 30)
                    {
                        Console.WriteLine($"Sound sensor value: {sound_sensor.Value}");
                    }

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(25);
                }
            }
        }
    }
}
