# A metronome application that uses the slider (linear potentiometer) to control the
# tempo, and the holding of the button to cycle through time signatures 2/4 to 4/4.
# Press the button to stop/start the metronome.
# 
# Coding challenge:
# 
# Part 1: Modify the source code to trigger the metronome with a hand clap. 
# Part 2: Modify to use the IR remote to start/stop the metronome.
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

# Convert raw analog value to BPM
def convert_to_bpm(value):
    return ((180 * value) / 1023) + 60

all_in_one_kit = RobotIO(serial_port)
try:
    all_in_one_kit.Connect()
    print("Press button to start/stop metronome.")
    print("Press Enter to stop program.")

    led = all_in_one_kit.Switch2
    tempo_bpm = all_in_one_kit.Analog.A0
    beeper = all_in_one_kit.Trigger
    display = all_in_one_kit.Display

    # Register converter function to convert slider analog value to BPM (60 to 240). 
    all_in_one_kit.Analog.UseConverter(AnalogConverter(convert_to_bpm), tempo_bpm)

    signature = 4           # must be between 2 to 4
    beat_counter = 0
    beat_interval = 0       # beat interval in milliseconds
    last_beat_time = datetime.now()
    metronome_active = False

    display.PrintAt(6, 0, f"{signature}/4")
                                
    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Calculate beat interval using tempo and signature settings
        # Allows for n/8 signature, although not currently used.
        beat_interval = 240_000 / (
            int(tempo_bpm.Value) * (4 if signature <= 4 else 8)
        )

        # Retrieve the latest digital input event (button press)
        input_event = all_in_one_kit.Digital.GetInputEvent()

        if input_event == Input.BUTTON_1_RELEASED:
            # Start/stop metronome on button 1 press/release.
            metronome_active = not metronome_active
            beat_counter = 0
            # Set the last beat time a full interval in the past
            last_beat_time = datetime.now() - timedelta(
                milliseconds=beat_interval
            )

        elif input_event == Input.BUTTON_1_SUSTAINED:
            # Cycle through time signatures while the button is held.
            signature += 1
            if signature > 4:
                signature = 2
            display.PrintAt(6, 0, f"{signature}/4")

        # Time elapsed since the previous beat (in milliseconds)
        time_diff = (datetime.now() - last_beat_time).total_seconds() * 1000

        if metronome_active and time_diff >= beat_interval:
            # Update last beat time while minimizing drift.
            adjustment = time_diff - beat_interval
            last_beat_time = datetime.now() - timedelta(
                milliseconds=adjustment
            )

            if beat_counter < 1 or beat_counter >= signature:
                beat_counter = 1
                beeper.Pulse(100)                # Longer pulse for down‑beat
                led.OnForDuration(0.1)           # LED on for 0.1 s
            else:
                beat_counter += 1
                beeper.Pulse(10)                 # Shorter pulse for other beats

            display.PrintAt(0, 0, str(beat_counter))

        # Show the tempo value (right‑aligned, 3 characters wide)
        display.PrintAt(13, 0, str(int(tempo_bpm.Value)).rjust(3))

        time.sleep(0.02)
finally:
    all_in_one_kit.Close()

