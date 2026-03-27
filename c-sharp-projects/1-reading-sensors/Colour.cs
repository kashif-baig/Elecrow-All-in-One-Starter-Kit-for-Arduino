using RoboTx.Api;

namespace c_sharp_projects._1_reading_sensors
{
    /// <summary>
    /// Uses separately available Grove I2C colour module based on TCS3472 sensor.
    /// Use cable to plug in to one of the I2C sockets of the All-in-one kit for Arduino.
    /// Ensure the illumination LED is switched on the sensor module and place
    /// the sensor close to the surface of the colour being detected.
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
    internal class Colour
    {
        // Define reference HSL values for Rubik's cube colours.
        // Adapt as necessary for your sensor/lighting environment.
        static (string name, float h_min, float h_max, float s_min, float s_max, float l_min, float l_max)[]
            rubiks_colours =
            [   ("white",   147,153, 17,20, 32,33),
                ("red",     2,17,    10,28, 31,37),
                ("orange",  29,59,   15,43, 29,33),
                ("yellow",  78,98,   23,40, 30,33),
                ("green",   121,154, 24,45, 32,36),
                ("blue",    188,204, 36,56, 29,32),
            ];

        static void Main(string[] args)
        {
            using (RobotIO all_in_one_kit = new RobotIO(AppConfig.SerialPortName))
            {
                all_in_one_kit.Connect();
                Console.WriteLine("Press Esc to stop the program.");

                var colour_sensor = all_in_one_kit.ColourSensor;
                colour_sensor.Enable();
                all_in_one_kit.WaitUntilSensorsReady(colour_sensor);

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    // Get raw HSL values of detected colour.
                    var col = colour_sensor.GetHSL();

                    foreach (var rubiks_col in rubiks_colours)
                    {
                        // For a detected colour to match a reference colour, each of the colour's
                        // hue, saturation and lightness values must be within the range of the
                        // corresponding component of the reference colour.
                        if (col.Hue >= rubiks_col.h_min && col.Hue <= rubiks_col.h_max &&
                            col.Saturation >= rubiks_col.s_min && col.Saturation <= rubiks_col.s_max &&
                            col.Lightness >= rubiks_col.l_min && col.Lightness <= rubiks_col.l_max)
                        {
                            Console.WriteLine(rubiks_col.name);
                        }
                    }

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(100);
                }
            }
        }
    }
}
