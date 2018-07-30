//--------------------------------------------------------------------------
// TapecartFlasher by *dg*
// Arduino Sketch for programming the Tapecart module with an Arduino UNO/NANO
//
// 05.07.2018 *dg* Project start
// 18.07.2018 *dg* Implemented all neccessary api functions
//                 Renamed project to TapecartFlasher
//                 Added xor checksum to protcol
// 19.07.2018 *dg* Implemented Erase64K
//                 Implemented 32 Byte block download from pc
//                 Fixed bit shifting errors
// 20.07.2018 *dg* Version 1.0/1 released
// 27.07.2018 *dg* API-Funktion LedOn / LedOff implementiert
//
// Arduino Serial API format:
// command: '#' <cmdgroup:8> <cmd:8> <datalen:16> <data 0 ... data n> <chksum:8> (binary format)
// result: '#' <cmdgroup:8> <cmd:8> <status:8> <datalen:16> <data 0 ... data n> <chksum:8>
// 1-byte checksum is simple XOR

#include "TapecartFlasher.h"
#include "CmdMode.h"
#include "Tcrt.h"
#include "PcCommands.h"

#include "Crc.h"

//#define USE_SDCARD

// because of the small memory of the Arduino UNO we use only one buffer for
// communication with the pc an the communication with the Tapecart module.
// To handle one data block we need 256 byte + protocol header
uint8_t databuffer[DATABUFFER_MAX + DATABUFFER_HEADER];

CmdMode tc_cmd;
PcCommands pc_cmd;

bool sdCardOk = false;

//--------------------------------------------------------------------------

void setup()
{
  pinMode(PIN_MOTOR, OUTPUT);
  pinMode(PIN_READ, INPUT_PULLUP);
  pinMode(PIN_WRITE, OUTPUT);
  pinMode(PIN_SENSE, INPUT);
  pinMode(PIN_LED, OUTPUT);

  Serial.begin(115000);
  //Serial.begin(57600);
  Serial.setTimeout(1000);
  Serial.print(F("*Tapecart V"));
  Serial.print(MAJOR_VERSION);
  Serial.print('.');
  Serial.print(MINOR_VERSION);
  Serial.print('/');
  Serial.println(API_VERSION);

  //sdInfo();
  sdCardOk = SD.begin(SD_SEL);
  if (!sdCardOk)
    Serial.println(F("*SD failed!"));
  //sdDir();

  if (!tc_cmd.start())
  {
    Serial.println(F("*error starting tapecart"));
    return;
  }
#ifndef USE_SDCARD
  //tc_cmd.exit();
#endif

  //tc_cmd.write_debugflags(0x0000);
  //read_loader_test();
  //command_test();
  //writeflash_test();
  //Tcrt::load_tcrt_file(FCSTR("devilv1.tcr"));
}

//--------------------------------------------------------------------------

void loop()
{
  pc_cmd.readCmd();
}

//--------------------------------------------------------------------------

//#if false

void command_test()
{
  String str;

  tc_cmd.read_deviceinfo(databuffer, DATABUFFER_MAX);
  Serial.print(F("deviceinfo: "));
  Serial.println((char *)databuffer);

/*
  uint16_t offset;
  uint16_t length;
  uint16_t calladdr;
  char filename[FILENAME_LENGTH+1];
  tc_cmd.read_loadinfo(&offset, &length, &calladdr, filename);
  sprintf(printbuffer, FCSTR("loadinfo: $%04x $%04x $%04x '%s'"), offset, length, calladdr, filename);
  Serial.println(printbuffer);

  tc_cmd.read_loader(databuffer);
  Serial.println(F("read_loader:"));
  dump_buffer(databuffer, LOADER_LENGTH);

  Serial.println(F("read_flash:"));
  tc_cmd.read_flash(0x000000, DATABUFFER_MAX, databuffer);
  dump_buffer(databuffer, DATABUFFER_MAX);
  */
}

//#endif

//--------------------------------------------------------------------------

void writeflash_test()
{
  if (!tc_cmd.start())
    return;

  uint32_t offset = 0;
  long start_time = millis();
  int cnt = 0;
  /*
  while(offset < tc_cmd.total_size)
  //while(offset < 0x1000)
  {
    if (offset % tc_cmd.erase_size == 0)
      tc_cmd.erase_flashblock(offset);

    //if (offset % 0x1000 == 0)
    {
      Serial.print("write ");
      Serial.println(offset, HEX);
    }
    
    for (int i=0; i<tc_cmd.page_size; i++)
      databuffer[i] = (cnt+i) % 256;
    databuffer[0] = (offset>>8) & 0xFF;
    databuffer[1] = (offset>>16) & 0xFF;
    Serial.print(databuffer[0], HEX);
    Serial.print(" ");
    Serial.println(databuffer[1], HEX);
    tc_cmd.write_flash(offset, tc_cmd.page_size, databuffer);
    
    offset += tc_cmd.page_size;
    cnt++;
  }
  */

  offset = 0;
  cnt = 0;
  while(offset < tc_cmd.total_size)
  //while(offset < 0x1000)
  {
    //if (offset % 0x1000 == 0)
    {
      Serial.print("read ");
      Serial.println(offset, HEX);
    }
    tc_cmd.read_flash(offset, tc_cmd.page_size, databuffer);
    uint32_t addr = ((uint32_t)databuffer[0]<<8) + ((uint32_t)databuffer[1]<<16);
    if (addr!=offset)
    {
        Serial.print("error ");
        Serial.print(offset, HEX);
        Serial.print(" ");
        Serial.println(addr, HEX);
        CmdMode::dump_buffer(databuffer, tc_cmd.page_size);
        while(Serial.read()!='#');
    }
    offset += tc_cmd.page_size;
    cnt++;
  }

/*
    uint32_t crc2 = Crc::crc_init();
    crc2 = Crc::crc_update(crc2, databuffer, tc_cmd.page_size); 
    crc2 = Crc::crc_finalize(crc2);
    uint32_t crc3 =  tc_cmd.crc32_flash(offset, tc_cmd.page_size);
    Serial.print(offset, HEX);
    Serial.print(F(" calc="));
    Serial.print(crc2, HEX);
    Serial.print(F(" flash="));
    Serial.println(crc3, HEX);
    if (crc2!=crc3)
    {
      while (Serial.read()!='#');
    }
   
    offset += tc_cmd.page_size;
    cnt += 7;
  }
  */
  Serial.print(F("time="));
  Serial.println(start_time/1000);
}

//--------------------------------------------------------------------------

#if false

void read_loader_test()
{
  tc_cmd.read_loader(databuffer);
  File lf = SD.open("loader.bin", FILE_WRITE);
  lf.write(databuffer, LOADER_LENGTH);
  lf.close();
}

#endif

//--------------------------------------------------------------------------

#if false
void sdDir()
{
  File dir = SD.open("/");
  while (true)
  {
    File entry = dir.openNextFile();
    if (!entry)
      return;
    Serial.print(entry.name());
    Serial.print(" ");
    Serial.println(entry.size());
    entry.close();
  };
  dir.close();  
}
#endif

//--------------------------------------------------------------------------

#if false

Sd2Card sdCard;
SdVolume sdVolume;
SdFile sdRoot;

void sdInfo()
{
  if (!sdCard.init(SPI_HALF_SPEED, SD_SEL))
  {
    Serial.println(F("sdInit error"));
    return;
  }
  
  Serial.println(F("sdInit ok"));

 // print the type of card
  Serial.print(F("Card type: "));
  switch (sdCard.type())
  {
    case SD_CARD_TYPE_SD1:
      Serial.println(F("SD1"));
      break;
    case SD_CARD_TYPE_SD2:
      Serial.println(F("SD2"));
      break;
    case SD_CARD_TYPE_SDHC:
      Serial.println(F("SDHC"));
      break;
    default:
      Serial.println(F("Unknown"));
      return;
  }

  // Now we will try to open the 'volume'/'partition' - it should be FAT16 or FAT32
  if (!sdVolume.init(sdCard))
  {
    Serial.println(F("no FAT16/FAT32 partition."));
    return;
  }

  // print the type and size of the first FAT-type volume
  uint32_t volumesize;
  Serial.print(F("Volume type is FAT"));
  Serial.println(sdVolume.fatType(), DEC);

  volumesize = sdVolume.blocksPerCluster();    // clusters are collections of blocks
  volumesize *= sdVolume.clusterCount();       // we'll have a lot of clusters
  volumesize *= 512;                            // SD card blocks are always 512 bytes
  Serial.print(F("Volume size (bytes): "));
  Serial.println(volumesize);
  /*
  Serial.print("Volume size (Kbytes): ");
  volumesize /= 1024;
  Serial.println(volumesize);
  Serial.print("Volume size (Mbytes): ");
  volumesize /= 1024;
  Serial.println(volumesize);
  */

  Serial.println(F("*Files found on the card (name, date and size in bytes): "));
  sdRoot.openRoot(sdVolume);

  // list all files in the card with date and size
  sdRoot.ls(LS_R | LS_DATE | LS_SIZE);
}

#endif

//--------------------------------------------------------------------------

