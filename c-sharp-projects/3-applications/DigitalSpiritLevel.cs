using RoboTx.Api;

namespace c_sharp_projects._3_applications
{
    internal class DigitalSpiritLevel
    {
        /// <summary>
        /// A digital spirit level that uses the accelerometer readings of MPU6050 sensor
        /// to measure the incline angle of a surface.
        /// 
        /// Coding challenge:
        /// Part 1:
        /// Modify the source code to write the value of variable angle_offset to a file after
        /// calibration is complete. Overwrite the file if it already exists. After the program
        /// starts, load the value from the file (if it exists) back in to the variable angle_offset.
        /// 
        /// Part 2:
        /// Stabilize the angle readings by retaining the five most recent angles in an array and
        /// calculating and reporting their average. Which provides a more stable result, mean average
        /// or median average?
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
            // Constant for converting radians to degrees.
            const float rad_to_deg = 180 / 3.14159f;

            using (RobotIO all_in_one_kit = new RobotIO(AppConfig.SerialPortName))
            {
                all_in_one_kit.Connect();
                Console.WriteLine("Press and hold the button to calibrate the sensor on a level surface.");
                Console.WriteLine("Press Esc to stop the program.");

                var display = all_in_one_kit.Display;
                var mpu_sensor = all_in_one_kit.MPUSensor;

                mpu_sensor.Enable();
                all_in_one_kit.WaitUntilSensorsReady(mpu_sensor);

                float angle_offset = 0;
                bool calibration_mode = false;

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    // Read the latest digital input event (in this case the button)
                    var input_event = all_in_one_kit.Digital.GetInputEvent();

                    // Handle button‑1 sustained press → start calibration
                    if (input_event == Input.BUTTON_1_SUSTAINED && !calibration_mode)
                    {
                        Console.WriteLine("Calibrating");
                        calibration_mode = true;

                        angle_offset = 0;
                        // Simple sensor calibration
                        // Calculate average of 10 readings on flat surface to use as offset.
                        for (int i = 0; i < 10; i++)
                        {
                            // Math.Atan returns radians; convert to degrees
                            angle_offset += (float)Math.Atan(mpu_sensor.Accel.X / mpu_sensor.Accel.Z) * rad_to_deg;
                            Thread.Sleep(100);  // 100 ms pause between samples
                        }
                        angle_offset = angle_offset / 10;
                        // Flush the input event buffer.
                        all_in_one_kit.Digital.ClearInputEvents();
                    }
                    else if (input_event == Input.BUTTON_1_SUSTAIN_RELEASED)
                    {
                        // Handle button‑1 release → exit calibration mode
                        calibration_mode = false;
                    }
                    // Compute the current slope angle (degrees)
                    // subtract the calibrated offset from the sensor reading
                    float angle = -angle_offset +
                                    (float)Math.Atan(mpu_sensor.Accel.X / mpu_sensor.Accel.Z) * rad_to_deg;
                    // Display the angle, right‑aligned in a 6‑character field
                    display.PrintAt(4, 0, $"{angle:0.0}".PadLeft(6));

                    // Build a simple level indicator bar (max length = 16 chars)
                    string level_indicator;
                    if (angle >= 0)
                    {
                        // Positive tilt: dashes on the left side
                        level_indicator = (new string('-', (int)(angle > 8 ? 8 : angle))).PadLeft(8) + new string(' ', 8);
                    }
                    else
                    {
                        // Negative tilt: dashes on the right side
                        level_indicator = new string(' ', 8) + (new string('-', (int)(angle < -8 ? 8 : -angle))).PadRight(8);
                    }
                    // Show the level indicator bar on the display
                    display.PrintAt(0, 1, level_indicator);

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(200);
                }
            }
        }
    }
}
