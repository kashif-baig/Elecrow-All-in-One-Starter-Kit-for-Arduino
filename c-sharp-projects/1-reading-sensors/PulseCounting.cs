using RoboTx.Api;

namespace c_sharp_projects._1_reading_sensors
{
    internal class PulseCounting
    {
        /// <summary>
        /// Uses separately available IR line or proximity sensor connected to socket A3
        /// of All-in-one kit for Arduino. By alternating a reflective and non-reflective
        /// surface at the sensor, the frequency of the rate of surface change can be measured.
        /// This arrangement can be used for measuring RPM of wheels etc. 
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

                var timeout_ms = 1000;  // measurement cleared if no pulse detected within this period.
                var trigger = 1;        // pulse start detected on high signal.

                var pulse_counter = all_in_one_kit.PulseCounter;
                pulse_counter.Enable(timeout_ms, trigger);

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    var pulse_period = pulse_counter.Period;

                    if (pulse_period > 0)
                    {
                        var pulse_freq = 1000 / pulse_period;

                        Console.WriteLine($"Pulse freq: {pulse_freq} hz");
                    }
                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(200);
                }
            }
        }
    }
}
