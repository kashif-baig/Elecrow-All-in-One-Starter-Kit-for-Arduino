# ... example using Robo-Tx framework
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

from app_config import *

# Convert raw analog value to percent
def convert_to_percent(value):
    return (100 * value)/1023

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press Enter to stop program.")

    beeper = all_in_one_kit.Trigger
    soil_moisture = all_in_one_kit.Analog.A6

    # Register function to convert raw moisture sensor value to percentage.
    all_in_one_kit.Analog.UseConverter(AnalogConverter(convert_to_percent), soil_moisture)

    display = all_in_one_kit.Display

    display.PrintAt(0, 0, "Soil moist:")
    
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Get the current input event (i.e. button events)
        input_event = all_in_one_kit.Digital.GetInputEvent()

        # Check if button 1 was pressed
        if input_event == Input.BUTTON_1_PRESSED:
            # TODO: implement button‑1 pressed handling
            pass

        # Show the soil moisture percentage, padded to a width of 4 characters
        moisture_percent = f"{int(soil_moisture.Value)}%".rjust(4)
        display.PrintAt(11, 0, moisture_percent)
        time.sleep(0.1)
finally:
    all_in_one_kit.Close()

