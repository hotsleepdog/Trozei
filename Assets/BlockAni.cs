using UnityEngine;
using System.Collections;
using System.IO;

public class BlockAni : MonoBehaviour {
    
    public enum BlockState
    {
        UnActive = 0,
        Down = 1,
        Move = 2,
        WaitingDel = 3,
        ShouldDel = 4,
        Nor = 5,
    };

    public BlockState _curState { get; set; }
    public int _picidx { get; set; }
    public int _uniqueId { get; set; }
    public Vector2 _lastPos { get; set; }
    public bool _bCheckRow { get; set; }
    public bool _bCheckCol { get; set; }
    void Awake()
    {
        _curState = BlockState.Down;
    }

    // Use this for initialization
    void Start () {
        string url = "icons/" + _picidx;
        Sprite sprite0 = Resources.Load<Sprite>(url);
        GetComponent<SpriteRenderer>().sprite = sprite0;
    }
	
	// Update is called once per frame
	void Update () {
      
    }

    public Vector2 getPosInArry(){
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;

        int anchorZeroX = x - GameConfig.TROZEI_SIZEOFF;
        int anchorZeroY = y - GameConfig.TROZEI_SIZEOFF;

        int offx = anchorZeroX - (int)GameConfig.TROZEI_AREA_STARTPOS.x;
        int offy = anchorZeroY - (int)GameConfig.TROZEI_AREA_STARTPOS.y;

        int truex = offx / GameConfig.TROZEI_SIZE;
        int truey = offy / GameConfig.TROZEI_SIZE;

        if (offy < 0) {
            truey --;
        }

        return new Vector2(truex, truey);
    }

    public void setPosByArrIdx(int x, int y) {
        Vector2 truepos = new Vector2(GameConfig.TROZEI_AREA_STARTPOS.x + x * GameConfig.TROZEI_SIZE + GameConfig.TROZEI_SIZEOFF, GameConfig.TROZEI_AREA_STARTPOS.y + y * GameConfig.TROZEI_SIZE + GameConfig.TROZEI_SIZEOFF);
        transform.position = new Vector3(truepos.x, truepos.y, transform.position.z);
    }

    public void updatePos() {
        if (_curState == BlockState.Down || _curState == BlockState.Nor) {
            GetComponent<Transform>().transform.Translate(Vector3.down * Time.deltaTime * GameConfig.SPEED_DOWN);
        }
             
    }
}
