# Demonstrates the blinking of the LED of All-in-one kit for Arduino.
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

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press Enter to stop program.")

    # This LED example uses the motor controlling logic.
    led = all_in_one_kit.Motor1
    # Cause LED to gradually change brightness.
    led.SetAcceleration(.5)

    # Alternatively, the LED can be driven as a simple On/Off switch.
    # led_alt = all_in_one_kit.Switch2

    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()

    # Initialize LED state
    led_is_on = False

    while detectEnterKey.is_alive():
        if led_is_on:
            # Drive LED at 50 % brightness (without gradual change)
            led.DriveNoAccel(50)
            led_is_on = False
        else:
            # Drive LED at 100 % brightness (gradually)
            led.Drive(100)
            led_is_on = True

        time.sleep(1)
finally:
    all_in_one_kit.Close()

