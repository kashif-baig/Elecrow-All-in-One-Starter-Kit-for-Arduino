# Uses separately available Grove I2C colour module based on TCS3472 sensor.
# Use cable to plug in to one of the I2C sockets of the of All-in-one kit for Arduino.
# Ensure the illumination LED is switched on the sensor module and place
# the sensor close to the surface of the colour being detected.
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

# Define the reference colours and their HSL ranges.
# Each entry is a tuple:
# (colour_name, hue_min, hue_max, sat_min, sat_max, light_min, light_max)
rubiks_colours = [
    ("white",   147, 153, 17, 20, 32, 33),
    ("red",       2,  17, 10, 28, 31, 37),
    ("orange",   29,  59, 15, 43, 29, 33),
    ("yellow",   78,  98, 23, 40, 30, 33),
    ("green",   121, 154, 24, 45, 32, 36),
    ("blue",    188, 204, 36, 56, 29, 32),
]

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press Enter to stop program.")

    colour_sensor = all_in_one_kit.ColourSensor
    colour_sensor.Enable()
    all_in_one_kit.WaitUntilSensorsReady(colour_sensor)
    
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Obtain the raw HSL values from the colour sensor.
        col = colour_sensor.GetHSL()

        # Find which reference colour matches the detected HSL values.
        for (
            colour_name,
            hue_min, hue_max,
            sat_min, sat_max,
            light_min, light_max,
        ) in rubiks_colours:
            if (
                hue_min <= col.Hue <= hue_max
                and sat_min <= col.Saturation <= sat_max
                and light_min <= col.Lightness <= light_max
            ):
                print(colour_name)

        time.sleep(0.1)
finally:
    all_in_one_kit.Close()

