# A security application that uses the IR remote for entering a 4 digit
# pin code to switch on the relay to simulate the opening of an
# electronic lock.
# 
# Coding challenge:
# Part 1:
# Modify the source code to use a 6 digit pass key.
# Part 2:
# Modify the source code to clear the digits on the display if an incomplete
# code has been entered, but no key presses on the IR remote for five seconds
# or more.
#
# (C) Kashif Baig
# 
# Robo-Tx firmware must be deployed to the Arduino. Before doing so, make sure
# SELECTED_PROFILE is set to PROFILE_ALL_IN_ONE_KIT_ARDU in file Settings.h
#
# https://github.com/kashif-baig/RoboTx_Firmware
#
# Robo-Tx API online help: https://help.cohesivecomputing.co.uk/Robo-Tx
#
# Check settings in file app_config.py before running the code.
# All examples are provided as is and at user's own risk.

import threading
import time
from datetime import datetime,timedelta
from app_config import *

# Define IR command code to digit mapping
ir_cmd_code_to_digit = {
    22: "0",
    12: "1",
    24: "2",
    94: "3",
    8:  "4",
    28: "5",
    90: "6",
    66: "7",
    82: "8",
    74: "9",
}

# Function to convert IR command code to button name.
def convert_cmd_code(command_code: int) -> str:
    return ir_cmd_code_to_digit.get(command_code, "")

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Enter the pass key code using the IR remote.")
    print("Press Enter to stop program.")

    pass_key = "1234"
    code_entered = ""

    buzzer = all_in_one_kit.Trigger
    relay = all_in_one_kit.Switch1
    display = all_in_one_kit.Display

    digital_input = all_in_one_kit.Digital

    # Register function for converting IR command code to button name.
    digital_input.UseIrCommandConverter(IrCommandConverter(convert_cmd_code))
    
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Retrieve the latest IR command
        ir_cmd = digital_input.GetIRCommand()

        # Was an IR remote button pressed and a code received by the sensor?
        if ir_cmd.Received:
            if ir_cmd.ButtonPressed and ir_cmd.Name:
                # Format code entered for display
                if len(code_entered) >= 4:
                    code_entered = ""

                code_entered += ir_cmd.Name

                # Show the entered code, padded to 4 characters
                display.PrintAt(0, 0, code_entered.ljust(4))
                # Clear the second line (12 spaces)
                display.PrintAt(0, 1, " " * 12)

                # If the entered code length matches the expected PIN length, validate it
                if len(code_entered) == len(pass_key):
                    if code_entered == pass_key:
                        # Correct PIN code accepted.
                        buzzer.Pulse(1000)               # 1000 ms pulse
                        relay.OnForDuration(1.5)         # 1.5 seconds
                    else:
                        # Wrong PIN code.
                        display.PrintAt(0, 1, "Wrong code!")

                    # Small pause before clearing the display
                    time.sleep(0.5)

                    # Clear the entered code display
                    display.PrintAt(0, 0, " " * 4)
        time.sleep(0.05)
finally:
    all_in_one_kit.Close()

