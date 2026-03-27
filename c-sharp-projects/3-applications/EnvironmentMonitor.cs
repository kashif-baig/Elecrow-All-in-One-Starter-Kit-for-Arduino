using RoboTx.Api;

namespace c_sharp_projects._3_applications
{
    internal class EnvironmentMonitor
    {
        /// <summary>
        /// Environment monitoring application that reports temperatue, relative
        /// humidity, ambient lighting and noise level.
        /// 
        /// Coding challenge:
        /// Part 1: use an array for the light level bands, and a loop to find
        /// the appropriate level from the actual LUX value. Try a variation
        /// of the binary search to find the light level.
        /// 
        /// Part 2: at one second intervals, append the sensor values as a line
        /// of comma separated values to a CSV file (create if not exists).
        /// The first value of every line must be the date and time in
        /// yyyy-MM-dd HH:mm:ss format.
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

                var sound_sensor = all_in_one_kit.Analog.A1;
                var light_meter = all_in_one_kit.LightMeter;
                var dht_sensor = all_in_one_kit.DHTSensor;

                light_meter.Enable();
                dht_sensor.Enable();

                all_in_one_kit.WaitUntilSensorsReady(light_meter, dht_sensor);

                var display = all_in_one_kit.Display;

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    // Light level determination
                    var lux_value = light_meter.LuxValue;
                    var light_level = string.Empty;

                    if (lux_value < 30)
                    {
                        light_level = "dark";
                    }
                    else if (lux_value < 75)
                    {
                        light_level = "dim";
                    }
                    else if (lux_value < 150)
                    {
                        light_level = "soft";
                    }
                    else if (lux_value < 400)
                    {
                        light_level = "OK";
                    }
                    else
                    {
                        light_level = "bright";
                    }

                    // Humidity level determination
                    var humidity = dht_sensor.Humidity;
                    var humidity_level = string.Empty;

                    if (humidity < 30)
                    {
                        humidity_level = "dry";
                    }
                    else if (humidity > 65)
                    {
                        humidity_level = "damp";
                    }
                    else
                    {
                        humidity_level = "OK";
                    }

                    // Noise level determination
                    var noise_level = sound_sensor.Value > 55 ? "noisy" : "";

                    // Display formatted output
                    display.PrintAt(0, 0, $"{dht_sensor.Temperature:0.0}".PadRight(5));
                    display.PrintAt(8, 0, $"{humidity:0}% {humidity_level}".PadLeft(8));
                    display.PrintAt(0, 1, $"Lum:{light_level}".PadRight(10));
                    display.PrintAt(11, 1, noise_level.PadRight(5));

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(100);
                }
            }
        }
    }
}
