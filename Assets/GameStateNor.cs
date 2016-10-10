using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameStateNor : BaseGameState {
    public override void enterState()
    {
       
    }

    public override void handleInput(GameInputEvent inputevent, int configidx)
    {
        Debug.Log(inputevent);
        GameStateMove move = _mainGame.GetComponent<GameStateMove>();
        move._curInputEvent = inputevent;
        move._moveConfig = configidx;
        _mainGame.GetComponent<MainGame>().changeStage(MainGame.GameStage.MOVE);
    }

    public override void leaveState()
    {
        
    }

    public override void StateUpdate()
    {   
        if(!_isStart)
        {
            return;
        }

        _mainGame.GetComponent<MainGame>().updatePos();
    }

    // Use this for initialization
    void Start () {
	    _mainGame = GameObject.FindGameObjectWithTag("MainCamera");
        _isStart = true;
    }
}
