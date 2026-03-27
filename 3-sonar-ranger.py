# Sonar ranging application that uses sonar to measure distance (max 250 cm) of object
# from sensor. Sound is emmited from the buzzer with an interval that corresponds to
# the distance.
# 
# Coding challenge:
# Modify the source code to generate a string of length 16 characters consisting of
# hyphens (-), where the number of hyphens is proportional to the distance measured.
# Pad the remainder of the string with spaces. Display the string on the second row
# of the LCD.
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

    display = all_in_one_kit.Display
    beeper = all_in_one_kit.Trigger
    # Start beeper repeating indefinitely: 50ms beep, 950ms interval.
    beeper.Repeat(50, 950)

    sonar = all_in_one_kit.Sonar
    
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Emit ping from sonar module.
        sonar.Ping()

        # Proceed only if a distance measurement is available
        if sonar.DistanceAcquired:
            # Retrieve the measured distance in centimeters
            distance_cm = sonar.GetDistance()

            # Consider only valid, non‑zero distances less than 250 cm
            if distance_cm != 0 and distance_cm < 250:
                # Calculate beep interval (in milliseconds) to correspond to distance.
                beep_interval = (distance_cm * 10) - 30

                # Ensure the interval is not negative
                if beep_interval < 0:
                    beep_interval = 0

                # Set beeper interval.
                beeper.SetOffPeriod(beep_interval)

            # Show the distance on the display, padded to three digits
            display.PrintAt(0, 0, f"{distance_cm:03d} cm")

        time.sleep(0.1)
finally:
    all_in_one_kit.Close()

