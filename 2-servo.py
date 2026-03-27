# Demonstrates the setting of the servo motor position of All-in-one kit for Arduino
# using the slider constrol. If insufficient power is supplied
# to the All-in-one Kit, it may reset and temporarily disconnect
# from the PC, causing the program to end.
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

# Convert raw analog value to angle
def convert_to_angle(value):
    return (180 * value)/1023

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press Enter to stop program.")

    servo = all_in_one_kit.Servo1
    slider = all_in_one_kit.Analog.A0
    
    # Register converter function to convert slider analog value to angle.
    all_in_one_kit.Analog.UseConverter(AnalogConverter(convert_to_angle), slider)

    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        print(f"Converted slider value: {slider.Value:.1f}")
        servo.SetPosition(slider.Value)
    
    time.sleep(0.05)
finally:
    all_in_one_kit.Close()

