using RoboTx.Api;

namespace c_sharp_projects._1_reading_sensors
{
    internal class Button
    {
        /// <summary>
        /// Demonstrates how to sense button press and release actions of All-in-one kit for Arduino.
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
                Console.WriteLine("Press, release, hold then release the button on the all in one kit.");
                Console.WriteLine("Press Esc to stop the program.");

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    var btn = all_in_one_kit.Digital.GetInputEvent();

                    switch (btn)
                    {
                        case Input.BUTTON_1_PRESSED:
                            Console.WriteLine("Button 1 pressed");
                            break;
                        case Input.BUTTON_1_SUSTAINED:
                            Console.WriteLine("Button 1 held");
                            break;
                        case Input.BUTTON_1_RELEASED:
                            Console.WriteLine("Button 1 released");
                            break;
                        case Input.BUTTON_1_SUSTAIN_RELEASED:
                            Console.WriteLine("Button 1 released from hold");
                            break;
                    }

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(25);
                }
            }
        }
    }
}
