//-------------------------------------------------------------------------------------------------
// CmdMode.cpp   *dg*    13.07.2018
//-------------------------------------------------------------------------------------------------

#include "TapecartFlasher.h"
#include "CmdMode.h"
#include "Tcrt.h"

//-------------------------------------------------------------------------------------------------
// constructor

CmdMode::CmdMode()
{
}

//--------------------------------------------------------------------------

bool CmdMode::start()
{
  if (!set_cmdmode())
    return false;

  read_sizes(&total_size, &page_size, &erase_pages);
  erase_size = page_size * erase_pages;

  return true;
}

//--------------------------------------------------------------------------

bool CmdMode::set_cmdmode()
{
  uint8_t i;
  long timeout;
  
  /* ensure the cart is in stream mode */
  fastDigitalWrite(PIN_WRITE, LOW);
  fastDigitalWrite(PIN_MOTOR, HIGH);
  delay(1);

  /* wait until sense is low */
  timeout = millis();
  while(fastDigitalRead(PIN_SENSE))
  {
    if (millis()-timeout>10)
    {
      Serial.println(F("*sense timeout 1"));
      return false;
    }
  }
  //int d = millis()-timeout;
  //Serial.println(d);

  /* send unlock sequence - needs ~70 ms */
  fastDigitalWrite(PIN_MOTOR, LOW);
  delay(1);

  //uint16_t cmdcode = TAPECART_CMDMODE_MAGIC;
  uint16_t cmdcode = 0xfce2;
  for (i = 0; i < 16; i++)
  {
    fastDigitalWrite(PIN_WRITE, (cmdcode & 0x8000) != 0);
    cmdcode <<= 1;

    delayMicroseconds(500);
    fastDigitalWrite(PIN_MOTOR, HIGH);
    delayMicroseconds(500);
    fastDigitalWrite(PIN_MOTOR, LOW);
  }

  delay(1);
  fastDigitalWrite(PIN_WRITE, HIGH);

  /* wait until sense is high */
  timeout = millis();
  while(!fastDigitalRead(PIN_SENSE))
  {
    if (millis()-timeout>50)
    {
      Serial.println(F("*sense timeout 2"));
      return false;
    }
  }

  /* wait for 100 pulses on read (0.384 ms periode) */
  delay(1);
  for (i = 0; i < 100; ++i)
  {
    timeout = millis();
    while(fastDigitalRead(PIN_READ))
    {
      if (millis()-timeout>100)
      {
        Serial.print("*read timeout HIGH ");
        Serial.print(i);
        Serial.print(" ");
        Serial.println(millis()-timeout);
        break;
      }
    }

    timeout = millis();
    while(!fastDigitalRead(PIN_READ))
    {
      if (millis()-timeout>100)
      {
        Serial.print(F("*read timeout LOW "));
        Serial.print(i);
        Serial.print(" ");
        Serial.println(millis()-timeout);
        break;
      }
    }
  }
  fastDigitalWrite(PIN_WRITE, LOW);
  return true;
} 

//--------------------------------------------------------------------------

void CmdMode::exit()
{
  sendbyte(CMD_EXIT);
}

//--------------------------------------------------------------------------

void CmdMode::read_deviceinfo(char *buffer, int maxlen)
{
  sendbyte(CMD_READ_DEVICEINFO);

  int cnt = 0;
  while(cnt < maxlen)
  {
    uint8_t data = getbyte();
    buffer[cnt++] = data;
    if (data==0)
      break;
  };
}

//--------------------------------------------------------------------------

void CmdMode::read_sizes(uint32_t *total_size, uint16_t *page_size, uint16_t *erase_pages)
{
  sendbyte(CMD_READ_DEVICESIZES);

  *total_size = get_u24();
  *page_size   = get_u16();
  *erase_pages = get_u16();
}

//--------------------------------------------------------------------------

void CmdMode::read_flash(uint32_t offset, uint16_t len, uint8_t *buffer)
{
  sendbyte(CMD_READ_FLASH);
  send_u24(offset);
  send_u16(len);

  for (uint16_t i=0; i<len; i++)
    buffer[i] = getbyte();
}

//--------------------------------------------------------------------------

void CmdMode::write_flash(uint32_t offset, uint16_t len, uint8_t *data)
{
  sendbyte(CMD_WRITE_FLASH);
  send_u24(offset);
  send_u16(len);

  for (int i = 0; i < len; i++)
    sendbyte(data[i]);
}

//--------------------------------------------------------------------------

void CmdMode::erase_flash64K(uint32_t offset)
{
  sendbyte(CMD_ERASE_FLASH_64K);
  send_u24(offset);
}

//--------------------------------------------------------------------------

void CmdMode::erase_flashblock(uint32_t offset)
{
  sendbyte(CMD_ERASE_FLASH_BLOCK);
  send_u24(offset);
}

//--------------------------------------------------------------------------

uint32_t CmdMode::crc32_flash(uint32_t offset, uint32_t len)
{
  sendbyte(CMD_CRC32_FLASH);
  send_u24(offset);
  send_u24(len);

  uint32_t crc  = get_u16();
  crc |= (uint32_t)get_u16() << 16;

  return crc;
}
 
//--------------------------------------------------------------------------

void CmdMode::read_loader(uint8_t *buffer)
{
  sendbyte(CMD_READ_LOADER);
  for (uint8_t i = 0; i < LOADER_LENGTH; ++i)
    *buffer++ = getbyte();
}

//--------------------------------------------------------------------------

void CmdMode::write_loader(const uint8_t *data)
{
  sendbyte(CMD_WRITE_LOADER);
  for (uint8_t i = 0; i < LOADER_LENGTH; ++i)
    sendbyte(data[i]);
}

//--------------------------------------------------------------------------

void CmdMode::read_loadinfo(uint16_t *offset, uint16_t *length, uint16_t *calladdr, char *filename)
{
  sendbyte(CMD_READ_LOADINFO);

  *offset = get_u16();
  *length = get_u16();
  *calladdr = get_u16();

  for (uint8_t i = 0; i < FILENAME_LENGTH; ++i)
    *filename++ = getbyte();
  *filename = '\0';
}

//--------------------------------------------------------------------------

void CmdMode::write_loadinfo(const uint16_t offset, const uint16_t length, const uint16_t calladdr, const char *filename)
{
  sendbyte(CMD_WRITE_LOADINFO);
  send_u16(offset);
  send_u16(length);
  send_u16(calladdr);

  for (uint8_t i = 0; i < FILENAME_LENGTH; ++i)
    sendbyte(filename[i]);
}
//--------------------------------------------------------------------------

void CmdMode::led_off()
{
  sendbyte(CMD_LED_OFF);
}

//--------------------------------------------------------------------------

void CmdMode::led_on()
{
  sendbyte(CMD_LED_ON);
} 

//--------------------------------------------------------------------------

uint16_t CmdMode::read_debugflags()
{
  sendbyte(CMD_READ_DEBUGFLAGS);
  return get_u16();
}

//--------------------------------------------------------------------------

void CmdMode::write_debugflags(uint16_t debug_flags)
{
  sendbyte(CMD_WRITE_DEBUGFLAGS);
  send_u16(debug_flags);
}

//--------------------------------------------------------------------------

void CmdMode::send_u24(uint32_t value)
{
  sendbyte( value & 0xff);
  sendbyte((value >> 8) & 0xff);
  sendbyte((value >> 16) & 0xff);
}

//--------------------------------------------------------------------------

void CmdMode::send_u16(uint16_t value)
{
  sendbyte(value & 0xff);
  sendbyte(value >> 8);
}

//--------------------------------------------------------------------------

uint32_t CmdMode::get_u24(void)
{
  uint32_t val;

  val  = getbyte();
  val |= (uint32_t)getbyte() << 8;
  val |= (uint32_t)getbyte() << 16;
  return val;
}

//--------------------------------------------------------------------------

uint16_t CmdMode::get_u16(void)
{
  uint16_t val;

  val  = getbyte();
  val |= (uint16_t)getbyte() << 8;

  return val;
}

//--------------------------------------------------------------------------

/* assumed initial state: write low */
void CmdMode::sendbyte(uint8_t data)
{
  uint8_t i;

  noInterrupts();
  
  /* wait until tapecart is ready (sense high) */
  while (!fastDigitalRead(PIN_SENSE)) ;

  /* switch sense to output */
  pinMode(PIN_SENSE, OUTPUT);

  /* send byte */
  for (i = 0; i < 8; i++)
  {
    fastDigitalWrite(PIN_SENSE, (data & 0x80) != 0);
    data <<= 1;

    delayMicroseconds(2);
    fastDigitalWrite(PIN_WRITE, HIGH);
    delayMicroseconds(3);
    fastDigitalWrite(PIN_WRITE, LOW);
    delayMicroseconds(3);
  }

  /* ensure known level on sense */
  fastDigitalWrite(PIN_SENSE, HIGH);
  delayMicroseconds(2);

  /* switch sense to input */
  /* (tapecart sets sense to output 10us after high->low edge) */
  fastDigitalWrite(PIN_SENSE, LOW);
  pinMode(PIN_SENSE, INPUT);
  delayMicroseconds(10);
  
  interrupts();
}

//--------------------------------------------------------------------------

/* assumed initial state: write output low, sense input */
uint8_t CmdMode::getbyte(void)
{
  noInterrupts();
  
  /* wait until tapecart is ready (sense high) */
  while (!fastDigitalRead(PIN_SENSE)) ;

  /* set write high to start the transmission on the tapecart side */
  fastDigitalWrite(PIN_WRITE, HIGH);
  delayMicroseconds(2); // !!! 10

  /* read byte */
  uint8_t data;
  for (uint8_t i = 0; i < 8; i++)
  {
    fastDigitalWrite(PIN_WRITE, LOW);
    delayMicroseconds(3);
    fastDigitalWrite(PIN_WRITE, HIGH);
    delayMicroseconds(2);

    data <<= 1;
    if (fastDigitalRead(PIN_SENSE))
      data |= 1;
  }

  /* return write to low */
  fastDigitalWrite(PIN_WRITE, LOW);
  delayMicroseconds(2);

  interrupts();

  return data;
}

//--------------------------------------------------------------------------

void CmdMode::dump_buffer(uint8_t *buffer, int len)
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
      if (p>len)
        break;
    }
    Serial.println();
  }
} 

//-------------------------------------------------------------------------------------------------
// CmdMode.cpp
//-------------------------------------------------------------------------------------------------


