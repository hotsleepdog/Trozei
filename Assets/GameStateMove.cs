using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameStateMove : BaseGameState {
    public GameInputEvent _curInputEvent;
    public int _moveConfig;

    private List<GameObject> _needMoveArr;
    private float _moveDur;
    private GameObject _needDelGameObject;
    private GameObject _needDownGameObject;
    private float _targetTime;
    private Vector3 _speed;
    private float _speedValue;
    void Awake()
    {
        _needMoveArr = new List<GameObject>();
        _mainGame = GameObject.FindGameObjectWithTag("MainCamera");
       
    }
    void Start () {
        _isStart = true;
      
        _speedValue = _mainGame.GetComponent<MainGame>().getSpeedValue();
        _moveDur = GameConfig.TROZEI_SIZE / _speedValue;
    }

    public override void enterState() {

        _targetTime = Time.time + _moveDur;
        GameObject[,] _arrSpriteIcon = _mainGame.GetComponent<MainGame>()._arrSpriteIcon;

        if (_curInputEvent == GameInputEvent.MoveLeft)
        {
            _speed.x = -_speedValue;
            _speed.y = 0.0f;

            int totalnum = GameConfig.GAMECOLUMN;
            int spidx = totalnum - 1;
            int cloneidx = 0;

            for (int i = 0; i < totalnum; i++)
            {
                if(_arrSpriteIcon[i, _moveConfig] != null)
                {                 
                    _needMoveArr.Add(_arrSpriteIcon[i, _moveConfig]);
                }              
            }

            GameObject clonetemp = null;
            if (_arrSpriteIcon[cloneidx, _moveConfig])
            {
                //_arrSpriteIcon[cloneidx, _moveConfig].GetComponent<BlockAni>()._curState = (BlockAni.BlockState.Move);
                clonetemp = Instantiate(_arrSpriteIcon[cloneidx, _moveConfig]);

                BlockAni clonecs = clonetemp.GetComponent<BlockAni>();
                BlockAni clonecs2 = _arrSpriteIcon[cloneidx, _moveConfig].GetComponent<BlockAni>();
                clonecs._picidx = clonecs2._picidx;

                clonetemp.GetComponent<BlockAni>().setPosByArrIdx(totalnum, _moveConfig);
                _needMoveArr.Add(clonetemp);
                _mainGame.GetComponent<MainGame>()._arrAllBlock.Add(clonetemp);

                clonetemp.GetComponent<BlockAni>()._lastPos = (new Vector2(totalnum - 1, _moveConfig));
               
            }

            _needDelGameObject = _arrSpriteIcon[cloneidx, _moveConfig];

            for (int i = 0; i < totalnum - 1; i++)
            {             
                _arrSpriteIcon[i, _moveConfig] = _arrSpriteIcon[i + 1, _moveConfig];
                if (_arrSpriteIcon[i, _moveConfig] != null)
                {
                    _arrSpriteIcon[i, _moveConfig].GetComponent<BlockAni>()._lastPos = (new Vector2(i, _moveConfig));
                }
            }
            _arrSpriteIcon[spidx, _moveConfig] = clonetemp;
        }
        else if (_curInputEvent == GameInputEvent.MoveRight)
        {
            _speed.x = _speedValue;
            _speed.y = 0.0f;
            int totalnum = GameConfig.GAMECOLUMN;
            int spidx = 0;
            int cloneidx = totalnum - 1;

            for (int i = 0; i < totalnum; i++)
            {
                if (_arrSpriteIcon[i, _moveConfig] != null)
                {
                    _needMoveArr.Add(_arrSpriteIcon[i, _moveConfig]);
                }
            }

            GameObject clonetemp = null;
            if (_arrSpriteIcon[cloneidx, _moveConfig])
            {
                //_arrSpriteIcon[cloneidx, _moveConfig].GetComponent<BlockAni>()._curState = (BlockAni.BlockState.Move);
                clonetemp = Instantiate(_arrSpriteIcon[cloneidx, _moveConfig]);

                BlockAni clonecs = clonetemp.GetComponent<BlockAni>();
                BlockAni clonecs2 = _arrSpriteIcon[cloneidx, _moveConfig].GetComponent<BlockAni>();
                clonecs._picidx = clonecs2._picidx;

                clonetemp.GetComponent<BlockAni>().setPosByArrIdx(-1, _moveConfig);
                _needMoveArr.Add(clonetemp);
                _mainGame.GetComponent<MainGame>()._arrAllBlock.Add(clonetemp);

                clonetemp.GetComponent<BlockAni>()._lastPos = (new Vector2(spidx, _moveConfig));

            }

            _needDelGameObject = _arrSpriteIcon[cloneidx, _moveConfig];

            for (int i = totalnum - 1; i > 0; i--)
            {
                _arrSpriteIcon[i, _moveConfig] = _arrSpriteIcon[i - 1, _moveConfig];
                if (_arrSpriteIcon[i, _moveConfig] != null)
                {
                    _arrSpriteIcon[i, _moveConfig].GetComponent<BlockAni>()._lastPos = (new Vector2(i, _moveConfig));
                }
            }
            _arrSpriteIcon[spidx, _moveConfig] = clonetemp;
        }
        else if (_curInputEvent == GameInputEvent.MoveDown)
        {
            _speed.y = -_speedValue;
            _speed.x = 0.0f;

            int totalnum = GameConfig.GAMEROW;
            int spidx = totalnum - 1;
            int cloneidx = 0;

            for (int i = 0; i < totalnum; i++)
            {
                if (_arrSpriteIcon[_moveConfig, i] != null)
                {
                    if (_arrSpriteIcon[_moveConfig, i].GetComponent<BlockAni>()._curState == BlockAni.BlockState.Stand || _arrSpriteIcon[_moveConfig, i].GetComponent<BlockAni>()._curState == BlockAni.BlockState.CountDown)
                        _needMoveArr.Add(_arrSpriteIcon[_moveConfig, i]);
                }
            }

            GameObject clonetemp = null;
            if (_arrSpriteIcon[_moveConfig, cloneidx])
            {
                //_arrSpriteIcon[_moveConfig, cloneidx].GetComponent<BlockAni>()._curState = (BlockAni.BlockState.Move);
                clonetemp = Instantiate(_arrSpriteIcon[_moveConfig, cloneidx]);

                BlockAni clonecs = clonetemp.GetComponent<BlockAni>();
                BlockAni clonecs2 = _arrSpriteIcon[_moveConfig, cloneidx].GetComponent<BlockAni>();
                clonecs._picidx = clonecs2._picidx;

                clonetemp.GetComponent<BlockAni>().setPosByArrIdx(_moveConfig, totalnum);
                _needMoveArr.Add(clonetemp);
                _mainGame.GetComponent<MainGame>()._arrAllBlock.Add(clonetemp);

                clonetemp.GetComponent<BlockAni>()._lastPos = (new Vector2(_moveConfig, spidx));

                _needDownGameObject = clonetemp;

            }

            _needDelGameObject = _arrSpriteIcon[_moveConfig, cloneidx];

            for (int i  = 0 ; i < totalnum-1; i++)
            {
                _arrSpriteIcon[_moveConfig, i] = _arrSpriteIcon[_moveConfig, i + 1];
                if (_arrSpriteIcon[_moveConfig, i] != null)
                {
                    _arrSpriteIcon[_moveConfig, i].GetComponent<BlockAni>()._lastPos = (new Vector2(_moveConfig, i));
                }
            }
            _arrSpriteIcon[_moveConfig, spidx] = clonetemp;
        }
        else if (_curInputEvent == GameInputEvent.MoveUp)
        {
            _mainGame.GetComponent<MainGame>().startProtect();
            _speed.y = _speedValue;
            _speed.x = 0.0f;

            int totalnum = GameConfig.GAMEROW;
        
            for (int i = 0; i < totalnum; i++)
            {
                if (_arrSpriteIcon[_moveConfig, i] != null)
                {
                    if (_arrSpriteIcon[_moveConfig, i].GetComponent<BlockAni>()._curState == BlockAni.BlockState.Stand || _arrSpriteIcon[_moveConfig, i].GetComponent<BlockAni>()._curState == BlockAni.BlockState.CountDown)
                        _needMoveArr.Add(_arrSpriteIcon[_moveConfig, i]);
                }
            }

            _needDelGameObject = null;

            for (int i = totalnum - 1; i > 0; i--)
            {
                _arrSpriteIcon[_moveConfig, i] = _arrSpriteIcon[_moveConfig, i - 1];
                if (_arrSpriteIcon[_moveConfig, i] != null)
                {
                    _arrSpriteIcon[_moveConfig, i].GetComponent<BlockAni>()._lastPos = (new Vector2(_moveConfig, i));
                }
            }
            _arrSpriteIcon[_moveConfig, 0] = null;
        }

        for (int i = 0; i < _needMoveArr.Count; i++)
        {
            BlockAni cs = _needMoveArr[i].GetComponent<BlockAni>();
            if (cs._curState == BlockAni.BlockState.CountDown || cs._curState == BlockAni.BlockState.ShouldDel)
            {                
              _mainGame.GetComponent<MainGame>().getWaitingStructById(cs._uniqueId).resetState();
              _mainGame.GetComponent<MainGame>().removeWaitingStructById(cs._uniqueId);              
            }

            if (_curInputEvent == GameInputEvent.MoveLeft || _curInputEvent == GameInputEvent.MoveRight)
            {
                _needMoveArr[i].GetComponent<BlockAni>()._curState = (BlockAni.BlockState.MoveLR);               
            }
            else if (_curInputEvent == GameInputEvent.MoveUp || _curInputEvent == GameInputEvent.MoveDown)
            {
                _needMoveArr[i].GetComponent<BlockAni>()._curState = (BlockAni.BlockState.MoveUpDown);                
            }
            
        }

        if (_curInputEvent == GameInputEvent.MoveLeft || _curInputEvent == GameInputEvent.MoveRight)
        {           
            if (_needDelGameObject != null)
                _needDelGameObject.GetComponent<BlockAni>()._curState = (BlockAni.BlockState.MoveLR);
        }
        else if (_curInputEvent == GameInputEvent.MoveUp || _curInputEvent == GameInputEvent.MoveDown)
        {            
            if (_needDelGameObject != null)
                _needDelGameObject.GetComponent<BlockAni>()._curState = (BlockAni.BlockState.MoveUpDown);
        }
    }
    public override void leaveState() {
        _curInputEvent = GameInputEvent.Unknow;

        if (_needDelGameObject != null)
        {
            _mainGame.GetComponent<MainGame>()._arrAllBlock.Remove(_needDelGameObject);
            _needMoveArr.Remove(_needDelGameObject);
            Destroy(_needDelGameObject);
        }

        if (_needDownGameObject != null)
        {
            _needDownGameObject.GetComponent<BlockAni>()._curState = (BlockAni.BlockState.TopDown);
        }

        for (int i = 0; i < _needMoveArr.Count; i++)
        {
            ((GameObject)_needMoveArr[i]).GetComponent<BlockAni>().setLastPosCur();
            if(_needMoveArr[i].GetComponent<BlockAni>().getPosInArry().y == GameConfig.GAMEROW - 1)
                _needMoveArr[i].GetComponent<BlockAni>()._curState = (BlockAni.BlockState.TopDown);
            else
                _needMoveArr[i].GetComponent<BlockAni>()._curState = (BlockAni.BlockState.Stand);
        }

        _needMoveArr.Clear();
    }

    public override void StateUpdate() {
        if (!_isStart)
        {
            return;
        }

        if(Time.time >= _targetTime)
        {
           _mainGame.GetComponent<MainGame>().changeStage(MainGame.GameStage.NOR);
            return;
        }

       for (int i = 0; i < _needMoveArr.Count; i++)
        {
            if (_needMoveArr[i] == null)
                Debug.Break();
            _needMoveArr[i].GetComponent<Transform>().transform.Translate(_speed * Time.deltaTime);
        }

        _mainGame.GetComponent<MainGame>().updatePos();
    }

    public override void handleInput(GameInputEvent inputevent, int configidx)
    {
       
    }

    void OnGUI()
    {
        string str = "";
        str += _curInputEvent;
        str += " ";
        str += _moveConfig;
        str += "\n";
        GUI.Label(new Rect(10, 400, 300, 800), str);
    }

    }
