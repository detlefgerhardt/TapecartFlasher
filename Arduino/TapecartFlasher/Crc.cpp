//-------------------------------------------------------------------------------------------------
// Crc.cpp   *dg*    08.07.2018
//-------------------------------------------------------------------------------------------------

#include <Arduino.h>
#include "Crc.h"

//-------------------------------------------------------------------------------------------------
// constructor

/*
Crc::Crc()
{
}
*/

//--------------------------------------------------------------------------

uint32_t Crc::crc_init(void)
{
    return 0xffffffff;
}

//-------------------------------------------------------------------------------------------------

uint32_t Crc::crc_finalize(uint32_t crc)
{
    return crc_reflect(crc, 32) ^ 0xffffffff;
}  

//-------------------------------------------------------------------------------------------------

uint32_t Crc::crc_reflect(uint32_t data, size_t data_len)
{
    unsigned int i;
    uint32_t ret;

    ret = data & 0x01;
    for (i = 1; i < data_len; i++)
    {
        data >>= 1;
        ret = (ret << 1) | (data & 0x01);
    }
    return ret;
} 

//-------------------------------------------------------------------------------------------------

uint32_t Crc::crc_update(uint32_t crc, const uint8_t *data, size_t data_len)
{
    unsigned int i;
    bool bit;
    unsigned char c;

    while (data_len--)
    {
        c = *data++;
        for (i = 0x01; i & 0xff; i <<= 1)
        {
            bit = crc & 0x80000000;
            if (c & i)
              bit = !bit;
            crc <<= 1;
            if (bit)
              crc ^= 0x04c11db7;
        }
        crc &= 0xffffffff;
    }
    return crc & 0xffffffff;
}

//--------------------------------------------------------------------------

uint32_t Crc::crc_buffer(uint8_t *buffer, int len)
{
  uint32_t crc = Crc::crc_init();
  crc = Crc::crc_update(crc, buffer, len); 
  return Crc::crc_finalize(crc);
}

//-------------------------------------------------------------------------------------------------
// Crt.cpp
//-------------------------------------------------------------------------------------------------


