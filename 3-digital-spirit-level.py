# A digital spirit level that uses the accelerometer readings of MPU6050 sensor
# to measure the incline angle of a surface.
# 
# Coding challenge:
# Part 1:
# Modify the source code to write the value of variable angle_offset to a file after
# calibration is complete. Overwrite the file if it already exists. After the program
# starts, load the value from the file (if it exists) back in to the variable angle_offset.
# 
# Part 2:
# Stabilize the angle readings by retaining the five most recent angles in an array and
# calculating and reporting their average. Which provides a more stable result, mean average
# or median average?
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
import time, math

from app_config import *

rad_to_deg = 180 / 3.14159

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press and hold the button to calibrate the sensor on a level surface.")
    print("Press Enter to stop program.")

    display = all_in_one_kit.Display
    mpu_sensor = all_in_one_kit.MPUSensor

    mpu_sensor.Enable()
    all_in_one_kit.WaitUntilSensorsReady(mpu_sensor)

    angle_offset = 0.0
    calibration_mode = False
    
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Read the latest digital input event (in this case the button)
        input_event = all_in_one_kit.Digital.GetInputEvent()

        # ------------------------------------------------------------
        # Handle button‑1 sustained press → start calibration
        if input_event == Input.BUTTON_1_SUSTAINED and not calibration_mode:
            print("Calibrating")
            calibration_mode = True
            angle_offset = 0.0

            # Simple sensor calibration:
            # average 10 readings taken on a flat surface
            for _ in range(10):
                # atan returns radians; convert to degrees
                angle_offset += math.atan(mpu_sensor.Accel.X / mpu_sensor.Accel.Z) * rad_to_deg
                time.sleep(0.1)           # 100 ms pause between samples

            angle_offset /= 10.0         # final offset (average)

            # Flush any pending input events
            all_in_one_kit.Digital.ClearInputEvents()

        # ------------------------------------------------------------
        # Handle button‑1 release → exit calibration mode
        elif input_event == Input.BUTTON_1_SUSTAIN_RELEASED:
            calibration_mode = False

        # ------------------------------------------------------------
        # Compute the current slope angle (degrees)
        #   subtract the calibrated offset from the sensor reading
        angle = -angle_offset + math.atan(mpu_sensor.Accel.X / mpu_sensor.Accel.Z) * rad_to_deg

        # Display the angle, right‑aligned in a 6‑character field
        display.PrintAt(4, 0, f"{angle:.1f}".rjust(6))

        # ------------------------------------------------------------
        # Build a simple level indicator bar (max length = 16 chars)
        if angle >= 0:
            # Positive tilt: dashes on the left side
            dash_count = int(min(angle, 8))
            level_indicator = ('-' * dash_count).rjust(8) + ' ' * 8
        else:
            # Negative tilt: dashes on the right side
            dash_count = int(min(-angle, 8))
            level_indicator = ' ' * 8 + ('-' * dash_count).ljust(8)

        # Show the level indicator bar on the display
        display.PrintAt(0, 1, level_indicator)

        time.sleep(0.2)
finally:
    all_in_one_kit.Close()

