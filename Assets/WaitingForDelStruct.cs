using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForDelStruct
{
    public List<GameObject> _waitingList = new List<GameObject>();
    public float _durTime = GameConfig.DEL_DUR;
    public bool _activity = true;
    public int _uniqueId = 0;
    public bool _needRecovery = false;

    public void pushWaitingDelObject(GameObject pushobject)
    {
        if (!_activity)
        {
            return;
        }
        if (!pushobject)
            Debug.Log("arr");

        BlockAni cs = (pushobject).GetComponent<BlockAni>();
        cs._curState = (BlockAni.BlockState.CountDown);
        cs._uniqueId = _uniqueId;
        _waitingList.Add(pushobject);
    }

    public void updateTime()
    {
        _durTime -= Time.deltaTime;
        if (_activity && _durTime <= 0.0f && _needRecovery == false)
        {
            _activity = false;
            foreach (GameObject temp in _waitingList)
            {
                BlockAni cs = (temp).GetComponent<BlockAni>();
                cs._curState = (BlockAni.BlockState.ShouldDel);
            }
            
        }
    }

    public void resetTime()
    {
        _activity = true;
        _durTime = GameConfig.DEL_DUR;
    }

    public void resetState()
    {
        foreach (GameObject temp in _waitingList)
        {
            BlockAni cs = (temp).GetComponent<BlockAni>();
            cs._curState = (BlockAni.BlockState.Stand);
        }
    }
}

