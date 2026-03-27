# Demonstrates how to detect IR remote command codes using All-in-one kit for Arduino.
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

    # Thread to detect Enter key
    detectEnterKey = threading.Thread(target = input)
    detectEnterKey.start()
   
    while detectEnterKey.is_alive():
        # Report any infra‑red command codes received.
        ir_cmd = all_in_one_kit.Digital.GetIRCommand()

        if ir_cmd.Code >= 0:
            state = "pressed" if ir_cmd.ButtonPressed else "released"
            print(f"IR Cmd: {ir_cmd.Code} {ir_cmd.Name} {state}")

        time.sleep(0.05)
finally:
    all_in_one_kit.Close()

