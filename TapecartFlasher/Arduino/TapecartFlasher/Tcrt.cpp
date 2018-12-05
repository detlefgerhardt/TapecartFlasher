//-------------------------------------------------------------------------------------------------
// Tcrt.cpp   *dg*    11.10.2018
//-------------------------------------------------------------------------------------------------

#include "TapecartFlasher.h"
#include "Tcrt.h"
#include "CmdMode.h"
#include "Crc.h"
#include "Helper.h"
#include <SdFat.h>

// this is only used to compare the Tapecart header (171 bytes!)
uint8_t loaderbuffer[LOADER_LENGTH];

// TCRT signatur
const PROGMEM uint8_t tcrt_signatur[] =
{
  0x74, 0x61, 0x70, 0x65, 0x63, 0x61, 0x72, 0x74,
  0x49, 0x6d, 0x61, 0x67, 0x65, 0x0d, 0x0a, 0x1a
};

// default loader
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
extern SdFat sd;

extern uint8_t databuffer[];
extern char printbuffer[];

//-------------------------------------------------------------------------------------------------
// constructor

Tcrt::Tcrt()
{
}

//--------------------------------------------------------------------------
// write TCRT file from SD card to Tapecart

bool Tcrt::write_tcrt_file(char *fname, byte loaderMode, bool useCrc)
{
  File tcrtFile;

  tcrtFile = sd.open(fname, FILE_READ);
  if (tcrtFile == NULL)
  {
    Serial.print(F("error reading '"));
    Serial.print(fname);
    Serial.println('\'');
    return false;
  }

  long bytesread = tcrtFile.read(databuffer, TCRT_HEADERSIZE);
  if (bytesread != TCRT_OFFSET_FLASHDATA)
  {
    Serial.println(F("error reading TCRT header"));
    return false;
  }

  uint32_t tcrtSize;
  if (!write_tcrt_header(databuffer, tc_cmd.total_size, &tcrtSize, loaderMode))
  {
    //Serial.println(F("TCRT header error"));
    return false;
  }

  //Serial.println(tcrtSize);
  //Serial.println(F("TCRT header ok"));

  uint32_t crc;
  if (useCrc) 
    crc = Crc::crc_init();

  uint32_t offset = 0;
  long start_time = millis();
  bool led = false;
  while(offset < tc_cmd.total_size)
  {
    if (offset % tc_cmd.erase_size == 0)
      tc_cmd.erase_flashblock(offset);

    if (offset<tcrtSize)
    {
      long bytesread = tcrtFile.read(databuffer, tc_cmd.page_size);
      if (bytesread==0)
        break;
      
      tc_cmd.write_flash(offset, bytesread, databuffer);
      if (useCrc)
        crc = Crc::crc_update(crc, databuffer, bytesread);
    }

    if (offset % 0x4000 == 0)
    {
      sprintf(printbuffer, FCSTR("write %06lx\r"), offset);
      Serial.print(printbuffer);
      if (offset<tcrtSize)
      {
        led = !led;
        if (led)
          tc_cmd.led_on();
        else
          tc_cmd.led_off();
      }
    }

    offset += tc_cmd.page_size;
  }
  sprintf(printbuffer, FCSTR("write %06lx"), offset);
  Serial.println(printbuffer);
  tcrtFile.close();
  tc_cmd.led_on();

  //sprintf(printbuffer, FCSTR("offset=%06lx"), offset);
  //Serial.println(printbuffer);

  Serial.print(F("write finished, time="));
  Serial.print((millis()-start_time)/1000);
  Serial.println('s');

  //start_time = millis();

  if (useCrc)
  {
    Serial.println(F("calculating checksum..."));
    Serial.flush();
    crc = Crc::crc_finalize(crc);
    uint32_t crc_flash =  tc_cmd.crc32_flash(0x000000, tcrtSize);
    sprintf(printbuffer, FCSTR("crc %06lx %06lx"), crc, crc_flash);
    Serial.println(printbuffer);
    if (crc != crc_flash)
    {
      Serial.println(F("checksum error"));
      return false;
    }
  }

  Serial.print(F("chksum finished, total time="));
  Serial.print((millis()-start_time)/1000);
  Serial.println('s');
  
  // bell
  Serial.print('\x7');

  return true;
}

//-------------------------------------------------------------------------------------------------
// Write data from header to TapeCart

bool Tcrt::write_tcrt_header(const uint8_t *header, uint32_t total_size, uint32_t *tcrtSize, byte loaderMode)
{
  uint32_t flashsize;

  // sanity-check TCRT header
  if (memcmp_P(databuffer, tcrt_signatur, TCRT_SIG_SIZE))
  {
    Serial.println(F("invalid TCRT header"));
    return false;
  }
  if (databuffer[TCRT_OFFSET_VERSION] != TCRT_VERSION || databuffer[TCRT_OFFSET_VERSION + 1] != 0)
  {
    Serial.println(F("unknown TCRT version"));
    return false;
  }

  memcpy(tcrtSize, databuffer + TCRT_OFFSET_FLASHSIZE, 4);
  if (*tcrtSize > total_size)
  {
    Serial.println(F("TCRT size exceeds cartridge size"));
    return false;
  }

  if (loaderMode==LOADER_MODE_NONE || (databuffer[TCRT_OFFSET_FLAGS] & TCRT_FLAG_LOADERPRESENT)==0)
    loaderMode = LOADER_MODE_DEFAULT;

  switch(loaderMode)
  {
    case LOADER_MODE_TCRT:
      Serial.println(F("using loader from TCRT file"));
      break;
    case LOADER_MODE_DEFAULT:
      memcpy_P(databuffer + TCRT_OFFSET_LOADER, loader, LOADER_LENGTH);    
      databuffer[TCRT_OFFSET_FLAGS] = 0x01;
      Serial.println(F("using default loader"));
      break;
  }

  // avoid writing loader if it's identical to the one already in Tapecart
  tc_cmd.read_loader(loaderbuffer);
  if (memcmp(databuffer + TCRT_OFFSET_LOADER, loaderbuffer, LOADER_LENGTH))
  {
    tc_cmd.write_loader(databuffer + TCRT_OFFSET_LOADER);
  }

  memcpy(&loadinfo, databuffer + TCRT_OFFSET_LOADER_CALLADDR, sizeof(loadinfo));
  tc_cmd.write_loadinfo(loadinfo.dataofs, loadinfo.datalen, loadinfo.calladdr, databuffer + TCRT_OFFSET_LOADER_NAME);

  sprintf(printbuffer, FCSTR("TCRT size=%lxh, Loader info=%xh, Loader length=%xh, Loader calladdr=%xh, '"),
      *tcrtSize, loadinfo.dataofs, loadinfo.datalen, loadinfo.calladdr);
  Serial.print(printbuffer);
  for(byte i=0; i<16; i++)
    Serial.print((char)(databuffer[TCRT_OFFSET_LOADER_NAME + i]));
  Serial.println('\'');

  //Helper::dumpBuffer(databuffer, TCRT_HEADERSIZE);
  //Serial.println();
  
  return true;
}

//--------------------------------------------------------------------------
// read TCRT file from Tapecart to SD card

bool Tcrt::read_tcrt_file(char *fname, byte loaderMode)
{
  uint16_t *uint16_p;
  uint32_t *uint32_p;

  sd.remove(fname);
  File tcrtFile = sd.open(fname, FILE_WRITE);
  if (tcrtFile == NULL)
  {
    Serial.print(F("error writing '"));
    Serial.print(fname);
    Serial.println('\'');
    return false;
  }

  memset(databuffer, 0x00, TCRT_HEADERSIZE);
  memcpy_P(databuffer, tcrt_signatur, TCRT_SIG_SIZE);
  uint16_p = (uint16_t*)(databuffer + TCRT_OFFSET_VERSION);
  *uint16_p = TCRT_VERSION;
  uint32_p = (uint32_t*)(databuffer + TCRT_OFFSET_FLASHSIZE);
  *uint32_p = MAX_FLASH_SIZE;

  switch(loaderMode)
  {
    case LOADER_MODE_NONE:
      databuffer[TCRT_OFFSET_FLAGS] = 0x00;
      break;
    case LOADER_MODE_TCRT:
      tc_cmd.read_loader(databuffer + TCRT_OFFSET_LOADER);
      databuffer[TCRT_OFFSET_FLAGS] = 0x01;
      break;
    case LOADER_MODE_DEFAULT:
      memcpy_P(databuffer + TCRT_OFFSET_LOADER, loader, sizeof LOADER_LENGTH);    
      databuffer[TCRT_OFFSET_FLAGS] = 0x01;
      break;
  }

  tc_cmd.read_loadinfo(&loadinfo.dataofs, &loadinfo.datalen, &loadinfo.calladdr, (char*)(databuffer + TCRT_OFFSET_LOADER_NAME));
  memcpy(databuffer + TCRT_OFFSET_LOADER_CALLADDR, &loadinfo, sizeof(loadinfo));

  //CmdMode::dump_buffer(databuffer, TCRT_HEADERSIZE);
  //tcrtFile.write(databuffer, TCRT_HEADERSIZE);

  uint32_t offset = 0;
  long start_time = millis();
  while(offset < MAX_FLASH_SIZE)
  {
    tc_cmd.read_flash(offset, DATABUFFER_MAX, databuffer);
    tcrtFile.write(databuffer, DATABUFFER_MAX);
    if (offset % 0x4000 == 0)
    {
      sprintf(printbuffer, FCSTR("read %06lx\r"), offset);
      Serial.print(printbuffer);
      //Serial.println(tcrtFile.position(), HEX);
    }
    offset += tc_cmd.page_size;
  }
  sprintf(printbuffer, FCSTR("write %06lx"), offset);
  Serial.println(printbuffer);
  tcrtFile.close();
  Serial.print(F("finished, time="));
  Serial.print((millis()-start_time)/1000);
  Serial.println('s');

  // bell
  Serial.print('\x7'); 

  return true;
}

//-------------------------------------------------------------------------------------------------
// Tcrt.cpp
//-------------------------------------------------------------------------------------------------

