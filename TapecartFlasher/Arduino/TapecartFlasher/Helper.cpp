//-------------------------------------------------------------------------------------------------
// Helper.cpp   *dg*    22.10.2018
//-------------------------------------------------------------------------------------------------

#include <Arduino.h>
#include "TapecartFlasher.h"
#include "Helper.h"

//-------------------------------------------------------------------------------------------------

bool Helper::inputConsole(char *buffer, byte len)
{
  const char cursor = '_';
  const char bs = 0x08;
  byte pos = 0;
  while(true)
  {
    if (!Serial.available())
      continue;
    char ch = Serial.read();
    switch(ch)
    {
      case CMD_PREFIX: // API CMD: check cmd and exit console
        buffer[0] = CMD_PREFIX;
        return true;
      case 0x1B: // ESC
        buffer[0] = 0x1B;
        return true;
      case 0x0D:
        buffer[pos] = '\0';
        return true;
      case 0x08:
        if (pos>0)
        {
          pos--;
          Serial.print(F("\x08 \x08"));
        }
        break;
      default:
        if (ch>=32 && pos<len)
        {
          buffer[pos++] = ch;
          Serial.print(ch);
        }
        break;
    }
  }
}

//--------------------------------------------------------------------------

void Helper::dumpBuffer(const uint8_t *buffer, int len)
{
  int p = 0;
  char s[80];

  while(p<len)
  {
    sprintf(s, FCSTR("*%04x "), p);
    Serial.print(s);
    for (int j=0; j<16; j++)
    {
      sprintf(s, FCSTR("%02x "), buffer[p]);
      Serial.print(s);
      p++;
      if (p>=len)
        break;
    }
    Serial.println();
  }
} 

//-------------------------------------------------------------------------------------------------
// Helper.cpp
//-------------------------------------------------------------------------------------------------


