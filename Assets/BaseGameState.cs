using UnityEngine;
using System.Collections;

public abstract class BaseGameState : MonoBehaviour {
    public enum GameInputEvent {
        MoveLeft = 0,
        MoveRight,
        MoveUp,
        MoveDown,
        Unknow,
    };
    public GameObject _mainGame;
    protected bool _isStart = false;
	// Use this for initialization
	void Start () {
	    
	}

    public abstract void enterState();
    public abstract void leaveState();

    public abstract void StateUpdate();

    public abstract void handleInput(GameInputEvent inputevent, int configidx);
}
