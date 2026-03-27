# Demonstrates how to sound the buzzer using different repeating patterns
# using All-in-one kit for Arduino.
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
    # Buzzer uses the firmware's trigger logic.
    buzzer = all_in_one_kit.Trigger

    print("Smoke alarm pattern.")
    buzzer.RunPattern(
        500,    # on for 500ms.
        500,    # off for 500ms.
        3,      # repeat on/off cycle 3 times.
        3,      # repeat above pattern 3 times
        1000    # with interval of 1000ms.
        )
    time.sleep(8)

    print("Alarm clock pattern.")
    buzzer.RunPattern(50, 50, 4, 3, 500)
    time.sleep(3)
    
    print("Reversing vehicle pattern.")
    buzzer.RunPattern(500, 500, 10)
    
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()

    print("Press Enter to stop program.")

    while detectEnterKey.is_alive():

        time.sleep(0.05)
finally:
    all_in_one_kit.Close()

