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
    void Awake()
    {
        _needMoveArr = new List<GameObject>();
        _mainGame = GameObject.FindGameObjectWithTag("MainCamera");
    }
	void Start () {
        _isStart = true;
    }

    public override void enterState() {
		

        GameObject[,] _arrSpriteIcon = _mainGame.GetComponent<MainGame>()._arrSpriteIcon;
        if (_curInputEvent == GameInputEvent.MoveLeft)
        {
            int totalnum = _mainGame.GetComponent<MainGame>()._arrWidth;
            int spidx = totalnum - 1;
            int cloneidx = 0;

            for (int i = 0; i < totalnum; i++)
            {
                _needMoveArr.Add(_arrSpriteIcon[i, _moveConfig]);
            }

            
            GameObject clonetemp = Instantiate(_arrSpriteIcon[cloneidx, _moveConfig]);
            clonetemp.GetComponent<BlockAni>().setPosByArrIdx(totalnum, _moveConfig);
            _needMoveArr.Add(clonetemp);
            _mainGame.GetComponent<MainGame>()._arrAllBlock.Add(clonetemp);

            _needDelGameObject = _arrSpriteIcon[cloneidx, _moveConfig];
            for (int i = 0; i < totalnum - 1; i++)
            {
                _arrSpriteIcon[i, _moveConfig] = _arrSpriteIcon[i + 1, _moveConfig];
            }
            _arrSpriteIcon[spidx, _moveConfig] = clonetemp;			
        }
        else if (_curInputEvent == GameInputEvent.MoveRight)
        {
            int totalnum = _mainGame.GetComponent<MainGame>()._arrWidth;
            int spidx = 0;
            int cloneidx = totalnum - 1;

            for (int i = 0; i < totalnum; i++)
            {
                _needMoveArr.Add(_arrSpriteIcon[i, _moveConfig]);
            }
       
            GameObject clonetemp = Instantiate(_arrSpriteIcon[cloneidx, _moveConfig]);
            clonetemp.GetComponent<BlockAni>().setPosByArrIdx(-1, _moveConfig);
            _needMoveArr.Add(clonetemp);
            _mainGame.GetComponent<MainGame>()._arrAllBlock.Add(clonetemp);

            _needDelGameObject = _arrSpriteIcon[cloneidx, _moveConfig];
            for (int i = 1; i < totalnum; i++)
            {
                _arrSpriteIcon[i, _moveConfig] = _arrSpriteIcon[i - 1, _moveConfig];
            }
            _arrSpriteIcon[spidx, _moveConfig] = clonetemp;			
        }
        else if (_curInputEvent == GameInputEvent.MoveUp)
        {
            int totalnum = _mainGame.GetComponent<MainGame>()._arrHeight;
           
            for (int i = 0; i < totalnum; i++)
            {
                if(_arrSpriteIcon[_moveConfig, i] != null)
                    _needMoveArr.Add(_arrSpriteIcon[_moveConfig, i]);
            }

            _needDelGameObject = null;

			for (int i = totalnum - 1; i > 0; i--)
            {
                _arrSpriteIcon[_moveConfig, i] = _arrSpriteIcon[_moveConfig, i - 1];
            }
            _arrSpriteIcon[_moveConfig, 0] = null;			
        }
        else if(_curInputEvent == GameInputEvent.MoveDown)
        {
            int totalnum = _mainGame.GetComponent<MainGame>()._arrHeight;

            for (int i = 0; i < totalnum; i++)
            {
                if (_arrSpriteIcon[_moveConfig, i] != null)
                    _needMoveArr.Add(_arrSpriteIcon[_moveConfig, i]);
            }

            GameObject clonetemp = Instantiate(_arrSpriteIcon[_moveConfig, 0]);
            clonetemp.GetComponent<BlockAni>().setPosByArrIdx(_moveConfig, 11);
            _needMoveArr.Add(clonetemp);
            _mainGame.GetComponent<MainGame>()._arrAllBlock.Add(clonetemp);

            _needDelGameObject = _arrSpriteIcon[_moveConfig, 0] ;

            for (int i = 0; i < totalnum - 1; i++)
            {
                _arrSpriteIcon[_moveConfig, i] = _arrSpriteIcon[_moveConfig, i+1];
            }
            _arrSpriteIcon[_moveConfig, totalnum - 1] = clonetemp;
        }
    }
    public override void leaveState() {
        _curInputEvent = GameInputEvent.Unknow;

		for (int i = 0; i < _needMoveArr.Count; i++)
			((GameObject)_needMoveArr [i]).GetComponent<BlockAni> ().setLastPosCur();

        _needMoveArr.Clear();

        if (_needDelGameObject != null)
        {
            _mainGame.GetComponent<MainGame>()._arrAllBlock.Remove(_needDelGameObject);
            Destroy(_needDelGameObject);
        }
           
    }

    public override void StateUpdate() {
        if (!_isStart)
        {
            return;
        }

        if(_endDistance < 0)
        {
            _mainGame.GetComponent<MainGame>().changeStage(MainGame.GameStage.NOR);
            return;
        }

       for (int i = 0; i < _needMoveArr.Count; i++)
        {

        }
      
       		
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
