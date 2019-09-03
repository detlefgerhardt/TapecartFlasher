//--------------------------------------------------------------------------
// TapecartFlasher    *dg*    22.10.2018
// Arduino Sketch for programming the Tapecart module with an
// Arduino UNO/NANO/MEGA/ProMicro
//
// Neded libraries:
// https://github.com/greiman/SdFat
//
//--------------------------------------------------------------------------
//
// Changes:
//
// 05.07.2018 *dg* Project start
// 18.07.2018 *dg* Implemented all neccessary api functions
//                 Renamed project to TapecartFlasher
//                 Added xor checksum to protcol
// 19.07.2018 *dg* Implemented Erase64K
//                 Implemented 32 Byte block download from PC
//                 Fixed bit shifting errors
// 20.07.2018 *dg* Version 1.0/1 released
// 27.07.2018 *dg* API-Funktion LedOn / LedOff implementiert
// 28.08.2018 *dg* Enabled SD card support
//                 Implemented Terminal console for SD card support
//                 Fixed Flashing from SD card 
//                 Implemented read Tapecart to SD card
// 02.09.2018 *dg* Included enhancements from bigby (frequency output for voltage pump)
// 04.09.2018 *dg* Improved error handling when Tapecart module is not detected
//                 Support for Arduino Leonardo / ProMirco
// 11.09.2018 *dg* Improved terminal console
// 11.10.2018 *dg* Further improved terminal console (smaller menu)
//                 Improved init command in terminal console for Tapecart module hot swap.
//                 Now uses SdFat library for long file names.
// 20.10.2018 *dg* Release 0.4 / API-Version 2
// 22.10.2018 *dg* ProMini sent the wrong type number.
//            *dg* Problems when terminal progamm sends CR/LF instead of CR.
//            *dg* Release 0.5 / API-Version 2
//
//--------------------------------------------------------------------------

#include "TapecartFlasher.h"
#include "CmdMode.h"
#include "Tcrt.h"
#include "PcCommands.h"
#include "Console.h"
#include <SdFat.h>

#include "Crc.h"

#define USE_SDCARD

// because of the small memory of the Arduino UNO we use only one buffer for
// communication with the pc an the communication with the Tapecart module.
// To handle one data block we need 256 byte + protocol header
uint8_t databuffer[DATABUFFER_MAX + DATABUFFER_HEADER];
char printbuffer[80];

CmdMode tc_cmd;
PcCommands pc_cmd;
Console pc_con;
SdFat sd;
bool sdCardOk = false;

//--------------------------------------------------------------------------

void setup()
{
  pinMode(PIN_MOTOR, OUTPUT);
  pinMode(PIN_READ, INPUT_PULLUP);
  pinMode(PIN_WRITE, OUTPUT);
  pinMode(PIN_SENSE, INPUT);
  pinMode(PIN_LED, OUTPUT);
  pinMode(PIN_VOLTAGE_PUMP, OUTPUT);
  pinMode(PIN_SD_SEL, OUTPUT);
  digitalWrite(PIN_SD_SEL, HIGH);

  // PWM freq to 31kHz and 50% duty cycle
  TCCR1B = (TCCR1B & 0b11111000) | 0x01;
  analogWrite(PIN_VOLTAGE_PUMP, 127);

  Serial.begin(115000);
  while (!Serial) ; // for Leonardo
  Serial.setTimeout(1000);

  //sdInfo();
  sdCardOk = sd.begin(PIN_SD_SEL, SD_SCK_MHZ(50));
  //sdCardOk = SD.begin(PIN_SD_SEL);
  if (!sdCardOk)
  {
    Serial.println(F("*SD card failed!"));
  }
  else
  {
    //Serial.println(F("*SD card ok"));
  }

  if (!tc_cmd.start())
  {
    Serial.println(F("*error starting tapecart"));
    return;
  }
}

//--------------------------------------------------------------------------

void loop()
{
  byte ch = PcCommands::recv_byte();
  if (ch==CMD_PREFIX)
  {
    // api command
    pc_cmd.readCmd();
  }
  else if (ch==CMD_CON)
  {
    // start terminal console
    ch = pc_con.start();
    // check for api command when leaving console
    if (ch==CMD_PREFIX)
      pc_cmd.readCmd();
  }
}

//--------------------------------------------------------------------------

#if false

Sd2Card sdCard;
SdVolume sdVolume;
SdFile sdRoot;

void sdInfo()
{
  if (!sdCard.init(SPI_HALF_SPEED, SD_SEL))
  {
    Serial.println(F("*sdInit error"));
    return;
  }
  
  Serial.println(F("*sdInit ok"));

   // print the type of card
  Serial.print(F("Card type: "));
  switch (sdCard.type())
  {
    case SD_CARD_TYPE_SD1:
      Serial.println(F("SD1"));
      break;
    case SD_CARD_TYPE_SD2:
      Serial.println(F("SD2"));
      break;
    case SD_CARD_TYPE_SDHC:
      Serial.println(F("SDHC"));
      break;
    default:
      Serial.println(F("Unknown"));
      return;
  }

  // Now we will try to open the 'volume'/'partition' - it should be FAT16 or FAT32
  if (!sdVolume.init(sdCard))
  {
    Serial.println(F("no FAT16/FAT32 partition."));
    return;
  }

  // print the type and size of the first FAT-type volume
  uint32_t volumesize;
  Serial.print(F("Volume type is FAT"));
  Serial.println(sdVolume.fatType(), DEC);

  volumesize = sdVolume.blocksPerCluster();    // clusters are collections of blocks
  volumesize *= sdVolume.clusterCount();       // we'll have a lot of clusters
  volumesize *= 512;                            // SD card blocks are always 512 bytes
  Serial.print(F("Volume size (bytes): "));
  Serial.println(volumesize);
  /*
  Serial.print("Volume size (Kbytes): ");
  volumesize /= 1024;
  Serial.println(volumesize);
  Serial.print("Volume size (Mbytes): ");
  volumesize /= 1024;
  Serial.println(volumesize);
  */

  Serial.println(F("*Files found on the card (name, date and size in bytes): "));
  sdRoot.openRoot(sdVolume);

  // list all files in the card with date and size
  sdRoot.ls(LS_R | LS_DATE | LS_SIZE);
}

#endif

//--------------------------------------------------------------------------

