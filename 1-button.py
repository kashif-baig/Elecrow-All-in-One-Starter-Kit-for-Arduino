# Demonstrates how to sense button press and release actions of All-in-one kit for Arduino.
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
    print("Press, release, hold then release the button on the all in one kit.")
    print("Press Enter to stop program.")

    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Obtain the current button event from the All‑In‑One kit
        button_event = all_in_one_kit.Digital.GetInputEvent()

        # Handle each possible button state
        if button_event == Input.BUTTON_1_PRESSED:
            print("Button 1 pressed")
        elif button_event == Input.BUTTON_1_SUSTAINED:
            print("Button 1 held")
        elif button_event == Input.BUTTON_1_RELEASED:
            print("Button 1 released")
        elif button_event == Input.BUTTON_1_SUSTAIN_RELEASED:
            print("Button 1 released from hold")

        time.sleep(0.05)
finally:
    all_in_one_kit.Close()

