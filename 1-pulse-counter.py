# Uses separately available IR line or proximity sensor connected to socket A3
# of All-in-one kit for Arduino. By alternating a reflective and non-reflective
# surface at the sensor, the frequency of the rate of surface change can be measured.
# This arrangement can be used for measuring RPM of wheels etc. 
#
# Robo-Tx firmware must be deployed to the Arduino. Before doing so, make sure
# SELECTED_PROFILE is set to PROFILE_ALL_IN_ONE_KIT_ARDU in file Settings.h 
#
# https://github.com/kashif-baig/RoboTx_Firmware
#
# Robo-Tx API online help: https://help.cohesivecomputing.co.uk/Robo-Tx
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

    timeout_ms = 1000;  # measurement cleared if no pulse detected within this period.
    trigger = 1;        # pulse start detected on high signal.

    pulse_counter = all_in_one_kit.PulseCounter
    pulse_counter.Enable(timeout_ms, trigger)
    
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        pulse_period = pulse_counter.Period

        if pulse_period > 0:
            pulse_frequency = 1000 / pulse_period
            print(f"Pulse freq: {pulse_frequency:.1f} hz")
        time.sleep(0.05)
finally:
    all_in_one_kit.Close()

