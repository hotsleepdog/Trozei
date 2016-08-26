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
    }

    public float getSpeedValue()
    {
        return _speedValue;
    }

    void Start()
    {            
        _curState = _norGameState;
        _curState.enterState();

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
            Debug.Log(touchDeltaPosition);
            if(Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y))
            {
                if(touchDeltaPosition.x > 0)
                {
                    Vector2 temptouchpos = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                    Vector3 worldpos =  Camera.main.ScreenToWorldPoint(temptouchpos);                   
                    _curState.handleInput(BaseGameState.GameInputEvent.MoveLeft, (int)getPosInArry(new Vector2(worldpos.x, worldpos.y)).y);
                }
                else
                {
                    Vector2 temptouchpos = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                    Vector3 worldpos = Camera.main.ScreenToWorldPoint(temptouchpos);
                    _curState.handleInput(BaseGameState.GameInputEvent.MoveRight, (int)getPosInArry(new Vector2(worldpos.x, worldpos.y)).y);
                }
            }
            else
            {
                if (touchDeltaPosition.y > 0)
                {
                    Vector2 temptouchpos = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                    Vector3 worldpos = Camera.main.ScreenToWorldPoint(temptouchpos);
                    _curState.handleInput(BaseGameState.GameInputEvent.MoveUp, (int)getPosInArry(new Vector2(worldpos.x, worldpos.y)).x);
                }
                else
                {
                    Vector2 temptouchpos = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                    Vector3 worldpos = Camera.main.ScreenToWorldPoint(temptouchpos);
                    _curState.handleInput(BaseGameState.GameInputEvent.MoveDown, (int)getPosInArry(new Vector2(worldpos.x, worldpos.y)).x);
                }
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            _curState.handleInput(BaseGameState.GameInputEvent.MoveLeft, 0);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _curState.handleInput(BaseGameState.GameInputEvent.MoveRight, 0);
        }
        else if (Input.GetMouseButtonDown(2))
        {
            _curState.handleInput(BaseGameState.GameInputEvent.MoveUp, 1);
        }
    }

    // Update is called once per frames
    void Update()
    {
        if (Time.time >= _creatTargetTime && flag < 40)
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
    }

    public Vector2 getPosInArry(Vector2 truepos)
    {
        float x = ((truepos.x - _startOffPos.x) / _blockSize.x);
        float y = (int)(truepos.y / _blockSize.y);
        if (truepos.y < 0)
            y -= 1;
        return new Vector2(x, y);
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
        str2 += _curGameStage;
        GUI.Label(new Rect(10, 300, 300, 800), str2);
    }

    public void updatePos()
    {
        for (int i = 0; i < _arrAllBlock.Count; i++)
        {

            BlockAni cs = (_arrAllBlock[i]).GetComponent<BlockAni>();
            if (cs.getBlockState() != BlockAni.BlockState.Move)
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
}


