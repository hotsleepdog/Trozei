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
		_newBlcokDur = 3.0f;
		_creatTargetTime = Time.time + _newBlcokDur;

		_arrAllBlock = new List<GameObject>();
		_arrSpriteIcon = new GameObject[GameConfig.GAMECOLUMN, GameConfig.GAMEROW];
		
		_curGameStage = GameStage.NOR;	
		_norGameState =  gameObject.AddComponent<GameStateNor>();
		_moveGameState = gameObject.AddComponent<GameStateMove>();

        _speedValue = GameConfig.SPEED_DOWN;
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

        if(Input.GetMouseButtonDown(0))
        {
            _curState.handleInput(BaseGameState.GameInputEvent.MoveLeft, 0);
        }
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
        if (Time.time >= _creatTargetTime && flag < 50)
        {
            int idx = Random.Range(0, 6);
            int x = Random.Range(0, GameConfig.GAMECOLUMN); ;        
            GameObject temp = Instantiate(_block);
            temp.GetComponent<BlockAni>().setPosByArrIdx(x, GameConfig.GAMEROW - 1);
            temp.GetComponent<BlockAni>()._curState = (BlockAni.BlockState.Down);           
            temp.GetComponent<BlockAni>()._lastPos = new Vector2(x, GameConfig.GAMEROW - 1);
            temp.GetComponent<BlockAni>()._picidx = idx;

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
        for (int i = 0; i < _arrAllBlock.Count; i++)
        {

            BlockAni cs = (_arrAllBlock[i]).GetComponent<BlockAni>();         
            cs.updatePos();
            Vector2 cspos = cs.getPosInArry();
            Vector2 csposlast = cs._lastPos;

           int row = (int)cspos.y;
           int col = (int)cspos.x;
           int rowlast = (int)csposlast.y;
           int collast = (int)csposlast.x;
           if (cspos.y < 0)
           {
                cs.setPosByArrIdx(col, 0);
                if(cs._curState != BlockAni.BlockState.Nor)
                {
                    Debug.Log("check < 0");
                    checkDel();
                }
                cs._curState = BlockAni.BlockState.Nor;
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
                            cs.setPosByArrIdx(col, row + 1);
                            if (cs._curState != BlockAni.BlockState.Nor)
                            {
                                Debug.Log("check down");
                                checkDel();
                            }
                           cs._curState = (BlockAni.BlockState.Nor);
                        }

                    }
                    else
                    {
                        if (rowlast == 0)
                        {
                            Debug.Log("error");
                        }
                        _arrSpriteIcon[collast, rowlast] = null;
                        _arrSpriteIcon[col, row] = _arrAllBlock[i];
                        cs._lastPos = cspos;
                    }
                }
            }
            else
            {
                 
            }
            
        }
    }


    public void checkDel()
    {
        List<GameObject> checklist = new List<GameObject>();
        for (int row = 0; row < GameConfig.GAMEROW; row++)
        {
            for(int col = 0; col < GameConfig.GAMECOLUMN; col++)
            {
                
                if (_arrSpriteIcon[col, row] != null)
                {
                    List<GameObject> tempColList = new List<GameObject>();
                    List<GameObject> tempRowList = new List<GameObject>();

                    BlockAni cs = _arrSpriteIcon[col, row].GetComponent<BlockAni>();
                    int checkPicId = cs._picidx;

                    if (cs._curState == BlockAni.BlockState.WaitingDel || cs._curState == BlockAni.BlockState.ShouldDel)
                    {
                        break;
                    }

                    if (cs._bCheckCol == false)
                    {
                        cs._bCheckCol = true;
                        tempColList.Add(_arrSpriteIcon[col, row]);
                        tempRowList.Add(_arrSpriteIcon[col, row]);
                        checklist.Add(_arrSpriteIcon[col, row]);
                    }
                    else
                    {
                        break;
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
                                tempcs._bCheckCol = true;
                                tempColList.Add(_arrSpriteIcon[tempCol, row]);
                                checklist.Add(_arrSpriteIcon[tempCol, row]);
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
                                tempcs._bCheckCol = true;
                                tempColList.Add(_arrSpriteIcon[tempCol, row]);
                                checklist.Add(_arrSpriteIcon[tempCol, row]);
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
                                tempcs._bCheckRow = true;
                                tempRowList.Add(_arrSpriteIcon[col, tempRow]);
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
                                tempcs._bCheckRow = true;
                                tempRowList.Add(_arrSpriteIcon[col, tempRow]);
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
                   
                    if (tempColList.Count >= _curNeedComboNum)
                    {
                        isColNumFit = true;
                    }

                    if (tempRowList.Count >= _curNeedComboNum)
                    {
                        isRowNumFit = true;
                    }

                    var temp = GameConfig.createStruct();

                    if (isColNumFit)
                    {
                        for (int n = 0; n < tempColList.Count; n++)
                        {
                            temp.pushWaitingDelObject(tempColList[n]);
                        }
                    }

                    if (isRowNumFit)
                    {
                        for (int n = 0; n < tempRowList.Count; n++)
                        {
                            temp.pushWaitingDelObject(tempRowList[n]);
                        }
                    }

                    if (isColNumFit || isRowNumFit)
                    {
                        _arrNeedDelList.Add(temp);
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
}


