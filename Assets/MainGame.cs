using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class MainGame : MonoBehaviour {
    public Vector2 _blockSize;
    public GameObject[,] _arrSpriteIcon;
    public int _arrWidth;
    public int _arrHeight;
    public GameObject _block;
    public float _newBlcokDur;
	private float _creatTargetTime;
    public int idxx;
	public List<GameObject> _arrAllBlock;

    private BaseGameState _curState;
    private GameStateNor _norGameState;
    private GameStateMove _moveGameState;

    private int flag = 0;
    public Vector2 _startOffPos;
    private float _speedValue;
    private float _protectInputTime;

    private Vector2 _moveTouchPos;
    private Vector2 _moveWorldPos;
    private int _curNeedComboNum = 4;
    private List<WaitingForDelStruct> _arrNeedDelList;
    public enum GameStage
    {
        NOR = 0,
        MOVE,
        CHECK,
    };

    GameStage _curGameStage;

	void Awake()
	{
		_newBlcokDur = 1.0f;
		_creatTargetTime = Time.time + _newBlcokDur;

		_arrAllBlock = new List<GameObject>();
		_arrSpriteIcon = new GameObject[6, 12];
		
		_curGameStage = GameStage.NOR;	
		_norGameState =  gameObject.AddComponent<GameStateNor>();
		_moveGameState = gameObject.AddComponent<GameStateMove>();

        _speedValue = 4.0f;
        _protectInputTime = 0.0f;
        _blockSize = new Vector2(0.60f, 0.60f);
        GetComponent<Camera>().aspect = 480.0f / 800.0f;
        Debug.Log("awark:" + _arrWidth);

        _arrNeedDelList = new List<WaitingForDelStruct>();
    }

    public float getSpeedValue()
    {
        return _speedValue;
    }

    void Start()
    {            
        _curState = _norGameState;
        _curState.enterState();
        Debug.Log(_arrWidth);

    }

   public  void startProtect()
    {
        _protectInputTime = Time.time + 0.3f;
    }

   public void changeStage(GameStage stage)
    {
        if (_curGameStage == stage)
            return;

        _curGameStage = stage;

        _curState.leaveState();

       switch(stage)
        {
            case GameStage.MOVE:
                _curState = _moveGameState;
                break;
            case GameStage.NOR:
                _curState = _norGameState;
                break;
            default:
                _curState = _norGameState;
                break;
        }

        _curState.enterState();
    }

    void HandleInput()
    {
        if(Time.time < _protectInputTime)
        {
            return;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            if(touchDeltaPosition.magnitude < 10.0f)
            {
                return;
            }
            Debug.Log(touchDeltaPosition);
            if(Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y))
            {
                if(touchDeltaPosition.x > 0)
                {
                    Vector2 temptouchpos = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);                  
                    Vector3 worldpos =  Camera.main.ScreenToWorldPoint(temptouchpos);

                    _moveTouchPos = temptouchpos;
                    _moveWorldPos = worldpos;

                    _curState.handleInput(BaseGameState.GameInputEvent.MoveRight, (int)getPosInArry(new Vector2(worldpos.x, worldpos.y)).y);
                }
                else
                {
                    Vector2 temptouchpos = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                    Vector3 worldpos = Camera.main.ScreenToWorldPoint(temptouchpos);

                    _moveTouchPos = temptouchpos;
                    _moveWorldPos = worldpos;

                    _curState.handleInput(BaseGameState.GameInputEvent.MoveLeft, (int)getPosInArry(new Vector2(worldpos.x, worldpos.y)).y);
                }
            }
            else
            {
                if (touchDeltaPosition.y > 0)
                {
                    Vector2 temptouchpos = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                    Vector3 worldpos = Camera.main.ScreenToWorldPoint(temptouchpos);

                    _moveTouchPos = temptouchpos;
                    _moveWorldPos = worldpos;

                    _curState.handleInput(BaseGameState.GameInputEvent.MoveUp, (int)getPosInArry(new Vector2(worldpos.x, worldpos.y)).x);
                }
                else
                {
                    Vector2 temptouchpos = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                    Vector3 worldpos = Camera.main.ScreenToWorldPoint(temptouchpos);

                    _moveTouchPos = temptouchpos;
                    _moveWorldPos = worldpos;

                    _curState.handleInput(BaseGameState.GameInputEvent.MoveDown, (int)getPosInArry(new Vector2(worldpos.x, worldpos.y)).x);
                }
            }
        }

        //else if(Input.GetMouseButtonDown(0))
        //{
        //    _curState.handleInput(BaseGameState.GameInputEvent.MoveLeft, 0);
        //}
        //else if (Input.GetMouseButtonDown(1))
        //{
        //    _curState.handleInput(BaseGameState.GameInputEvent.MoveRight, 0);
        //}
        //else if (Input.GetMouseButtonDown(2))
        //{
        //    _curState.handleInput(BaseGameState.GameInputEvent.MoveUp, 1);
        //}
    }

    // Update is called once per frames
    void Update()
    {
        if (Time.time >= _creatTargetTime && flag < 45)
        {
            int idx = Random.Range(0, 12);          
            _block.GetComponent<BlockAni>()._picidx = idx * 3;

            int x = Random.Range(0, 5);        
            GameObject temp = Instantiate(_block);
            temp.GetComponent<BlockAni>().setPosByArrIdx(x, 11);
            temp.GetComponent<BlockAni>().setBlockState(BlockAni.BlockState.Down);
           _creatTargetTime = Time.time + _newBlcokDur;

			_block.GetComponent<BlockAni>()._lastPso.x = x;
			_block.GetComponent<BlockAni>()._lastPso.y = 11;

            _arrAllBlock.Add(temp);

			flag++;
        }

        _curState.StateUpdate();
        HandleInput();
        tryDelBlocks();

    }

    public void tryDelBlocks()
    {
        for (int i = _arrNeedDelList.Count - 1; i >= 0; i--)
        {
            _arrNeedDelList[i].updateTime();
            if (!_arrNeedDelList[i]._activity)
                _arrNeedDelList.RemoveAt(i);
        }

        for (int j = 11; j >= 0; j--)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_arrSpriteIcon[i, j] != null)
                {
                    BlockAni cs = (_arrSpriteIcon[i, j]).GetComponent<BlockAni>();
                    if(cs.getBlockState() == BlockAni.BlockState.ShouldDel)
                    {
                        GameObject temp = _arrSpriteIcon[i, j];
                        _arrAllBlock.Remove(_arrSpriteIcon[i, j]);
                        _arrSpriteIcon[i, j] = null;
                       Destroy(temp);
                    }
                }
            }
        }      
    }

    public Vector2 getPosInArry(Vector2 truepos)
    {
        float x = ((truepos.x - _startOffPos.x) / _blockSize.x);
        float y = ((truepos.y ) / _blockSize.y);
        if (truepos.y < 0)
            y -= 1;
        return new Vector2(x,y);
    }

    void OnGUI()
    {
        string str = "";
        for (int i = 11; i >= 0; i--)
        {
            str += "\n";
            for(int j = 0; j < 6; j++)
            {
                if (_arrSpriteIcon[j, i] != null)
                {
                    str += "1";
                }
                else
                {
                    str += "0";
                }
            }
        }
        GUI.Label(new Rect(10, 10, 200, 800), str);
        string str2 = "";
        str2 += _moveWorldPos;
        str2 += "\n";
        str2 += getPosInArry(_moveWorldPos);
        GUI.Label(new Rect(10, 300, 300, 800), str2);
    }

    public void updatePos()
    {
        for (int i = 0; i < _arrAllBlock.Count; i++)
        {

            BlockAni cs = (_arrAllBlock[i]).GetComponent<BlockAni>();
            if (cs.getBlockState() != BlockAni.BlockState.Move && cs.getBlockState() != BlockAni.BlockState.WaitingDel && cs.getBlockState() != BlockAni.BlockState.ShouldDel)
            {
                cs.updatePos();

                Vector2 cspos = cs.getPosInArry();
                Vector2 csposlast = cs._lastPso;

                int row = (int)cspos.y;
                int col = (int)cspos.x;
                int rowlast = (int)csposlast.y;
                int collast = (int)csposlast.x;

                if (cspos.y < 0)
                {
                    cs.setPosByArrIdx(col, 0);
                    if(cs.getBlockState() != BlockAni.BlockState.Nor)
                    {
                        Debug.Log("check < 0");
                        checkDel();
                    }
                    cs.setBlockState(BlockAni.BlockState.Nor);
                    cspos.y = 0;
                }

                else if (rowlast != row)
                {
                    if (row >= 0 && row <= 11)
                    {
                        if (_arrSpriteIcon[col, row] != null)
                        {
                            if (_arrSpriteIcon[col, row] != _arrAllBlock[i])
                            {
                                cs.setPosByArrIdx(col, row + 1);
                                if (cs.getBlockState() != BlockAni.BlockState.Nor)
                                {
                                    Debug.Log("check down");
                                    checkDel();
                                }
                                cs.setBlockState(BlockAni.BlockState.Nor);
                            }

                        }
                        else
                        {
                            _arrSpriteIcon[collast, rowlast] = null;
                            _arrSpriteIcon[col, row] = _arrAllBlock[i];
                            cs.setLastPos(cspos);
                        }
                    }
                }
                else
                {
                 
                }
            }
        }
    }

    public void checkDel()
    {

        List<GameObject> tempCheckList = new List<GameObject>();

        for(int i = 0; i < _arrWidth; i++ )
        {
            for(int j = 0; j < _arrHeight; j++)
            {
                if(_arrSpriteIcon[i,j]!=null)
                {
                    BlockAni cs = (_arrSpriteIcon[i, j]).GetComponent<BlockAni>();

                    if(cs.getBlockState() == BlockAni.BlockState.WaitingDel)
                    {
                        continue;
                    }

                    if(tempCheckList.Count == 0)
                    {
                        tempCheckList.Add(_arrSpriteIcon[i, j]);
                    }
                    else
                    {
                        BlockAni lastTempBlack = (tempCheckList[tempCheckList.Count - 1]).GetComponent<BlockAni>();
                        if (lastTempBlack.getPicIdx() == cs.getPicIdx())
                        {
                            tempCheckList.Add(_arrSpriteIcon[i, j]);
                        }
                        else
                        {
                            if(tempCheckList.Count >= _curNeedComboNum)
                            {
                                WaitingForDelStruct del = new WaitingForDelStruct();                              
                                foreach(GameObject temp in tempCheckList)
                                {
                                    del.pushWaitingDelObject(temp);
                                }
                                _arrNeedDelList.Add(del);
                            }
                           
                            tempCheckList.Clear();                            
                        }
                    }
                   
                }
                else
                {
                    if (tempCheckList.Count >= _curNeedComboNum)
                    {
                        WaitingForDelStruct del = new WaitingForDelStruct();
                        foreach (GameObject temp in tempCheckList)
                        {
                            del.pushWaitingDelObject(temp);
                        }

                        _arrNeedDelList.Add(del);
                    }
                    tempCheckList.Clear();
                }
            }

            if (tempCheckList.Count >= _curNeedComboNum)
            {
                WaitingForDelStruct del = new WaitingForDelStruct();
                foreach (GameObject temp in tempCheckList)
                {
                    del.pushWaitingDelObject(temp);
                }

                _arrNeedDelList.Add(del);
            }         
           tempCheckList.Clear();           
        }

        checkDel2();
    }

    public void checkDel2()
    {

        List<GameObject> tempCheckList = new List<GameObject>();

        for (int i = 0; i < _arrHeight; i++)
        {
            for (int j = 0; j < _arrWidth; j++)
            {
                if (_arrSpriteIcon[j, i] != null)
                {
                    BlockAni cs = (_arrSpriteIcon[j, i]).GetComponent<BlockAni>();

                    if (cs.getBlockState() == BlockAni.BlockState.WaitingDel)
                    {
                        continue;
                    }

                    if (tempCheckList.Count == 0)
                    {
                        tempCheckList.Add(_arrSpriteIcon[j, i]);
                    }
                    else
                    {
                        BlockAni lastTempBlack = (tempCheckList[tempCheckList.Count - 1]).GetComponent<BlockAni>();
                        if (lastTempBlack.getPicIdx() == cs.getPicIdx())
                        {
                            tempCheckList.Add(_arrSpriteIcon[j, i]);
                        }
                        else
                        {
                            if (tempCheckList.Count >= _curNeedComboNum)
                            {
                                WaitingForDelStruct del = new WaitingForDelStruct();
                                foreach (GameObject temp in tempCheckList)
                                {
                                    del.pushWaitingDelObject(temp);
                                }
                                _arrNeedDelList.Add(del);
                            }

                            tempCheckList.Clear();
                        }
                    }

                }
                else
                {
                    if (tempCheckList.Count >= _curNeedComboNum)
                    {
                        WaitingForDelStruct del = new WaitingForDelStruct();
                        foreach (GameObject temp in tempCheckList)
                        {
                            del.pushWaitingDelObject(temp);
                        }

                        _arrNeedDelList.Add(del);
                    }
                    tempCheckList.Clear();
                }
            }

            if (tempCheckList.Count >= _curNeedComboNum)
            {
                WaitingForDelStruct del = new WaitingForDelStruct();
                foreach (GameObject temp in tempCheckList)
                {
                    del.pushWaitingDelObject(temp);
                }

                _arrNeedDelList.Add(del);
            }
            tempCheckList.Clear();
        }
    }

    public class WaitingForDelStruct
    {
        public List<GameObject> _waitingList = new List<GameObject>();      
        public float _durTime = 0.5f;
        public bool _activity = true;
        public void pushWaitingDelObject(GameObject pushobject)
        {
            if (!pushobject)
                Debug.Log("arr");

            BlockAni cs = (pushobject).GetComponent<BlockAni>();
            cs.setBlockState(BlockAni.BlockState.WaitingDel);
            _waitingList.Add(pushobject);
        }

        public void updateTime()
        {
            _durTime -= Time.deltaTime;
            if(_activity && _durTime <= 0.0f)
            {
                foreach (GameObject temp in _waitingList)
                {
                    BlockAni cs = (temp).GetComponent<BlockAni>();
                    cs.setBlockState(BlockAni.BlockState.ShouldDel);
                }
                _activity = false;
            }
        }
    }
}


