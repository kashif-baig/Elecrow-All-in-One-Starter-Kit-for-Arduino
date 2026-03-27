using RoboTx.Api;

namespace c_sharp_projects._1_reading_sensors
{
    internal class IrRemote
    {
        /// <summary>
        /// Demonstrates how to detect IR remote command codes using All-in-one kit for Arduino.
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

                while (all_in_one_kit.ConnectionState.IsConnected)
                {
                    // Report any infra-red command codes received.
                    var ir_cmd = all_in_one_kit.Digital.GetIRCommand();

                    if (ir_cmd.Code >= 0)
                    {
                        string state = ir_cmd.ButtonPressed ? "pressed" : "released";
                        Console.WriteLine($"IR Cmd: {ir_cmd.Code} {ir_cmd.Name} {state}");
                    }
                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;

                    Thread.Sleep(50);
                }
            }
        }
    }
}
