using RoboTx.Api;

namespace c_sharp_projects._2_driving_actuators
{
    internal class Relay
    {
        /// <summary>
        /// Demonstrate switching relay on and off using the slider of All-in-one kit for Arduino.
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
                Console.WriteLine("Use the slider to switch relay on/off.");
                Console.WriteLine("Press Esc to stop the program.");

                var relay = all_in_one_kit.Switch1;
                var slider = all_in_one_kit.Analog.A0;

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    if (slider.Value > 511)
                    {
                        relay.On();
                    }
                    else
                    {
                        relay.Off();
                    }
                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(100);
                }
            }
        }
    }
}
