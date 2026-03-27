using RoboTx.Api;

namespace c_sharp_projects._3_applications
{
    internal class SoilMoistureMeter
    {
        /// <summary>
        /// Application to measure the moisture content of the soil in a plant pot.
        /// Connect the soil moisture sensor to the socket marked A6 on the
        /// all in one kit using the supplied cable. Push the sensor prongs in to
        /// the soil to obtain the moisture reading.
        /// 
        /// Coding challenge:
        /// Below the TODO comment, modify the source code such that when the button
        /// is pressed, the message "Water required" is displayed on the second row
        /// of the LCD if the moisture content is less than 20%. Futhermore, the beeper
        /// must be sounded if the moisture content is less than 10%.
        /// Use beeper.RunPattern(100, 100, 3) to sound the beeper.
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
                var soil_moisture = all_in_one_kit.Analog.A6;

                // Register function to convert raw moisture sensor value to percentage.
                all_in_one_kit.Analog.UseConverter(ConvertToPercent, soil_moisture);

                var display = all_in_one_kit.Display;

                display.PrintAt(0, 0, "Soil moist:");

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    // Get the current input event (i.e. button events)
                    var input_event = all_in_one_kit.Digital.GetInputEvent();

                    if (input_event == Input.BUTTON_1_PRESSED)
                    {
                        // TODO implement button‑1 pressed handling
                    }
                    // Show the soil moisture percentage, padded to a width of 4 characters
                    display.PrintAt(11, 0, $"{(int)soil_moisture.Value}%".PadLeft(4));

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Function to convert raw analog value (0 to 1023) to a percentage value.
        /// </summary>
        /// <param name="analogValue"></param>
        /// <returns></returns>
        static float ConvertToPercent(float analogValue)
        {
            return (100 * analogValue) / 1023;
        }
    }
}
