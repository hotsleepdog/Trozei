using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForDelStruct
{
    public List<GameObject> _waitingList = new List<GameObject>();
    public float _durTime = 0.0f;
    public bool _activity = true;
    public int _uniqueId = 0;

    public void pushWaitingDelObject(GameObject pushobject)
    {
        if (!pushobject)
            Debug.Log("arr");

        BlockAni cs = (pushobject).GetComponent<BlockAni>();
        cs._curState = (BlockAni.BlockState.WaitingDel);
        _waitingList.Add(pushobject);
    }

    public void updateTime()
    {
        _durTime -= Time.deltaTime;
        if (_activity && _durTime <= 0.0f)
        {
            foreach (GameObject temp in _waitingList)
            {
                BlockAni cs = (temp).GetComponent<BlockAni>();
                cs._curState = (BlockAni.BlockState.ShouldDel);
            }
            _activity = false;
        }
    }
}

