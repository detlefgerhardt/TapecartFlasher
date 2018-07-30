//-------------------------------------------------------------------------------------------------
// Tcrt.cpp   *dg*    18.07.2018
//-------------------------------------------------------------------------------------------------

#include "TapecartFlasher.h"
#include "Tcrt.h"
#include "CmdMode.h"
#include "Crc.h"

uint8_t loaderbuffer[LOADER_LENGTH];

const uint8_t tcrt_header[] =
{
  0x74, 0x61, 0x70, 0x65, 0x63, 0x61, 0x72, 0x74,
  0x49, 0x6d, 0x61, 0x67, 0x65, 0x0d, 0x0a, 0x1a
};

const PROGMEM uint8_t loader[] =
{
  0x6e,0x11,0xd0,0x78,0x18,0xa0,0x10,0xa2,0x00,0x86,0xc6,0xa9,0x27,0x2e,0xf3,0x03,
  0x90,0x02,0x09,0x08,0x85,0x01,0xca,0xea,0xd0,0xfc,0x29,0xdf,0x85,0x01,0xca,0xd0,
  0xfd,0x88,0xd0,0xe7,0xa9,0x30,0x85,0x01,0xa0,0xfa,0x20,0xb2,0x03,0x99,0xb0,0xff,
  0xc8,0xd0,0xf7,0x20,0xb2,0x03,0x91,0xae,0xe6,0xae,0xd0,0x02,0xe6,0xaf,0xa5,0xae,
  0xc5,0xac,0xd0,0xef,0xa6,0xaf,0xe4,0xad,0xd0,0xe9,0xa0,0x37,0x84,0x01,0x38,0x2e,
  0x11,0xd0,0x86,0x2e,0x86,0x30,0x85,0x2d,0x85,0x2f,0x58,0x20,0x53,0xe4,0x6c,0xaa,
  0x00,0xa9,0x10,0x24,0x01,0xd0,0xfc,0xa2,0x38,0xa9,0x27,0x86,0x01,0x85,0x00,0xea,
  0xa5,0x01,0x29,0x18,0x4a,0x4a,0x45,0x01,0x4a,0x29,0x0f,0xaa,0xa5,0x01,0x29,0x18,
  0x4a,0x4a,0x45,0x01,0x4a,0x29,0x0f,0x1d,0xe3,0x03,0xa2,0x2f,0x86,0x00,0xe8,0x86,
  0x01,0x60,0x00,0x10,0x20,0x30,0x40,0x50,0x60,0x70,0x80,0x90,0xa0,0xb0,0xc0,0xd0,
  0xe0,0xf0,0xca,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
};

extern CmdMode tc_cmd;
extern uint8_t databuffer[];

char printbuffer[80];

#define USE_CRC

//-------------------------------------------------------------------------------------------------
// constructor

Tcrt::Tcrt()
{
}

//--------------------------------------------------------------------------

bool Tcrt::load_tcrt_file(char *fname)
{
  File tcrtFile;

  tcrtFile = SD.open(fname, FILE_READ);
  if (tcrtFile == NULL)
  {
    Serial.print(F("File not found "));
    Serial.println(fname);
    return false;
  }

  long bytesread = tcrtFile.read(databuffer, TCRT_OFFSET_FLASHDATA);
  if (bytesread != TCRT_OFFSET_FLASHDATA)
  {
    Serial.println(F("error reading TCRT header"));
    return false;
  }
  
  if (!Tcrt::write_tcrt_header(databuffer, tc_cmd.total_size))
  {
    Serial.println(F("TCRT header error"));
    return false;
  }

  Serial.println(F("TCRT header ok"));

#ifdef USE_CRC
  uint32_t crc = Crc::crc_init();
#endif

  uint32_t offset = 0;
  long start_time = millis();
  bool led = false;
  while(offset < tc_cmd.total_size)
  {
    if (offset % tc_cmd.erase_size == 0)
      tc_cmd.erase_flashblock(offset);
    
    long bytesread = tcrtFile.read(databuffer, tc_cmd.page_size);
    if (bytesread==0)
      break;
#ifdef USE_CRC
    crc = Crc::crc_update(crc, databuffer, bytesread); 

    /*
    uint32_t crc2 = Crc::crc_init();
    crc2 = Crc::crc_update(crc2, databuffer, bytesread); 
    crc2 = Crc::crc_finalize(crc2);
    uint32_t crc3 =  tc_cmd.crc32_flash(offset, bytesread);
    if (crc2!=crc3)
    {
      Serial.print(F("crc err offset="));
      Serial.print(offset, HEX);
      Serial.print(F(" calc="));
      Serial.print(crc2, HEX);
      Serial.print(F(" flash="));
      Serial.println(crc3, HEX);
      while (Serial.read()!='#');
    }
    */
#endif

    if (offset % 0x4000 == 0)
    {
      sprintf(printbuffer, FCSTR("write %06lx"), offset);
      Serial.println(printbuffer);
      led = !led;
      if (led)
        tc_cmd.led_on();
      else
        tc_cmd.led_off();
    }

    tc_cmd.write_flash(offset, bytesread, databuffer);
    offset += tc_cmd.page_size;
  }
 
  while(offset < tc_cmd.total_size)
  {
    if (offset % tc_cmd.erase_size == 0)
      tc_cmd.erase_flashblock(offset);
    if (offset % 0x4000 == 0)
    {
      sprintf(printbuffer, FCSTR("erase %06lx"), offset);
      Serial.println(printbuffer);
      led = !led;
      if (led)
        tc_cmd.led_on();
      else
        tc_cmd.led_off();
    }
    offset += tc_cmd.page_size;
  }
  tc_cmd.led_on();

  sprintf(printbuffer, FCSTR("offset=%06lx"), offset);
  Serial.println(printbuffer);

  Serial.print(F("time="));
  Serial.println((millis()-start_time)/1000);

  start_time = millis();

#ifdef USE_CRC
  crc = Crc::crc_finalize(crc);
  uint32_t crc_flash =  tc_cmd.crc32_flash(0x000000, offset);
  sprintf(printbuffer, FCSTR("crc %06lx %06lx"), crc, crc_flash);
  Serial.println(printbuffer);
  if (crc != crc_flash)
    return false;
#endif

  Serial.print(F("time="));
  Serial.println((millis()-start_time)/1000);

  tcrtFile.close();
}

//-------------------------------------------------------------------------------------------------

bool Tcrt::write_tcrt_header(uint8_t *header, uint32_t total_size)
{
  uint32_t flashsize;

  // sanity-check TCRT header
  if (memcmp(databuffer, tcrt_header, TCRT_HEADER_SIZE))
  {
    Serial.println(F("invalid TCRT header"));
    return false;
  }
  if (databuffer[TCRT_OFFSET_VERSION] != TCRT_VERSION || databuffer[TCRT_OFFSET_VERSION + 1] != 0)
  {
    Serial.println(F("unknown TCRT version"));
    return false;
  }
  
  memcpy(&flashsize, databuffer + TCRT_OFFSET_FLASHLENGTH, 4);
  if (flashsize > total_size)
  {
    Serial.println(F("TCRT size exceeds carttridge size"));
    return false;
  }

  /* copy loader into place if not present */
  if (!(databuffer[TCRT_OFFSET_FLAGS] & TCRT_FLAG_LOADERPRESENT))
  { // loader not present, load it from file into header
    Serial.println(F("loader not present: use default loader"));
    File lf = SD.open("loader.bin", FILE_READ);
    lf.read(databuffer + TCRT_OFFSET_LOADER, LOADER_LENGTH);
    lf.close();
  }

  /* avoid writing loader if it's identical to the one already in cart */
  tc_cmd.read_loader(loaderbuffer);
  if (memcmp(databuffer + TCRT_OFFSET_LOADER, loaderbuffer, LOADER_LENGTH))
  {
    Serial.println(F("write new loader"));
    tc_cmd.write_loader(databuffer + TCRT_OFFSET_LOADER);
  }

  memcpy(&loadinfo, databuffer + TCRT_OFFSET_DATAADDR, sizeof(loadinfo));
  tc_cmd.write_loadinfo(loadinfo.dataofs, loadinfo.datalen, loadinfo.calladdr, databuffer + TCRT_OFFSET_FILENAME);
  
  return true;
}

//-------------------------------------------------------------------------------------------------
// Tcrt.cpp
//-------------------------------------------------------------------------------------------------


