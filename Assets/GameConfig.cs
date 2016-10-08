using UnityEngine;
using System.Collections;

public class GameConfig
{
    public static int GAMEROW = 12;
    public static int GAMECOLUMN = 6;
    public static int DESIGN_WIDTH = 640;
    public static int DESIGN_HEIGHT = 960;
    public static int TROZEI_SIZE = 80;
    public static int TROZEI_SIZEOFF = 40;
    public static Vector2 TROZEI_AREA_STARTPOS = new Vector2(0.0f, 0.0f);
    public static int SPEED_DOWN = 400;

    private static int s_waitForDelUniqueIdx = 0;
    public static  WaitingForDelStruct createStruct()
    {
        var temp = new WaitingForDelStruct();
        temp._uniqueId = s_waitForDelUniqueIdx;
        s_waitForDelUniqueIdx++;
        return temp;

    }
}

