# A countdown timer that uses the slider to set the timer duration,
# and button to start/stop and reset the countdown (when held).
#
# Coding challenge:
# Modify the source code to implement an intervalometer whereby
# the relay is temporarily switched on at one second intervals
# during the the countdown. Use relay.OnForDuration() with a duration
# parameter.
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
from datetime import datetime,timedelta
from app_config import *

# Convert raw analog value to duration
def convert_to_duration(value):
    return ((600 * value) / 1023)

# Helper method to format time for display.
def to_time_format(seconds):
    return f"{seconds // 60:02}:{str(seconds % 60).zfill(2)}"

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press Enter to stop program.")

    beeper = all_in_one_kit.Trigger
    display = all_in_one_kit.Display
    slider = all_in_one_kit.Analog.A0
    relay = all_in_one_kit.Switch1

    # Register function to convert slider to alarm duration.
    all_in_one_kit.Analog.UseConverter(AnalogConverter(convert_to_duration), slider)
    
    countdown_running = False
    timer_reset = True
    alarm_sound_active = False
    timer_value_seconds = slider.Value
    target_time = datetime.now() + timedelta(seconds=timer_value_seconds)
    
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        input_event = all_in_one_kit.Digital.GetInputEvent()

        if alarm_sound_active and input_event == Input.BUTTON_1_RELEASED:
            # Turn off alarm if sounding.
            alarm_sound_active = False
            beeper.Off()
        elif not countdown_running:
            if input_event == Input.BUTTON_1_RELEASED:
                # Start the countdown if not running.
                target_time = datetime.now() + timedelta(seconds=timer_value_seconds)
                countdown_running = timer_value_seconds > 0
                timer_reset = False
            elif input_event == Input.BUTTON_1_SUSTAINED or timer_reset:
                # Reset countdown timer.
                timer_value_seconds = int(slider.Value)
                display.PrintAt(0, 0, to_time_format(timer_value_seconds))
                target_time = datetime.now() + timedelta(seconds=timer_value_seconds)
                timer_reset = True
        else:
            if input_event == Input.BUTTON_1_RELEASED:
                # Stop countdown if running.
                countdown_running = False
            else:
                # Update display with changing countdown.
                timer_value_seconds = int((target_time - datetime.now()).total_seconds())
                display.PrintAt(0, 0, to_time_format(timer_value_seconds))

                if timer_value_seconds <= 0:
                    # Countdown has completed, sound the alarm.
                    countdown_running = False
                    alarm_sound_active = True
                    beeper.RunPattern(50, 50, 4, 3, 500)

        time.sleep(0.05)
finally:
    all_in_one_kit.Close()

