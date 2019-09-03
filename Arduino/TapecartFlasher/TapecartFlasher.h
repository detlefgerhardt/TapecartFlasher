//-------------------------------------------------------------------------------------------------
// TapecartFlasher.h   *dg*    22.10.2018
//-------------------------------------------------------------------------------------------------

#ifndef Tapecart_h
#define Tapecart_h

//-------------------------------------------------------------------------------------------------

#define FCSTR(str) (String(F(str)).c_str())

#include <Arduino.h>
#include "Sd2PinMap.h"
//#include <SD.h>

#define PIN_MOTOR 2
#define PIN_READ 3
#define PIN_WRITE 4
#define PIN_SENSE 5
#define PIN_LED 13
#define PIN_VOLTAGE_PUMP 9
#define PIN_SD_SEL 10

#define MAJOR_VERSION 0
#define MINOR_VERSION 5
#define API_VERSION 2

#define ARDUINO_UNO 1
#define ARDUINO_NANO 2
#define ARDUINO_MEGA2560 3
#define ARDUINO_PRO_MINI 4

#ifdef ARDUINO_AVR_UNO
#define ARDUINO_TYPE ARDUINO_UNO
#endif
#ifdef ARDUINO_AVR_NANO
#define ARDUINO_TYPE ARDUINO_NANO
#endif
#ifdef ARDUINO_AVR_MEGA2560
#define ARDUINO_TYPE ARDUINO_MEGA2560
#endif
#ifdef ARDUINO_AVR_LEONARDO
#define ARDUINO_TYPE ARDUINO_PRO_MINI
#endif
#ifdef ARDUINO_AVR_PRO
#define ARDUINO_TYPE ARDUINO_PRO_MINI
#endif

#define DATABUFFER_MAX 256
#define DATABUFFER_HEADER 10

#define CMD_PREFIX 0x01 // SOH
#define CMD_ENQ 0x05 // ENQ


//-------------------------------------------------------------------------------------------------

#endif

//-------------------------------------------------------------------------------------------------
// TapecartFlasher.h   *dg*    18.07.2018
//-------------------------------------------------------------------------------------------------

