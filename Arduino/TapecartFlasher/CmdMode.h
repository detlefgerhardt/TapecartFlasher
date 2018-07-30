//-------------------------------------------------------------------------------------------------
// CmdMode.h   *dg*    13.07.2018
//-------------------------------------------------------------------------------------------------

#ifndef CmdMode_h
#define CmdMode_h 

//-------------------------------------------------------------------------------------------------

#define TAPECART_CMDMODE_MAGIC 0xfce2

typedef enum
{
  CMD_EXIT = 0,
  CMD_READ_DEVICEINFO,
  CMD_READ_DEVICESIZES,
  CMD_READ_CAPABILITIES,

  CMD_READ_FLASH  = 0x10,
  CMD_READ_FLASH_FAST,
  CMD_WRITE_FLASH,
  CMD_WRITE_FLASH_FAST, // FIXME: Not Yet Implemented
  CMD_ERASE_FLASH_64K,
  CMD_ERASE_FLASH_BLOCK,
  CMD_CRC32_FLASH,

  CMD_READ_LOADER = 0x20,
  CMD_READ_LOADINFO,
  CMD_WRITE_LOADER,
  CMD_WRITE_LOADINFO,

  CMD_LED_OFF = 0x30,
  CMD_LED_ON,
  CMD_READ_DEBUGFLAGS,
  CMD_WRITE_DEBUGFLAGS,

  CMD_DIR_SETPARAMS = 0x40,
  CMD_DIR_LOOKUP,

  /* internal use only */
  CMD_RAMEXEC = 0xf0,
} command_t;

//-------------------------------------------------------------------------------------------------

class CmdMode
{
  public:
    CmdMode();
    bool set_cmdmode(void);
    bool start(void);
    void exit();
    void read_deviceinfo(char *buffer, int maxlen);
    void read_sizes(uint32_t *total_size, uint16_t *page_size, uint16_t *erase_pages);
    void read_flash(uint32_t offset, uint16_t len, uint8_t *buffer);
    void write_flash(uint32_t offset, uint16_t len, uint8_t *data);
    void erase_flash64K(uint32_t offset);
    void erase_flashblock(uint32_t offset);
    uint32_t crc32_flash(uint32_t offset, uint32_t len);
    void read_loader(uint8_t *buffer);
    void write_loader(const uint8_t *data);
    void read_loadinfo(uint16_t *offset, uint16_t *length, uint16_t *calladdr, char *filename);
    void write_loadinfo(const uint16_t offset, const uint16_t length, const uint16_t calladdr, const char *filename);
    void led_off();
    void led_on();
    uint16_t read_debugflags();
    void write_debugflags(uint16_t debug_flags);

    uint32_t total_size; // in bytes
    uint16_t page_size; // in bytes
    uint16_t erase_pages; // in pages
    uint16_t erase_size; // in bytes
    
    static uint32_t crc_buffer(uint8_t buffer, int len);
    static void dump_buffer(uint8_t *buffer, int len);

  private:
    void send_u24(uint32_t value);
    void send_u16(uint16_t value);
    void sendbyte(uint8_t data);
    uint32_t get_u24(void);
    uint16_t get_u16(void);
    uint8_t getbyte(void);
   
};

//-------------------------------------------------------------------------------------------------

#endif

//-------------------------------------------------------------------------------------------------


