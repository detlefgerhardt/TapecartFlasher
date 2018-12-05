//-------------------------------------------------------------------------------------------------
// Console.h   *dg*    11.09.2018
//-------------------------------------------------------------------------------------------------

#ifndef Console_h
#define Console_h 

#define CMD_CON 0x0D // carriage return

//-------------------------------------------------------------------------------------------------

class Console
{
  public:
    Console();
    byte start();
    void static arduinoVersion();

  private:
    void showMenu();
    bool readMenu(char *buffer);
    bool readFilename(char *buffer);
    void deviceInit();
    void sdDir();
    bool checkTapecart();
    bool checkSdCard();
};

//-------------------------------------------------------------------------------------------------

#endif

//-------------------------------------------------------------------------------------------------


