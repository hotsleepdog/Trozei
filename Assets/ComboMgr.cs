using UnityEngine;
using System.Collections;

public class ComboMgr{
	public enum ComboMessage{
		START_COUNT_DOWN = 0,
		END_COUNT_DOWN,
		CANCLE_COUNT_DOWN
	}

	private static ComboMgr _instance = null;
	private bool _needUpdata = false;
	private float _countDownTime = GameConfig.COMBODUR;
	public MainGame _mainGame;
	public static ComboMgr getInstance(){
		if (_instance == null) {
			_instance = new ComboMgr();
			_instance._mainGame = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainGame>();
		}

		return _instance;
	}

	public void updata(float dur){
		if(_needUpdata){
			_countDownTime -= dur;
			if(_countDownTime <= 0)
			{
				_needUpdata = false;
				_mainGame._curNeedComboNum = 4;
			}
		}
	}

	public void sendMessage(ComboMessage mes){
		switch (mes) {
			case ComboMessage.START_COUNT_DOWN:
			{
			_needUpdata = false;
			}
			break;
		case ComboMessage.END_COUNT_DOWN:
		{
			_mainGame._curNeedComboNum--;
			if(_mainGame._curNeedComboNum < 2){
				_mainGame._curNeedComboNum = 2;
			}
			_needUpdata = true;
		}
			break;
				case ComboMessage.CANCLE_COUNT_DOWN:
		{
			_needUpdata = true;
		}
			break;
		}
	}

}
