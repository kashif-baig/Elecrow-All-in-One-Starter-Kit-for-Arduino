# Application for calculating the aperture F stop of a camera.
# Use the slider to select the shutter speed, and press the button to cycle
# through ISO values. Position the all in one kit where the photography
# subject is locationed, and allow the ambient light to fall on the light sensor.
# 
# Coding challenge:
# 
# Part 1: Modify the source code to use the IR remote to freeze/unfreeze the F stop
# reading on the LCD.
# Part 2: Modify the source code to support aperture priority. I.e., allow the user
# to select the F stop, and calculate the shutter speed accordingly.
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
import math

from app_config import *

# List of common shutter speeds
SHUTTER_SPEEDS = [
    3, 4, 5, 6, 10, 13, 15, 20, 25, 30, 40, 50, 60, 80,
    100, 125, 160, 200, 250, 320, 400, 500, 640, 800, 1400
]

# Convert an analog sensor reading (0‑1023) to the nearest shutter speed
# from the predefined SHUTTER_SPEEDS list.
def map_to_shutter_speed(analog_value: float) -> float:
    max_index = len(SHUTTER_SPEEDS) - 1
    index = int((max_index * analog_value) / 1023)

    # Guard against potential out‑of‑range values
    index = max(0, min(index, max_index))
    return SHUTTER_SPEEDS[index]

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press Enter to stop program.")

    # F stop calibration constant.
    calibration_const = 330

    light_meter = all_in_one_kit.LightMeter
    light_meter.Enable()
    all_in_one_kit.WaitUntilSensorsReady(light_meter)

    display = all_in_one_kit.Display
    shutter = all_in_one_kit.Analog.A0
    all_in_one_kit.Analog.UseConverter(AnalogConverter(map_to_shutter_speed), shutter)

    iso = 100
    display.PrintAt(6, 0, f"{iso}")
                                
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Get the current input event (button press)
        input_event = all_in_one_kit.Digital.GetInputEvent()

        # Handle button‑1 release: double ISO, wrap around at 800, and show it
        if input_event == Input.BUTTON_1_RELEASED:
            iso *= 2
            if iso > 800:
                iso = 100
            display.PrintAt(6, 0, f"{iso}")

        # Compute the f‑stop value
        f_stop = math.sqrt((light_meter.LuxValue * iso * (1 / shutter.Value)) / calibration_const)

        # Display the f‑stop (or an error) left‑justified to a width of 5 characters
        if 1.8 <= f_stop <= 22:
            display.PrintAt(0, 0, f"F{f_stop:.1f}".ljust(5))
        else:
            display.PrintAt(0, 0, "Error".ljust(5))

        # Display the shutter speed, right‑justified to a width of 6 characters
        display.PrintAt(10, 0, f"1/{shutter.Value:.0f}".rjust(6))

        time.sleep(0.1)
finally:
    all_in_one_kit.Close()

