using RoboTx.Api;

namespace c_sharp_projects._3_applications
{
    internal class PinCodeSecurity
    {
        /// <summary>
        /// A security application that uses the IR remote for entering a 4 digit
        /// pin code to switch on the relay to simulate the opening of an
        /// electronic lock.
        /// 
        /// Coding challenge:
        /// Part 1:
        /// Modify the source code to use a 6 digit pass key.
        /// Part 2:
        /// Modify the source code to clear the digits on the display if an incomplete
        /// code has been entered, but no key presses on the IR remote for five seconds
        /// or more.
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
                Console.WriteLine("Enter the pass key code using the IR remote.");
                Console.WriteLine("Press Esc to stop the program.");

                var pass_key = "1234";
                var code_entered = "";

                var buzzer = all_in_one_kit.Trigger;
                var relay = all_in_one_kit.Switch1;
                var display = all_in_one_kit.Display;

                var digital_input = all_in_one_kit.Digital;

                // Register function for converting IR command code to button name.
                digital_input.UseIrCommandConverter(ConvertCmdCode);

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    var ir_cmd = digital_input.GetIRCommand();

                    // Was IR remote button pressed and code received by sensor?
                    if (ir_cmd.Received)
                    {
                        if (ir_cmd.ButtonPressed && !string.IsNullOrEmpty(ir_cmd.Name))
                        {
                            // Format code entered for display
                            if (code_entered.Length >= 4)
                            {
                                code_entered = string.Empty;
                            }
                            code_entered += ir_cmd.Name;

                            // Show the entered code, padded to 4 characters
                            display.PrintAt(0, 0, code_entered.PadRight(4));
                            // Clear the second line (12 spaces)
                            display.PrintAt(0, 1, new string(' ', 12));

                            // If the entered code length matches the expected PIN length, validate it
                            if (code_entered.Length == pass_key.Length)
                            {
                                if (code_entered == pass_key)
                                {
                                    // Correct pin code accepted.
                                    buzzer.Pulse(1000);         // 1000 ms pulse
                                    relay.OnForDuration(1.5f);  // 1.5 seconds
                                }
                                else
                                {
                                    // Wrong pin code.
                                    display.PrintAt(0, 1, "Wrong code!");
                                }
                                // Small pause before clearing the display
                                Thread.Sleep(500);
                                // Clear the entered code display
                                display.PrintAt(0, 0, new string(' ', 4));
                            }
                        }
                    }
                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(50);
                }
            }
        }

        // Define IR command code to button name dictionary.
        static Dictionary<int, string> cmd_dict =
            new Dictionary<int, string>{
                { 22, "0" },
                { 12, "1" },
                { 24, "2" },
                { 94, "3" },
                { 8,  "4" },
                { 28, "5" },
                { 90, "6" },
                { 66, "7" },
                { 82, "8" },
                { 74, "9" },
            };

        /// <summary>
        /// Function to convert IR command code to button name.
        /// </summary>
        /// <param name="cmd_code"></param>
        /// <returns></returns>
        static string ConvertCmdCode(int cmd_code)
        {
            if (cmd_dict.TryGetValue(cmd_code, out var button_name))
            {
                return button_name;
            }
            return string.Empty;
        }
    }
}
