using RoboTx.Api;

namespace c_sharp_projects._1_reading_sensors
{
    internal class Slider
    {
        /// <summary>
        /// Demonstrates how to read the linear potentiometer position of All-in-one kit for Arduino.
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

                var slider = all_in_one_kit.Analog.A0;
                
                // Optional step to register a function to convert raw slider values (0 to 1023)
                // to percent range (0 to 100).
                all_in_one_kit.Analog.UseConverter(ConvertToPercent, slider);

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    Console.WriteLine($"Slider value: {slider.Value:0.0}");
                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        /// Function to convert raw analog value (0 to 1023) to percent range (0 to 100).
        /// </summary>
        static float ConvertToPercent(float analogValue)
        {
            return (analogValue * 100)/1023;
        }
    }
}
