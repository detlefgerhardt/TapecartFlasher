//-------------------------------------------------------------------------------------------------
// Console.cpp   *dg*    22.10.2018
//-------------------------------------------------------------------------------------------------

#include "TapecartFlasher.h"
#include "Tcrt.h"
#include "CmdMode.h"
//#include "PcCommands.h"
#include "Console.h"
#include "Crc.h"
#include "Helper.h"
#include <SdFat.h>
#include <spi.h>

extern CmdMode tc_cmd;
extern SdFat sd;

extern uint8_t databuffer[];
extern char printbuffer[];
extern bool sdCardOk;

//-------------------------------------------------------------------------------------------------
// constructor

Console::Console()
{
}

//-------------------------------------------------------------------------------------------------

byte Console::start()
{
  arduinoVersion();
  Serial.println();

  while(true)
  {
    showMenu();
    if (!readMenu(databuffer))
      continue;
    Serial.println("");
    
    switch(toupper(databuffer[0]))
    {
      case CMD_PREFIX:
        return CMD_PREFIX;  // API command, exit console
      case 0x1B: // ESC
        return 0;
      case 'I':
        if (checkTapecart())
          deviceInit();
        break;
      case 'F':
        if (checkSdCard() && checkTapecart())
          sdDir();
        break;
      case 'W':
        if (checkSdCard() && checkTapecart())
        {
          if (!readFilename(databuffer))
            break;
          if (databuffer[0] == CMD_PREFIX)
            return CMD_PREFIX; // API command, exit console
          Serial.print(F("\r\nwriting '"));
          Serial.print((char*)databuffer);
          Serial.println(F("' to Tapecart"));
          Tcrt::write_tcrt_file(databuffer, LOADER_MODE_TCRT, true);
        }
        break;
      case 'R':
        if (checkSdCard() && checkTapecart())
        {
          if (!readFilename(databuffer))
            break;
          if (databuffer[0] == CMD_PREFIX)
            return CMD_PREFIX; // API command, exit console
          Serial.print(F("\r\nreading Tapecart to '"));
          Serial.print((char*)databuffer);
          Serial.println('\'');
          Tcrt::read_tcrt_file(databuffer, LOADER_MODE_TCRT);
        }
        break;
      case 'V':
        arduinoVersion();
        checkSdCard();
        checkTapecart();
        break;
    }
    Serial.println();
  };
}

//-------------------------------------------------------------------------------------------------

void Console::showMenu()
{
  Serial.print(F("(I)nit,(F)iles,(W)rite,(R)ead,(V)ersion"));
}

//-------------------------------------------------------------------------------------------------

void Console::deviceInit()
{
  if (tc_cmd.start())
  {
    tc_cmd.read_deviceinfo(databuffer, 32);
    Serial.println(F("Tapecart info:"));
    Serial.print(F("name="));
    byte p=0;
    while(databuffer[p])
    {
      Serial.print((char)databuffer[p]);
      p++;
    }
    sprintf(printbuffer, FCSTR("name=%lxh, pagesize=%xh, erasepages=%xh"), tc_cmd.total_size, tc_cmd.page_size, tc_cmd.erase_pages);
    Serial.println(printbuffer);
  }
  else
  {
    Serial.println(F("error setting command mode"));
  }

  //SdSpiLibDriver sdSpi;
  //sdSpi.begin(PIN_SD_SEL);
  //SPI.end();
  if (!sd.begin(PIN_SD_SEL))
  {
    //sd.initErrorHalt();
    Serial.println(F("SD card error"));
  }

  /*
  Serial.print(F(", TotalSize="));
  Serial.print(tc_cmd.total_size, HEX);
  Serial.print(F("h, PageSize="));
  Serial.print(tc_cmd.page_size, HEX);
  Serial.print(F("h, ErasePages="));
  Serial.print(tc_cmd.erase_pages, HEX);
  Serial.println('h');
  */
  //Name= V1.0.0 W25QFLASH, TotalSize=200000h, PageSize=100h, ErasePages=20h
}

//-------------------------------------------------------------------------------------------------

void Console::sdDir()
{
  sd.ls("/", LS_DATE | LS_SIZE);
  /*  
  File dir = sd.open("/");
  while (true)
  {
    File entry = dir.openNextFile();
    if (!entry)
      return;
    Serial.print(entry.name());
    Serial.print(' ');
    Serial.println(entry.size());
    entry.close();
  };
  dir.close();  
  */
}

//-------------------------------------------------------------------------------------------------

bool Console::checkTapecart()
{
  if (!tc_cmd.cmd_mode_active)
  {
    Serial.println(F("tapecart error"));
    return false;
  }
  else
    return true;
}

//-------------------------------------------------------------------------------------------------

bool Console::checkSdCard()
{
  if (!sdCardOk)
  {
    Serial.println(F("no sd-card detected"));
    return false;
  }
  else
    return true;
}

//-------------------------------------------------------------------------------------------------

bool Console::readMenu(char *buffer)
{
  Serial.print(": ");
  bool result = Helper::inputConsole(buffer, 1);
  Serial.println();
  return strlen(buffer)>0 ? result : false;
}

//-------------------------------------------------------------------------------------------------

bool Console::readFilename(char *buffer)
{
  Serial.print(F("filename: "));
  bool result = Helper::inputConsole(buffer, 30);
  Serial.println();
  return strlen(buffer)>0 ? result : false;
}

//-------------------------------------------------------------------------------------------------

void Console::arduinoVersion()
{
  sprintf(printbuffer, FCSTR("Arduino TapecartFlasher V%d.%d/%d"), MAJOR_VERSION, MINOR_VERSION, API_VERSION);
  Serial.println(printbuffer);

  /*
  Serial.print(MAJOR_VERSION);
  Serial.print('.');
  Serial.print(MINOR_VERSION);
  Serial.print('/');
  Serial.println(API_VERSION);
  */
}

//-------------------------------------------------------------------------------------------------
// Console.cpp
//-------------------------------------------------------------------------------------------------

