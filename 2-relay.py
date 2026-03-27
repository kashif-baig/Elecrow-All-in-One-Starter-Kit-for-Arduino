# Demonstrate switching relay on and off using the slider of All-in-one kit for Arduino.
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
    print("Use the slider to switch relay on/off.")
    print("Press Enter to stop program.")

    relay = all_in_one_kit.Switch1
    slider = all_in_one_kit.Analog.A0
    
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        if slider.Value > 511:
            relay.On()
        else:
            relay.Off()

        time.sleep(0.05)
finally:
    all_in_one_kit.Close()

