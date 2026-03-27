# Environment monitoring application that reports temperatue, relative
# humidity, ambient lighting and noise level.
# 
# Coding challenge:
# Part 1: use an array for the light level bands, and a loop to find
# the appropriate level from the actual LUX value. Try a variation
# of the binary search to find the light level.
# 
# Part 2: at one second intervals, append the sensor values as a line
# of comma separated values to a CSV file (create if not exists).
# The first value of every line must be the date and time in
# yyyy-MM-dd HH:mm:ss format.
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

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press Enter to stop program.")

    sound_sensor = all_in_one_kit.Analog.A1
    light_meter = all_in_one_kit.LightMeter
    dht_sensor = all_in_one_kit.DHTSensor

    light_meter.Enable()
    dht_sensor.Enable()
    all_in_one_kit.WaitUntilSensorsReady(light_meter, dht_sensor)

    display = all_in_one_kit.Display

    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Light level determination
        lux_value = light_meter.LuxValue
        if lux_value < 30:
            light_level = "dark"
        elif lux_value < 75:
            light_level = "dim"
        elif lux_value < 150:
            light_level = "soft"
        elif lux_value < 400:
            light_level = "OK"
        else:
            light_level = "bright"

        # Humidity level determination
        humidity = dht_sensor.Humidity
        if humidity < 30:
            humidity_level = "dry"
        elif humidity > 65:
            humidity_level = "damp"
        else:
            humidity_level = "OK"

        # Noise level determination
        noise_level = "noisy" if sound_sensor.Value > 55 else ""

        # Display output (using Python's string formatting and padding)
        display.PrintAt(0, 0, f"{dht_sensor.Temperature:.1f}".ljust(5))
        display.PrintAt(8, 0, f"{humidity:.0f}% {humidity_level}".rjust(8))
        display.PrintAt(0, 1, f"Lum:{light_level}".ljust(10))
        display.PrintAt(11, 1, noise_level.ljust(5))

        time.sleep(0.1)
finally:
    all_in_one_kit.Close()

