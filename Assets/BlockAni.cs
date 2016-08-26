using UnityEngine;
using System.Collections;
using System.IO;

public class BlockAni : MonoBehaviour {
    private Sprite[] _arrSpriteFrames;
    public float _aniDur;
    private float _nextChangeFrameTime;
    private int _curidx;
    private Vector2 _blockSize;
    public int _picidx;
    private Vector2 _startOffPos;

	public Vector2 _lastPso;
    private GameObject _mainGame;
    private float _speedValue;
    public enum BlockState
    {
        UnActive = 0,
        Down = 1,
        Move = 2,
        WaitingDel = 3,
        ShouldDel = 4,
        Nor = 5,
    };

    public BlockState _curState;

    void Awake()
    {
		_arrSpriteFrames = new Sprite[3];
        _mainGame = GameObject.FindGameObjectWithTag("MainCamera");
        _blockSize = new Vector2(0.6f, 0.6f);
        _startOffPos = new Vector2(0.0f, 0.0f);	
		_curState = BlockState.UnActive;     
    }

    // Use this for initialization
    void Start () {
        _nextChangeFrameTime = Time.time + _aniDur;     
       
        string url = "icons/"+_picidx;
        Sprite sprite0 = Resources.Load<Sprite>(url);
        GetComponent<SpriteRenderer>().sprite = sprite0;

        string url2 = "icons/" + (_picidx + 1);
        Sprite sprite1 = Resources.Load<Sprite>(url2);

        string url3 = "icons/" + (_picidx + 2);
        Sprite sprite2 = Resources.Load<Sprite>(url3);

        _arrSpriteFrames[0] = sprite0;
        _arrSpriteFrames[1] = sprite1;
        _arrSpriteFrames[2] = sprite2;

        _speedValue = _mainGame.GetComponent<MainGame>().getSpeedValue();
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time >= _nextChangeFrameTime) {
            _curidx += 1;
            if (_curidx >= _arrSpriteFrames.Length)
                _curidx = 0;

           GetComponent<SpriteRenderer>().sprite = _arrSpriteFrames[_curidx];
            _nextChangeFrameTime = Time.time + _aniDur;
        }
    }

    public Vector2 getPosInArry(){    
        Vector3 pos = GetComponent<Transform>().transform.position;
        pos.x -= _blockSize.x / 2.0f;
        pos.y -= _blockSize.y / 2.0f;
        
		float x = ((pos.x - _startOffPos.x) / _blockSize.x);
		float y = (pos.y / _blockSize.y);
		if (pos.y < 0)
            y -= 1;
        return new Vector2((int)x, (int)y);
    }

    public void setPosByArrIdx(int x, int y) {
        float tx = _startOffPos.x + x * _blockSize.x + _blockSize.x / 2.0f;
        float ty = _startOffPos.y + y * _blockSize.y + _blockSize.y / 2.0f;
        GetComponent<Transform>().transform.position = new Vector3(tx, ty, GetComponent<Transform>().transform.position.z);
    }

    public void setLasePosX()
    {
        int x = (int)_lastPso.x;
        float tx = _startOffPos.x + x * _blockSize.x + _blockSize.x / 2.0f;
        GetComponent<Transform>().transform.position = new Vector3(tx, GetComponent<Transform>().transform.position.y, GetComponent<Transform>().transform.position.z);
    }

    public void updatePos() {   
        GetComponent<Transform>().transform.Translate(Vector3.down * Time.deltaTime * _speedValue);      
    }

    public void setLastPos(Vector2 lastpos)
    {
        _lastPso = lastpos;
    }

	public void setLastPosCur()
	{
        setPosByArrIdx((int)_lastPso.x, (int)_lastPso.y);
	}

    public BlockState getBlockState()
    {
        return _curState;
    }

    public void setBlockState(BlockState state)
    {
        _curState = state;
    }
}
