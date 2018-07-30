//-------------------------------------------------------------------------------------------------
// Tcrt.h   *dg*    13.07.2018
//-------------------------------------------------------------------------------------------------

#ifndef Tcrt_h
#define Tcrt_h 

//-------------------------------------------------------------------------------------------------

#define FILENAME_LENGTH 16
#define TCRT_HEADER_SIZE 16
#define TCRT_OFFSET_VERSION 16
#define TCRT_OFFSET_DATAADDR 18
#define TCRT_OFFSET_FILENAME 24
#define TCRT_OFFSET_FLAGS 40
#define TCRT_OFFSET_LOADER 41
#define TCRT_OFFSET_FLASHLENGTH 212
#define TCRT_OFFSET_FLASHDATA 216
#define TCRT_FLAG_LOADERPRESENT 1

#define TCRT_VERSION 1
#define LOADER_LENGTH 171


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
    static bool Tcrt::load_tcrt_file(char *fname);
    static bool write_tcrt_header(uint8_t *header, uint32_t total_size);

  private:
};

//-------------------------------------------------------------------------------------------------

#endif

//-------------------------------------------------------------------------------------------------


