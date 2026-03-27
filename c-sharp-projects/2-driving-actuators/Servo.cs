using RoboTx.Api;

namespace c_sharp_projects._2_driving_actuators
{
    internal class Servo
    {
        /// <summary>
        /// Demonstrates the setting of the servo motor position of All-in-one kit for Arduino
        /// using the slider constrol. If insufficient power is supplied
        /// to the All in One Kit, it may reset and temporarily disconnect
        /// from the PC, causing the program to end.
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

                var servo = all_in_one_kit.Servo1;
                var slider = all_in_one_kit.Analog.A0;

                // Register converter function to convert slider analog value to angle. 
                all_in_one_kit.Analog.UseConverter(ConvertToAngle, slider);

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    Console.WriteLine($"Converted slider value: {slider.Value}");
                    servo.SetPosition(slider.Value);

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(20);
                }
            }
        }

        /// <summary>
        /// Converts the input value (0 to 1023) to an angle value (0 to 180)
        /// for setting the position of the servo.
        /// </summary>
        /// <param name="analogValue"></param>
        /// <returns></returns>
        static float ConvertToAngle(float analogValue)
        {
            return (180 * analogValue) / 1023;
        }
    }
}

