# TapecartFlasher
TapecartFlasher is a Windows tool to write and read out 
C64 Tapecart modules directly from the PC via an Arduino
UNO or NANO.

At the first use of the Arduino, the Arduino software can
from TapecartFlasher to Ardunio.
Each connection recording (Connect) with the Arduino will check if
the Arduino software is up to date.
The Arduino development environment is not needed for this.
The hex files for the used Arduino board must be in
same directory as TapecartFlasher.exe.

Procedure for programming a TCRT file:

1. Connect the Tapecart module to the Arduino via an adapter.

Wiring:

Arduino! Tapecart
------------------
GND    ! GND
+ 5V   ! + 5V
Pin D2 ! MOTOR
Pin D3 ! READ
Pin D4 ! WRITE
Pin D5 ! SENSE

2. Connect the Arduino via USB to the PC

3. Please check that the Arduino is properly recognized by Windows
and appears as a COM port in Device Manager.
You may still need to match the Arduino USB driver
be installed.

4. Start TapecartFlasher

5. Click on "Detect". If the Arduino already has a TapecartFlasher
Software, the COM port would have to be found and displayed.

6. If the Arduino does not yet contain TapecartFlasher software,
select the correct COM port, click on "Upload / Update Sketch",
choose the right Arduino board and click Update.
The software will now be transferred to the Arduino.
Close the update window and click on "Detect" again.
The correct COM port should now be displayed.

7. Click on "Connect". After a few seconds, some should
Information about the connected Tapecart module will be displayed.

8. With the "Write" or "Read" button can now a TCRT file
written or read.

In case of problems please contact  feedback@dgerhardt.de or user detlef at 
forum64.de.

Have fun.

-------------------------------------------------- ------------------------

Credits

This software uses the ArduinoSketchUploader Library by Christophe Diericx.

https://github.com/christophediericx

-------------------------------------------------- ------------------------
