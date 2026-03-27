using RoboTx.Api;

namespace c_sharp_projects._2_driving_actuators
{
    internal class LED
    {
        /// <summary>
        /// Demonstrates the blinking of the LED of All-in-one kit for Arduino.
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
                Console.WriteLine("Hold Esc to stop the program.");

                // This LED example uses the motor controlling logic.
                var led = all_in_one_kit.Motor1;
                // Cause LED to gradually change brightness.
                led.SetAcceleration(.5f);

                // Alternatively, the LED can be driven as a simple On/Off switch.
                // var led_alt = all_in_one_kit.Switch2;

                var ledOn = false;
                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    if (ledOn)
                    {
                        // Drive LED at 50% brightness
                        led.DriveNoAccel(50);
                        ledOn = false;
                    }
                    else
                    {
                        // Drive LED at 100% brightess.
                        led.Drive(100);
                        ledOn = true;
                    }

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
