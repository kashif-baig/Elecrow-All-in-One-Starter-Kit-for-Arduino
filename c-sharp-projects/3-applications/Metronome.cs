using RoboTx.Api;

namespace c_sharp_projects._3_applications
{
    internal class Metronome
    {
        /// <summary>
        /// A metronome application that uses the slider (linear potentiometer) to control the
        /// tempo, and the holding of the button to cycle through time signatures 2/4 to 4/4.
        /// Press the button to stop/start the metronome.
        /// 
        /// Coding challenge:
        /// 
        /// Part 1: Modify the source code to trigger the metronome with a hand clap. 
        /// Part 2: Modify to use the IR remote to start/stop the metronome.
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
                Console.WriteLine("Press button to start/stop metronome.");
                Console.WriteLine("Press Esc to stop the program.");

                var led = all_in_one_kit.Switch2;
                var tempo_bpm = all_in_one_kit.Analog.A0;
                var beeper = all_in_one_kit.Trigger;
                var display = all_in_one_kit.Display;

                // Register converter function to convert slider analog value to BPM (60 to 240). 
                all_in_one_kit.Analog.UseConverter(ConvertToBpm, tempo_bpm);

                var signature = 4;      // must be between 2 to 4
                var beat_counter = 0;
                var beat_interval = 0;  // beat interval in milliseconds
                var last_beat_time = DateTime.Now;
                var metronome_active = false;

                display.PrintAt(6, 0, $"{signature}/4");

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    // Calculate beat interval using tempo and signature settings
                    // Allow for n/8 signature, although not currently used.
                    beat_interval = 240000 / ((int)tempo_bpm.Value * (signature <= 4 ? 4 : 8));

                    // Retrieve the latest digital input event (button press)
                    var input_event = all_in_one_kit.Digital.GetInputEvent();

                    if (input_event == Input.BUTTON_1_RELEASED)
                    {
                        // Start/stop metronome on button 1 press/release.
                        metronome_active = !metronome_active;
                        beat_counter = 0;
                        // Set the last beat time a full interval in the past
                        last_beat_time = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(beat_interval));
                    }
                    else if (input_event == Input.BUTTON_1_SUSTAINED)
                    {
                        // Cycle through time signatures while the button is held.
                        signature++;
                        if (signature > 4)
                        {
                            signature = 2;
                        }
                        display.PrintAt(6, 0, $"{signature}/4");
                    }
                    // Time elapsed since the previous beat (in milliseconds)
                    var time_diff = (DateTime.Now - last_beat_time).TotalMilliseconds;

                    if (metronome_active && time_diff >= beat_interval)
                    {
                        // Update last beat time, whilst minimising time drift.
                        last_beat_time = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(time_diff - beat_interval));

                        if (beat_counter < 1 || beat_counter >= signature)
                        {
                            beat_counter = 1;
                            beeper.Pulse(100);      // Longer pulse for down‑beat
                            led.OnForDuration(.1f);
                        }
                        else
                        {
                            beat_counter++;
                            beeper.Pulse(10);       // Shorter pulse for other beats
                        }
                        display.PrintAt(0, 0, beat_counter.ToString());
                    }
                    // Show the tempo value (right‑aligned, 3 characters wide)
                    display.PrintAt(13, 0, ((int)tempo_bpm.Value).ToString().PadLeft(3));

                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(20);
                }
            }
        }

        /// <summary>
        /// Converts raw analog value (0 to 1023) to tempo (60 to 240 beats per minute).
        /// </summary>
        /// <param name="analogValue"></param>
        /// <returns></returns>
        static float ConvertToBpm(float analogValue)
        {
            return ((180 * analogValue) / 1023) + 60;
        }
    }
}
