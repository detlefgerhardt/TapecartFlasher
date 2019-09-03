//-------------------------------------------------------------------------------------------------
// PcCommands.cpp   *dg*    11.10.2018
//-------------------------------------------------------------------------------------------------
//
// Arduino USB/Serial API format:
// command: 0x01 <cmdgroup:8> <cmd:8> <datalen:16> <data 0 ... data n> <chksum:8> (binary format)
// result: 0x01 <cmdgroup:8> <cmd:8> <status:8> <datalen:16> <data 0 ... data n> <chksum:8>
// 1-byte checksum is simple XOR
// a result starting with a "*" is a debug message terminated by 0x10 (new line)

#include "TapecartFlasher.h"
#include "Tcrt.h"
#include "CmdMode.h"
#include "PcCommands.h"
#include "Crc.h"

extern CmdMode tc_cmd;
extern uint8_t databuffer[];

//-------------------------------------------------------------------------------------------------
// constructor

PcCommands::PcCommands()
{
}

//-------------------------------------------------------------------------------------------------

bool PcCommands::readCmd()
{
  uint8_t calcChksum = 0;

  byte cmdGroup = recv_byte();
  calcChksum ^= cmdGroup;
  byte cmd = recv_byte();
  calcChksum ^= cmd;

  uint16_t cmdLen = recv_u16();
  calcChksum ^= (cmdLen & 0xFF);
  calcChksum ^= ((cmdLen>>8) & 0xFF);
  //Serial.print(F("*len="));
  //Serial.println(cmdLen);

  if (cmdLen>DATABUFFER_MAX + DATABUFFER_HEADER)
    return false; // length error

  uint16_t len;
  if (cmdLen<32)
  {
    len = Serial.readBytes(databuffer, cmdLen);
  }
  else
  {
    fastDigitalWrite(PIN_LED, true);
    len = recv_buffer(databuffer, cmdLen);
  }

  for (uint16_t i=0; i<cmdLen; i++)
    calcChksum ^= databuffer[i];

  uint8_t chksum = recv_byte();
  if (chksum != calcChksum)
  {
    sendResponse(cmdGroup, cmd, RESULT_CHKSUM_ERROR, 0);
    return false; // checksum error
  }

  switch(cmdGroup)
  {
    case CMDGROUP_ARDUINO:
      arduinoCommand(cmd, cmdLen);
      break;
    case CMDGROUP_TAPECART:
      tapecartCommand(cmd, cmdLen);
  }
}

//-------------------------------------------------------------------------------------------------

void PcCommands::arduinoCommand(uint8_t cmd, uint16_t cmdLen)
{
  uint8_t result = RESULT_NOT_IMPLEMENTED;
  uint16_t resultLen = 0;

  switch(cmd)
  {
    case ARDUINOCMD_VERSION:
      result = arduinoCmd_Version(&resultLen);
      break;
    case ARDUINOCMD_START_CMDMODE:
      result = arduinoCmd_StartCmdMode();
      break;
  }

  sendResponse(CMDGROUP_ARDUINO, cmd, result, resultLen);
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::arduinoCmd_Version(uint16_t *resultLen)
{
  // arduino sketch version 0.1
  databuffer[0] = MINOR_VERSION;
  databuffer[1] = MAJOR_VERSION;
  // api version
  databuffer[2] = API_VERSION;
  databuffer[3] = ARDUINO_TYPE;
  *resultLen = 4;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::arduinoCmd_StartCmdMode()
{
  uint8_t result = RESULT_OK;
  if (tc_cmd.start())
    return RESULT_OK;

  Serial.println(F("*error starting tapecart command mode"));
  return RESULT_ERROR;
}

//-------------------------------------------------------------------------------------------------

#if false
void PcCommands::SdDir()
{
  File dir = SD.open("/");
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
}
#endif

//-------------------------------------------------------------------------------------------------

void PcCommands::tapecartCommand(uint8_t cmd, uint16_t cmdLen)
{
  uint8_t result = RESULT_NOT_IMPLEMENTED;
  uint16_t resultLen = 0;
  
  if (!tc_cmd.cmd_mode_active)
  {
    sendResponse(CMDGROUP_TAPECART, cmd, RESULT_TAPECART_ERROR, 0);
    return;
  }

  switch(cmd)
  {
    case CMD_EXIT:
      tc_cmd.exit();
      result = RESULT_OK;
      break;
    case CMD_READ_DEVICEINFO:
      result = tapecartCmd_ReadDeviceInfo(&resultLen);
      break;
    case CMD_READ_DEVICESIZES:
      result = tapecartCmd_ReadDeviceSizes(&resultLen);
      break;
    case CMD_READ_CAPABILITIES:
      break;
    case CMD_READ_FLASH:
      result = tapecartCmd_ReadFlash(&resultLen);
      break;
    case CMD_READ_FLASH_FAST:
      break;
    case CMD_WRITE_FLASH:
      result = tapecartCmd_WriteFlash(&resultLen);
      break;
    case CMD_ERASE_FLASH_64K:
      result = tapecartCmd_EraseFlash64K(&resultLen);
      break;
    case CMD_ERASE_FLASH_BLOCK:
      result = tapecartCmd_EraseFlashBlock(&resultLen);
      break;
    case CMD_CRC32_FLASH:
      result = tapecartCmd_Crc32Flash(&resultLen);
      break;
    case CMD_READ_LOADER:
      result = tapecartCmd_ReadLoader(&resultLen);
      break;
    case CMD_READ_LOADINFO:
      result = tapecartCmd_ReadLoadInfo(&resultLen);
      break;
    case CMD_WRITE_LOADER:
      result = tapecartCmd_WriteLoader(&resultLen);
      break;
    case CMD_WRITE_LOADINFO:
      result = tapecartCmd_WriteLoadInfo(&resultLen);
      break;
    case CMD_LED_OFF:
      result = tapecartCmd_LedOff(&resultLen);
      break;
    case CMD_LED_ON:
      result = tapecartCmd_LedOn(&resultLen);
      break;
    case CMD_READ_DEBUGFLAGS:
      break;
    case CMD_WRITE_DEBUGFLAGS:
      break;
    case CMD_DIR_SETPARAMS:
      break;
    case CMD_DIR_LOOKUP:
      break;
  }

  if (result==RESULT_NOT_IMPLEMENTED)
  {
    Serial.print(F("*not implemented "));
    Serial.println(cmd, HEX);
  }

  sendResponse(CMDGROUP_TAPECART, cmd, result, resultLen);
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_ReadDeviceInfo(uint16_t *resultLen)
{
  tc_cmd.read_deviceinfo(databuffer, 32);
  *resultLen = strlen(databuffer);
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_ReadDeviceSizes(uint16_t *resultLen)
{
  toBuffer_u24(databuffer, tc_cmd.total_size);
  toBuffer_u16(databuffer+3, tc_cmd.page_size);
  toBuffer_u16(databuffer+5, tc_cmd.erase_pages);
  *resultLen = 7;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_ReadFlash(uint16_t *resultLen)
{
  uint32_t offset = fromBuffer_u24(databuffer);
  uint16_t len = fromBuffer_u16(databuffer+3);
  /*
  Serial.print(F("*read="));
  Serial.print(offset, HEX);
  Serial.print(",");
  Serial.println(len, HEX);
  */
  tc_cmd.read_flash(offset, len, databuffer);
  *resultLen = len;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_WriteFlash(uint16_t *resultLen)
{
  uint32_t offset = fromBuffer_u24(databuffer);
  uint16_t len = fromBuffer_u16(databuffer+3);
  /*
  //uint32_t crc = Crc::crc_buffer(databuffer+5, len);
  Serial.print(F("*write="));
  Serial.print(offset, HEX);
  Serial.print(",");
  Serial.println(len, HEX);
  //Serial.print(",");
  //Serial.println(crc, HEX);
  //CmdMode::dump_buffer(databuffer+5, len);
  */
  tc_cmd.write_flash(offset, len, databuffer+5);
  *resultLen = 0;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_EraseFlash64K(uint16_t *resultLen)
{
  uint32_t offset = fromBuffer_u24(databuffer);
  /*
  Serial.print(F("*erase64k="));
  Serial.println(offset, HEX);
  */
  tc_cmd.erase_flash64K(offset);
  *resultLen = 0;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_EraseFlashBlock(uint16_t *resultLen)
{
  uint32_t offset = fromBuffer_u24(databuffer);
  /*
  Serial.print(F("*erase="));
  Serial.println(offset, HEX);
  */
  tc_cmd.erase_flashblock(offset);
  *resultLen = 0;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_Crc32Flash(uint16_t *resultLen)
{
  uint32_t offset = fromBuffer_u24(databuffer);
  uint32_t len = fromBuffer_u24(databuffer+3);
  uint32_t crc = tc_cmd.crc32_flash(offset, len);
  /*
  Serial.print(F("*crc "));
  Serial.print(offset, HEX);
  Serial.print(" ");
  Serial.print(len, HEX);
  Serial.print(" ");
  Serial.println(crc, HEX);
  */
  toBuffer_u32(databuffer, crc);
  *resultLen = 4;
  return RESULT_OK;
}


//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_ReadLoader(uint16_t *resultLen)
{
  tc_cmd.read_loader(databuffer);
  *resultLen = LOADER_LENGTH;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_ReadLoadInfo(uint16_t *resultLen)
{
  uint16_t offset;
  uint16_t length;
  uint16_t calladdr;
  tc_cmd.read_loadinfo(&offset, &length, &calladdr, databuffer+6);
  toBuffer_u16(databuffer, offset);
  toBuffer_u16(databuffer+2, length);
  toBuffer_u16(databuffer+4, calladdr);
  tc_cmd.read_loadinfo(offset, length, calladdr, databuffer+6);
  *resultLen = 6+16;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_WriteLoader(uint16_t *resultLen)
{
  tc_cmd.write_loader(databuffer);
  *resultLen = 0;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_WriteLoadInfo(uint16_t *resultLen)
{
  uint16_t offset = fromBuffer_u16(databuffer);
  uint16_t length = fromBuffer_u16(databuffer+2);
  uint16_t calladdr = fromBuffer_u16(databuffer+4);
  tc_cmd.write_loadinfo(offset, length, calladdr, databuffer+6);
  *resultLen = 0;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_LedOff(uint16_t *resultLen)
{
  tc_cmd.led_off();
  *resultLen = 0;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::tapecartCmd_LedOn(uint16_t *resultLen)
{
  tc_cmd.led_on();
  *resultLen = 0;
  return RESULT_OK;
}

//-------------------------------------------------------------------------------------------------

void PcCommands::sendResponse(uint8_t cmdGroup, uint8_t cmd, uint8_t result, uint16_t len)
{
  uint8_t chksum = 0;
  
  send_byte(CMD_PREFIX);
  send_byte(cmdGroup);
  chksum ^= cmdGroup;
  send_byte(cmd);
  chksum ^= cmd;
  send_byte(result);
  chksum ^= result;
  send_u16(len);  // length = 0
  chksum ^= len & 0xff;
  chksum ^= (len>>8) & 0xff;
  for (uint16_t i=0; i<len; i++)
  {
    Serial.write(databuffer[i]);
    chksum ^= databuffer[i];
  }
  send_byte(chksum);
}

//-------------------------------------------------------------------------------------------------

uint16_t PcCommands::recv_buffer(uint8_t *buffer, uint16_t length)
{
  uint16_t len;
  uint16_t pos = 0;
  while(pos<length)
  {
    send_byte(CMD_ENQ); // ack
    len = length-pos;
    if (len>32)
      len = 32;
    Serial.readBytes(buffer+pos, len);
    pos += len;
  }
}

//-------------------------------------------------------------------------------------------------

uint16_t PcCommands::recv_u16()
{

  uint16_t value = recv_byte();
  value += ((uint16_t)recv_byte() << 8);
  return value;
}

//-------------------------------------------------------------------------------------------------

uint32_t PcCommands::recv_u24()
{
  uint32_t value = recv_byte();
  value += (recv_byte() << 8);
  value += (recv_byte() << 16);
  return value;
}

//-------------------------------------------------------------------------------------------------

uint32_t PcCommands::recv_u32()
{
  uint32_t value = recv_byte();
  value += (recv_byte() << 8);
  value += (recv_byte() << 16);
  value += (recv_byte() << 24);
  return value;
}

//-------------------------------------------------------------------------------------------------

uint8_t PcCommands::recv_byte()
{
  uint8_t data = 0xFF;
  Serial.readBytes(&data, 1);
  return data;;
}

//-------------------------------------------------------------------------------------------------

void PcCommands::send_u16(uint16_t value)
{
  send_byte( value & 0xff);
  send_byte((value >> 8) & 0xff);
}

//-------------------------------------------------------------------------------------------------

void PcCommands::send_u24(uint32_t value)
{
  send_byte( value & 0xff);
  send_byte((value >> 8) & 0xff);
  send_byte((value >> 16) & 0xff);
}

//-------------------------------------------------------------------------------------------------

void PcCommands::send_byte(uint8_t value)
{
  Serial.write(value);
}

//-------------------------------------------------------------------------------------------------

uint16_t PcCommands::fromBuffer_u16(uint8_t *buffer)
{
  return *buffer + (*(buffer+1)<<8);
}

//-------------------------------------------------------------------------------------------------

uint32_t PcCommands::fromBuffer_u24(uint8_t *buffer)
{
  return *buffer + ((uint32_t)*(buffer+1)<<8) + ((uint32_t)*(buffer+2)<<16);
}

//-------------------------------------------------------------------------------------------------

/*
uint32_t PcCommands::fromBuffer_u32(uint8_t *buffer)
{
  return *buffer + ((uint32_t)*(buffer+1)<<8) + ((uint32_t)*(buffer+2)<<16) + ((uint32_t)*(buffer+3)<<24);
}
*/

//-------------------------------------------------------------------------------------------------

void PcCommands::toBuffer_u16(uint8_t *buffer, uint16_t value)
{
  *buffer = value & 0xFF;
  *(buffer+1) = (value>>8) & 0xFF;
}

//-------------------------------------------------------------------------------------------------

void PcCommands::toBuffer_u24(uint8_t *buffer, uint32_t value)
{
  *buffer = value & 0xFF;
  *(buffer+1) = (value>>8) & 0xFF;
  *(buffer+2) = (value>>16) & 0xFF;
}

//-------------------------------------------------------------------------------------------------

void PcCommands::toBuffer_u32(uint8_t *buffer, uint32_t value)
{
  *buffer = value & 0xFF;
  *(buffer+1) = (value>>8) & 0xFF;
  *(buffer+2) = (value>>16) & 0xFF;
  *(buffer+3) = (value>>24) & 0xFF;
}

//-------------------------------------------------------------------------------------------------
// PcCommands.cpp
//-------------------------------------------------------------------------------------------------

