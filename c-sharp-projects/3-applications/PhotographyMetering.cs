using RoboTx.Api;

namespace c_sharp_projects._3_applications
{
    internal class PhotographyMetering
    {
        /// <summary>
        /// Application for calculating the aperture F stop of a camera.
        /// Use the slider to select the shutter speed, and press the button to cycle
        /// through ISO values. Position the all in one kit where the photography
        /// subject is locationed, and allow the ambient light to fall on the light sensor.
        /// 
        /// Coding challenge:
        /// 
        /// Part 1: Modify the source code to use the IR remote to freeze/unfreeze the F stop
        /// reading on the LCD.
        /// Part 2: Modify the source code to support aperture priority. I.e., allow the user
        /// to select the F stop, and calculate the shutter speed accordingly.
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
                // F stop calibration constant.
                const int calibration_const = 330;

                all_in_one_kit.Connect();
                Console.WriteLine("Press Esc to stop the program.");

                var light_meter = all_in_one_kit.LightMeter;
                light_meter.Enable();

                all_in_one_kit.WaitUntilSensorsReady(light_meter);

                var display = all_in_one_kit.Display;
                var shutter = all_in_one_kit.Analog.A0;
                all_in_one_kit.Analog.UseConverter(MapToShutterSpeed, shutter);

                var iso = 100;
                display.PrintAt(6, 0, $"{iso}");

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    // Get the current input event (button press)
                    var input_event = all_in_one_kit.Digital.GetInputEvent();

                    // Handle button‑1 release: double ISO, wrap around at 800, and show it
                    if (input_event == Input.BUTTON_1_RELEASED)
                    {
                        iso = iso * 2;
                        if (iso > 800)
                        {
                            iso = 100;
                        }
                        display.PrintAt(6, 0, $"{iso}");
                    }
                    // Compute the f‑stop value
                    var f_stop = Math.Sqrt((light_meter.LuxValue * iso * (1 / shutter.Value)) / calibration_const);

                    // Display the f‑stop (or an error) left‑justified to a width of 5 characters
                    if (f_stop >= 1.8 && f_stop <= 22)
                    {
                        display.PrintAt(0, 0, $"F{f_stop:0.0}".PadRight(5));
                    }
                    else
                    {
                        display.PrintAt(0, 0, "Error".PadRight(5));
                    }

                    // Display the shutter speed, right‑justified to a width of 6 characters
                    display.PrintAt(10, 0, $"1/{shutter.Value.ToString()}".PadLeft(6));

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Shutter speeds expressed in inverse form.
        /// </summary>
        static int[] shutter_speeds =
            new[] {3,4,5,6,10,13,15,20,25,30,40,50,60,80,
            100,125,160,200,250,320,400,500,640,800,1400};

        /// <summary>
        /// Maps raw analog value (0 to 1023) to shutter speeds.
        /// </summary>
        /// <param name="analogValue"></param>
        /// <returns></returns>
        static float MapToShutterSpeed(float analogValue)
        {
            return shutter_speeds[(int)(((shutter_speeds.Length - 1) * analogValue) / 1023)];
        }
    }
}
