# A proximity security light that switches on when motion is detected by the PIR
# sensor, and when the ambient light level is below the threshold set by the slider
# (linear potentiometer).
# 
# Coding challenge:
# Part 1: Show both the current light meter LUX value and the LUX threshold value on the LCD.
# Part 2: Add a further condition where an object must be within 50cm of the sonar sensor
#         before LED and relay are switched on.
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

from app_config import *

# Converts raw analog value of slider to a value range that is suitable for
# setting the LUX threshold.
def convert_to_lux_threshold(value):
    return (200 * value) / 1023

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press Enter to stop program.")

    relay = all_in_one_kit.Switch1
    led = all_in_one_kit.Switch2
    motion_detected = all_in_one_kit.Digital.IN1
    lux_threshold = all_in_one_kit.Analog.A0

    # Register function to convert raw slider value to LUX threshold value.
    all_in_one_kit.Analog.UseConverter(AnalogConverter(convert_to_lux_threshold), lux_threshold)

    light_meter = all_in_one_kit.LightMeter
    light_meter.Enable()

    all_in_one_kit.WaitUntilSensorsReady(light_meter)
    
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Has motion been detected and ambient lighting lower than threshold?
        if motion_detected.Value and light_meter.LuxValue <= lux_threshold.Value:
            if not led.IsOn:
                # Switch on LED and relay
                led.On()
                relay.On()
        else:
            if led.IsOn:
                # Switch off LED and relay
                led.Off()
                relay.Off()
        time.sleep(0.05)
finally:
    all_in_one_kit.Close()

