# Install a version of Python no later than 3.13, and dot net version no earlier than 8.0.
# Ensure pythonnet is installed by issuing the following on a command line of the
# selected Python environment:
# pip install pythonnet

from pythonnet import load
load("coreclr")
import clr

# Use correct path for your OS platform.
clr.AddReference("./RoboTx/win-x64/Robo-Tx.Api")
from RoboTx.Api import RobotIO, Input, AnalogConverter, IrCommandConverter
from RoboTx import *

# If connecting to an Arduino using USB, use Device Manager (Windows OS) to identify the COM port.
# For Linux, try the default port "/dev/ttyACM0".
serial_port = "COM4"

