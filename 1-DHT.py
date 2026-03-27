# Demonstrates how to enable and read the digital humidity and temperature (DHT) sensor
# of All-in-one kit for Arduino.
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

    dht_sensor = all_in_one_kit.DHTSensor
    dht_sensor.Enable()
    all_in_one_kit.WaitUntilSensorsReady(dht_sensor)

    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Print temperature and humidity readings from a DHT sensor
        print(f"Temp {dht_sensor.Temperature:.1f}, Humidity {dht_sensor.Humidity:.1f}")

        time.sleep(0.05)
finally:
    all_in_one_kit.Close()

