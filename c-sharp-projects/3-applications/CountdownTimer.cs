using RoboTx.Api;

namespace c_sharp_projects._3_applications
{
    internal class CountdownTimer
    {
        /// <summary>
        /// A countdown timer that uses the slider to set the timer duration,
        /// and button to start/stop and reset the countdown.
        /// 
        /// Coding challenge:
        /// Modify the source code to implement an intervalometer whereby
        /// the relay is temporarily switched on at one second intervals
        /// during the the countdown. Use relay.OnForDuration() with a duration
        /// parameter.
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
                var display = all_in_one_kit.Display;
                var slider = all_in_one_kit.Analog.A0;
                var relay = all_in_one_kit.Switch1;

                // Register function to convert slider to alarm duration.
                all_in_one_kit.Analog.UseConverter(ConvertToDuration, slider);

                var countdown_running = false;
                var timer_reset = true;
                var alarm_sound_active = false;
                var timer_value_seconds = (int)slider.Value;
                var target_time = DateTime.Now.AddSeconds(timer_value_seconds);

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    var input_event = all_in_one_kit.Digital.GetInputEvent();

                    if (alarm_sound_active && input_event == Input.BUTTON_1_RELEASED)
                    {
                        // Turn off alarm if sounding.
                        alarm_sound_active = false;
                        beeper.Off();
                    }
                    else if (!countdown_running)
                    {
                        if (input_event == Input.BUTTON_1_RELEASED)
                        {
                            // Start the countdown if not running.
                            target_time = DateTime.Now.AddSeconds(timer_value_seconds);
                            countdown_running = timer_value_seconds > 0;
                            timer_reset = false;
                        }
                        else if (input_event == Input.BUTTON_1_SUSTAINED || timer_reset)
                        {
                            // Reset countdown timer.
                            timer_value_seconds = (int)slider.Value;
                            display.PrintAt(0, 0, ToTimeFormat(timer_value_seconds));
                            target_time = DateTime.Now.AddSeconds(timer_value_seconds);
                            timer_reset = true;
                        }
                    }
                    else
                    {
                        if (input_event == Input.BUTTON_1_RELEASED)
                        {
                            // Stop countdown if running.
                            countdown_running = false;
                        }
                        else
                        {
                            // Update display with changing countdown.
                            timer_value_seconds = (int)(target_time - DateTime.Now).TotalSeconds;
                            display.PrintAt(0, 0, ToTimeFormat(timer_value_seconds));

                            if (timer_value_seconds <= 0)
                            {
                                // Countdown has completed, sound the alarm.
                                countdown_running = false;
                                alarm_sound_active = true;

                                beeper.RunPattern(50, 50, 4, 3, 500);
                            }
                        }
                    }
                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        /// Function to convert slider value to timer duration.
        /// </summary>
        /// <param name="analogValue"></param>
        /// <returns></returns>
        static float ConvertToDuration(float analogValue)
        {
            return ((int)((600 * analogValue) / 1023));
        }

        /// <summary>
        /// Function to format seconds in to mm:ss format.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        static string ToTimeFormat(int seconds)
        {
            return $"{seconds / 60:00}:{seconds % 60:00}";
        }
    }
}
