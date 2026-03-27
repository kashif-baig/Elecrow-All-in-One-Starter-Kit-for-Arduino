namespace c_sharp_projects
{
    /// <summary>
    /// Get config values.
    /// </summary>
    internal class AppConfig
    {
        /// <summary>
        /// Get serial port name.
        /// Change port name to suit your environment.
        /// 
        /// Make sure the project references the Robo-Tx and System.IO.Ports
        /// assemblies specific to your environment under the RoboTx folder.
        /// </summary>
        public static string SerialPortName
        {
            get
            {
                return "COM4";
            }
        }
    }
}
