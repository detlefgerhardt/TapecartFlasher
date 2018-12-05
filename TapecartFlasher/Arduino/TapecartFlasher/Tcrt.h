//-------------------------------------------------------------------------------------------------
// Tcrt.h   *dg*    29.08.2018
//-------------------------------------------------------------------------------------------------

#ifndef Tcrt_h
#define Tcrt_h 

//-------------------------------------------------------------------------------------------------

#define MAX_FLASH_SIZE 0x200000
#define FILENAME_LENGTH 16
#define TCRT_SIG_SIZE 16
#define TCRT_OFFSET_VERSION 16
#define TCRT_OFFSET_LOADER_CALLADDR 0x12
#define TCRT_OFFSET_LOADER_NAME 0x18
#define TCRT_OFFSET_FLAGS 0x28
#define TCRT_OFFSET_LOADER 0x29
#define TCRT_OFFSET_FLASHSIZE 0xD4
#define TCRT_OFFSET_FLASHDATA 0xD8
#define TCRT_HEADERSIZE 0xD8
#define TCRT_FLAG_LOADERPRESENT 1

#define TCRT_VERSION 1
#define LOADER_LENGTH 171

// use default loader (read and write)
#define LOADER_MODE_DEFAULT 1
// keep loader from TCRT file (read and write)
#define LOADER_MODE_TCRT 2
// do not include any loader (read only)
#define LOADER_MODE_NONE 3

static struct
{
  uint16_t dataofs;
  uint16_t datalen;
  uint16_t calladdr;
} loadinfo; 

//-------------------------------------------------------------------------------------------------

class Tcrt
{
  public:
    Tcrt(void);
    static bool write_tcrt_file(char *fname, byte headerMode, bool useCrc);
    static bool read_tcrt_file(char *fname, byte headerMode);

  private:
    static bool write_tcrt_header(const uint8_t *header, uint32_t total_size, uint32_t *tcrtSize, byte headerMode);
};

//-------------------------------------------------------------------------------------------------

#endif

//-------------------------------------------------------------------------------------------------
// Tcrt.h
//-------------------------------------------------------------------------------------------------


