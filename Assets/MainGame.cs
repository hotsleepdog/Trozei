using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class MainGame : MonoBehaviour {
    public Vector2 _blockSize;
    public GameObject[,] _arrSpriteIcon;
    private int _arrWidth = GameConfig.GAMECOLUMN;
    private int _arrHeight = GameConfig.GAMEROW;
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
		_newBlcokDur = 0.5f;
		_creatTargetTime = Time.time + _newBlcokDur;

		_arrAllBlock = new List<GameObject>();
		_arrSpriteIcon = new GameObject[GameConfig.GAMECOLUMN, GameConfig.GAMEROW];
		
		_curGameStage = GameStage.NOR;	
		_norGameState =  gameObject.AddComponent<GameStateNor>();
		_moveGameState = gameObject.AddComponent<GameStateMove>();

        _speedValue = GameConfig.SPEED_DOWN;
        _protectInputTime = 0.0f;
        _blockSize = new Vector2(GameConfig.TROZEI_SIZE, GameConfig.TROZEI_SIZE);
        GetComponent<Camera>().orthographicSize = 320 / GetComponent<Camera>().aspect;
        Debug.Log("size:" + GetComponent<Camera>().orthographicSize);
        GetComponent<Camera>().transform.position = new Vector3(320, GetComponent<Camera>().orthographicSize, -10);
        //GetComponent<Camera>().
        Debug.Log("awark:" + _arrWidth);

        _arrNeedDelList = new List<WaitingForDelStruct>();
    }

    public void OnClickTest()
    {
        Debug.Log("click button");
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
                Debug.Log("stage change check");
                checkDel();
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

        if(Input.GetKeyDown(KeyCode.A))
        {
           _curState.handleInput(BaseGameState.GameInputEvent.MoveLeft, 0);
        }
       else if (Input.GetKeyDown(KeyCode.D))
        {
            _curState.handleInput(BaseGameState.GameInputEvent.MoveRight, 0);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
           _curState.handleInput(BaseGameState.GameInputEvent.MoveUp, 0);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _curState.handleInput(BaseGameState.GameInputEvent.MoveDown, 0);
        }
    }

    public void addWaitingForDelStruct(WaitingForDelStruct wait)
    {
        bool find = false;
        for (int i = _arrNeedDelList.Count - 1; i >= 0; i--)
        {
            if (wait._uniqueId == _arrNeedDelList[i]._uniqueId)
                Debug.Break();
        }

        _arrNeedDelList.Add(wait);
    }

    // Update is called once per frames
    void Update()
    {
        int[] arr = new int[]{
            0,4,5,4,5,0,
            0,3,3,3,4,0,
            0,2,2,2,3,0,
            1,0,0,2,0,0
        };
        if (Time.time >= _creatTargetTime && flag < 24)
        {
            int idx = Random.Range(0, 5);
            int x = Random.Range(0, GameConfig.GAMECOLUMN);
           
            GameObject temp = Instantiate(_block);
            temp.GetComponent<BlockAni>().setPosByArrIdx(flag%6, GameConfig.GAMEROW - 1);
            temp.GetComponent<BlockAni>()._curState = (BlockAni.BlockState.TopDown);           
            temp.GetComponent<BlockAni>()._lastPos = new Vector2(flag % 6, GameConfig.GAMEROW - 1);
            temp.GetComponent<BlockAni>()._picidx = arr[flag];

            _creatTargetTime = Time.time + _newBlcokDur;
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
            bool shouldremove = false;
            if (!_arrNeedDelList[i]._activity)
                shouldremove = true;

            if (_arrNeedDelList[i]._needRecovery)
            {
                _arrNeedDelList[i].resetState();
                shouldremove = true;
            }

            if (shouldremove)
            {
                _arrNeedDelList.RemoveAt(i);
            }

        }

        for (int j = GameConfig.GAMEROW - 1; j >= 0; j--)
        {
            for (int i = 0; i < GameConfig.GAMECOLUMN; i++)
            {
                if (_arrSpriteIcon[i, j] != null)
                {
                    BlockAni cs = (_arrSpriteIcon[i, j]).GetComponent<BlockAni>();
                    if(cs._curState == BlockAni.BlockState.ShouldDel)
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
        bool needcheck = false;
        for (int i = 0; i < _arrAllBlock.Count; i++)
        {

            BlockAni cs = (_arrAllBlock[i]).GetComponent<BlockAni>();

            if (cs._curState == BlockAni.BlockState.CountDown || cs._curState == BlockAni.BlockState.MoveUpDown 
                //|| cs._curState == BlockAni.BlockState.MoveLR  
                || cs._curState == BlockAni.BlockState.ShouldDel)
            {
                continue;
            }

            cs.updatePos();
            Vector2 cspos = cs.getPosInArry();
            Vector2 csposlast = cs._lastPos;

           int row = (int)cspos.y;
           int col = (int)cspos.x;
           int rowlast = (int)csposlast.y;
           int collast = (int)csposlast.x;
           if (cspos.y < 0)
           {
                //cs.setPosByArrIdx(col, 0);
                cs.setPosYByArrIdx(0);
                if (cs._curState != BlockAni.BlockState.Stand)
                {
                    Debug.Log("check < 0");
                    needcheck = true;
                }
                cs._curState = BlockAni.BlockState.Stand;
                cspos.y = 0;
            }

            else if (rowlast != row)
            {
                if (row >= 0 && row <= GameConfig.GAMEROW)
                {
                    if (_arrSpriteIcon[col, row] != null)
                    {
                        if (_arrSpriteIcon[col, row] != _arrAllBlock[i])
                        {
                            //cs.setPosByArrIdx(col, row + 1);
                            cs.setPosYByArrIdx(row + 1);
                            if (cs._curState != BlockAni.BlockState.Stand)
                            {
                                Debug.Log("check down");
                                needcheck = true;
                            }
                           cs._curState = (BlockAni.BlockState.Stand);
                        }

                    }
                    else
                    {                 
                        _arrSpriteIcon[collast, rowlast] = null;
                        _arrSpriteIcon[col, row] = _arrAllBlock[i];
                        cs._lastPos = cspos;

                        if (cs._curState != BlockAni.BlockState.TopDown)
                        {
                            cs._curState = (BlockAni.BlockState.ReDown);
                        }

                    }
                }
            }
            else
            {
                 
            }
            
        }

        if (needcheck)
            checkDel();
    }


    public void checkDel()
    {
        if (_curState == _moveGameState)
            return;

        for (int i = 0; i < _arrAllBlock.Count; i++)
        {

            BlockAni cs = (_arrAllBlock[i]).GetComponent<BlockAni>();

            if (cs._curState == BlockAni.BlockState.ReDown)
                return;           
        }

            List<GameObject> checklist = new List<GameObject>();
        for (int row = 0; row < GameConfig.GAMEROW; row++)
        {
            for(int col = 0; col < GameConfig.GAMECOLUMN; col++)
            {

                //bool isRowHaveDeling = false;
                //bool isColHaveDeling = false;
                int delingRowIdx = -1;
                int delingColIdx = -1;

                if (_arrSpriteIcon[col, row] != null)
                {
                    List<GameObject> tempColList = new List<GameObject>();
                    List<GameObject> tempRowList = new List<GameObject>();

                    BlockAni cs = _arrSpriteIcon[col, row].GetComponent<BlockAni>();
                    int checkPicId = cs._picidx;

                    if (cs._curState == BlockAni.BlockState.CountDown || cs._curState == BlockAni.BlockState.ShouldDel || cs._curState == BlockAni.BlockState.MoveLR || cs._curState == BlockAni.BlockState.MoveUpDown)
                    {
                        continue;
                    }

                    if (cs._bCheckCol == true && cs._bCheckRow == true)
                    {
                        continue;
                    }
                    else
                    {
                        cs._bCheckCol = true;
                        cs._bCheckRow = true;
                        checklist.Add(_arrSpriteIcon[col, row]);
                    }



                    int tempCol = col + 1;
                    while (tempCol != GameConfig.GAMECOLUMN)
                    {
                        if (_arrSpriteIcon[tempCol, row] != null)
                        {
                            BlockAni tempcs = _arrSpriteIcon[tempCol, row].GetComponent<BlockAni>();
                            int curId = tempcs._picidx;
                            if (curId == checkPicId && tempcs._bCheckCol == false)
                            {
                                if (tempcs._curState == BlockAni.BlockState.CountDown)
                                {                                   
                                    delingColIdx = tempcs._uniqueId;
                                }                            
                                
                                    tempcs._bCheckCol = true;
                                    checklist.Add(_arrSpriteIcon[tempCol, row]);
                                    tempColList.Add(_arrSpriteIcon[tempCol, row]);                                                                                               
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }

                        tempCol++;
                    }

                    tempCol = col - 1;
                    while (tempCol !=  - 1)
                    {
                        if (_arrSpriteIcon[tempCol, row] != null)
                        {
                            BlockAni tempcs = _arrSpriteIcon[tempCol, row].GetComponent<BlockAni>();
                            int curId = tempcs._picidx;
                            if (curId == checkPicId && tempcs._bCheckCol == false)
                            {
                                if (tempcs._curState == BlockAni.BlockState.CountDown)
                                {
                                    //isColHaveDeling = true;
                                    delingColIdx = tempcs._uniqueId;
                                }
                                
                                    tempcs._bCheckCol = true;
                                    checklist.Add(_arrSpriteIcon[tempCol, row]);
                                    tempColList.Add(_arrSpriteIcon[tempCol, row]);
                                
                               
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }

                        tempCol--;
                    }


                    int tempRow = row + 1;
                    while (tempRow != GameConfig.GAMEROW)
                    {
                        if (_arrSpriteIcon[col, tempRow] != null)
                        {
                            BlockAni tempcs = _arrSpriteIcon[col, tempRow].GetComponent<BlockAni>();
                            int curId = tempcs._picidx;
                            if (curId == checkPicId && tempcs._bCheckRow == false)
                            {
                                if (tempcs._curState == BlockAni.BlockState.CountDown)
                                {
                                    //isRowHaveDeling = true;
                                    delingRowIdx = tempcs._uniqueId;
                                }
                                
                                    tempRowList.Add(_arrSpriteIcon[col, tempRow]);
                                    tempcs._bCheckRow = true;
                                    checklist.Add(_arrSpriteIcon[col, tempRow]);
                                
                            
                                
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }

                        tempRow++;
                    }

                    tempRow = row - 1;
                    while (tempRow != -1)
                    {
                        if (_arrSpriteIcon[col, tempRow] != null)
                        {
                            BlockAni tempcs = _arrSpriteIcon[col, tempRow].GetComponent<BlockAni>();
                            int curId = tempcs._picidx;
                            if (curId == checkPicId && tempcs._bCheckRow == false)
                            {
                                if (tempcs._curState == BlockAni.BlockState.CountDown)
                                {
                                    //isRowHaveDeling = true;
                                    delingRowIdx = tempcs._uniqueId;
                                }
                                tempRowList.Add(_arrSpriteIcon[col, tempRow]);
                                tempcs._bCheckRow = true;
                                checklist.Add(_arrSpriteIcon[col, tempRow]);                               
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }

                        tempRow--;
                    }

                    bool isColNumFit = false;
                    bool isRowNumFit = false;
                   
                    if (tempColList.Count >= _curNeedComboNum - 1)
                    {
                        isColNumFit = true;
                    }

                    if (tempRowList.Count >= _curNeedComboNum - 1)
                    {
                        isRowNumFit = true;
                    }

                    var temp = GameConfig.createStruct();

                    if (isColNumFit && !isRowNumFit)
                    {
                        if (delingColIdx != -1)
                        {
                            temp = getWaitingStructById(delingColIdx);
                            for (int n = 0; n < tempColList.Count; n++)
                            {
                                temp.pushWaitingDelObject(tempColList[n]);
                            }

                            temp.pushWaitingDelObject(_arrSpriteIcon[col, row]);
                        }
                        else
                        {
                            for (int n = 0; n < tempColList.Count; n++)
                            {
                                temp.pushWaitingDelObject(tempColList[n]);
                            }

                            temp.pushWaitingDelObject(_arrSpriteIcon[col, row]);
                            addWaitingForDelStruct(temp);
                        }                    
                    }

                    else if (isRowNumFit && !isColNumFit)
                    {
                        if (delingRowIdx != -1)
                        {
                            temp = getWaitingStructById(delingRowIdx);
                            for (int n = 0; n < tempRowList.Count; n++)
                            {
                                temp.pushWaitingDelObject(tempRowList[n]);
                            }


                            temp.pushWaitingDelObject(_arrSpriteIcon[col, row]);
                        }
                        else
                        {
                            for (int n = 0; n < tempRowList.Count; n++)
                            {
                                temp.pushWaitingDelObject(tempRowList[n]);
                            }


                            temp.pushWaitingDelObject(_arrSpriteIcon[col, row]);
                            addWaitingForDelStruct(temp);
                        }
                       
                    }
                    else if (isRowNumFit && isColNumFit)
                    {
                        if (delingRowIdx != -1)
                        {
                            temp = getWaitingStructById(delingRowIdx);
                        }
                        else if (delingColIdx != -1)
                        {
                            temp = getWaitingStructById(delingColIdx);
                        }
                        else
                        {
                            addWaitingForDelStruct(temp);
                        }

                        for (int n = 0; n < tempRowList.Count; n++)
                        {
                            temp.pushWaitingDelObject(tempRowList[n]);
                        }

                        for (int n = 0; n < tempColList.Count; n++)
                        {
                            temp.pushWaitingDelObject(tempColList[n]);
                        }

                        temp.pushWaitingDelObject(_arrSpriteIcon[col, row]);
                       
                    }
                }
            }
        }

        for (int i = 0; i < checklist.Count; i++)
        {
            checklist[i].GetComponent<BlockAni>()._bCheckCol = false;
            checklist[i].GetComponent<BlockAni>()._bCheckRow = false;
        }

        checklist.Clear();
    }

   public WaitingForDelStruct getWaitingStructById(int id)
    {
        int count = _arrNeedDelList.Count;
        int findnum = 0;
        WaitingForDelStruct temp = null;
        for (int i = 0; i < count; i++)
        {
            if (id == _arrNeedDelList[i]._uniqueId)
            {
                temp = _arrNeedDelList[i];
                findnum++;
                if (findnum >= 2)
                {
                    Debug.Break();
                }
            }
                
        }

        return temp;
    }


    public void removeWaitingStructById(int id)
    {        
        for (int i = _arrNeedDelList.Count - 1; i >= 0; i--)
        {
            if(_arrNeedDelList[i]._uniqueId == id)
                _arrNeedDelList.RemoveAt(i);
        }
    }
}


