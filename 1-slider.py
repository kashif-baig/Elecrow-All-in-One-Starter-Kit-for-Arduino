# Demonstrates how to read the linear potentiometer position of All-in-one kit for Arduino.
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

def convert_to_percent(analog_value: float) -> float:
    # Map the raw analog value to a percentage (0-100)
    return (analog_value / 1023) * 100


all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press Enter to stop program.")

    slider = all_in_one_kit.Analog.A0
    # Register a function to map the raw analog value to a percentage
    all_in_one_kit.Analog.UseConverter(AnalogConverter(convert_to_percent), slider)

    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        print(f"Slider value: {slider.Value:.1f}")
        time.sleep(0.05)
finally:
    all_in_one_kit.Close()

