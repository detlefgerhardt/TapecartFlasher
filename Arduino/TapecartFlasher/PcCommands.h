//-------------------------------------------------------------------------------------------------
// PcCommands.h   *dg*    13.07.2018
//-------------------------------------------------------------------------------------------------

#ifndef PcCommands_h
#define PcCommands_h 

//-------------------------------------------------------------------------------------------------

typedef enum
{
  CMDGROUP_ARDUINO = 0x01,
  CMDGROUP_TAPECART = 0x02,
};

typedef enum
{
  ARDUINOCMD_VERSION = 1,
  ARDUINOCMD_START_CMDMODE = 2,
  ARDUINOCMD_SD_DIR = 4,
  ARDUINOCMD_SD_WRITE = 5,
  ARDUINOCMD_SD_READ = 5,
};

typedef enum
{
  RESULT_OK = 0x00,
  RESULT_ERROR = 0x01,
  RESULT_NOT_IMPLEMENTED = 0x02,
  RESULT_CHKSUM_ERROR = 0x03,
};

#define CMD_PREFIX 0x01 // SOH
#define CMD_ENQ 0x05 // ENQ

//-------------------------------------------------------------------------------------------------

class PcCommands
{
  public:
    PcCommands();
    bool readCmd(void);

  private:
    void arduinoCommand(uint8_t cmd, uint16_t cmdLen);
    uint8_t arduinoCmd_Version(uint16_t *resultLen);
    uint8_t arduinoCmd_StartCmdMode();
    void tapecartCommand(uint8_t cmd, uint16_t cmdLen);
    uint8_t tapecartCmd_ReadDeviceInfo(uint16_t *resultLen);
    uint8_t tapecartCmd_ReadDeviceSizes(uint16_t *resultLen);
    uint8_t tapecartCmd_ReadFlash(uint16_t *resultLen);
    uint8_t tapecartCmd_WriteFlash(uint16_t *resultLen);
    uint8_t tapecartCmd_EraseFlash64K(uint16_t *resultLen);
    uint8_t tapecartCmd_EraseFlashBlock(uint16_t *resultLen);
    uint8_t tapecartCmd_Crc32Flash(uint16_t *resultLen);
    uint8_t tapecartCmd_ReadLoader(uint16_t *resultLen);
    uint8_t tapecartCmd_ReadLoadInfo(uint16_t *resultLen);
    uint8_t tapecartCmd_WriteLoader(uint16_t *resultLen);
    uint8_t tapecartCmd_WriteLoadInfo(uint16_t *resultLen);
    uint8_t tapecartCmd_LedOff(uint16_t *resultLen);
    uint8_t tapecartCmd_LedOn(uint16_t *resultLen);

    void sendResponse(uint8_t cmdGroup, uint8_t cmd, uint8_t result, uint16_t len);
    uint16_t recv_buffer(uint8_t *buffer, uint16_t length);
    uint16_t recv_u16();
    uint32_t recv_u24();
    uint32_t recv_u32();
    uint8_t recv_byte();
    void send_u16(uint16_t value);
    void send_u24(uint32_t value);
    void send_byte(uint8_t value);
    uint16_t fromBuffer_u16(uint8_t *buffer);
    uint32_t fromBuffer_u24(uint8_t *buffer);
    uint32_t fromBuffer_u32(uint8_t *buffer);
    void toBuffer_u16(uint8_t *buffer, uint16_t value);
    void toBuffer_u24(uint8_t *buffer, uint32_t value);
    void toBuffer_u32(uint8_t *buffer, uint32_t value);
};

//-------------------------------------------------------------------------------------------------

#endif

//-------------------------------------------------------------------------------------------------


