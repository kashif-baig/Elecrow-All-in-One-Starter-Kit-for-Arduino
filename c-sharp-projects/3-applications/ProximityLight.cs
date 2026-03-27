using RoboTx.Api;

namespace c_sharp_projects._3_applications
{
    /// <summary>
    /// A proximity security light that switches on when motion is detected by the PIR
    /// sensor, and when the ambient light level is below the threshold set by the slider
    /// (linear potentiometer).
    /// 
    /// Coding challenge:
    /// Part 1: Show both the current light meter LUX value and the LUX threshold value on the LCD.
    /// Part 2: Add a further condition where an object must be within 50cm of the sonar sensor
    ///         before LED and relay are switched on.
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
    internal class ProximityLight
    {
        static void Main(string[] args)
        {
            using (RobotIO all_in_one_kit = new RobotIO(AppConfig.SerialPortName))
            {
                all_in_one_kit.Connect();
                Console.WriteLine("Press Esc to stop the program.");

                var relay = all_in_one_kit.Switch1;
                var led = all_in_one_kit.Switch2;
                var motion_detected = all_in_one_kit.Digital.IN1;
                var lux_threshold = all_in_one_kit.Analog.A0;

                // Register function to convert raw slider value to LUX threshold value.
                all_in_one_kit.Analog.UseConverter(ConvertToLuxThreshold, lux_threshold);

                var light_meter = all_in_one_kit.LightMeter;
                light_meter.Enable();

                all_in_one_kit.WaitUntilSensorsReady(light_meter);

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    // Has motion been detected and ambient lighting lower than threshold?
                    if (motion_detected && light_meter.LuxValue <= lux_threshold.Value)
                    {
                        // Switch on LED and relay
                        if (!led.IsOn)
                        {
                            led.On();
                            relay.On();
                        }
                    }
                    else
                    {
                        // Switch off LED and relay
                        if (led.IsOn)
                        {
                            led.Off();
                            relay.Off();
                        }
                    }
                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        /// Converts raw analog value of slider to a value range that is suitable for
        /// setting the LUX threshold.
        /// </summary>
        /// <param name="analogValue"></param>
        /// <returns></returns>
        static float ConvertToLuxThreshold(float analogValue)
        {
            return (200 * analogValue) / 1023;
        }
    }
}
