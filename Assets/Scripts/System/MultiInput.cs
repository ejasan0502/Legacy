using UnityEngine;
using System.Collections;

public class MultiInput
{
	private RuntimePlatform _platform;
	private bool _multiTouch;

	private static MultiInput _instance;

	private MultiInput() {
		_platform = Application.platform;
	}

	public static MultiInput Instance {
		get {
			if(_instance == null) 
				_instance = new MultiInput();
			return _instance;
		}
	}

	public void SetMultiTouch(bool multi) {
		_multiTouch = multi;
	}

	public Vector2 InputGUIPosition(){
		return new Vector2(Input.mousePosition.x,Screen.height-Input.mousePosition.y);
	}

	public Vector3? InputPosition() {
		if(_platform == RuntimePlatform.Android || _platform == RuntimePlatform.IPhonePlayer) {
			if(Input.touchCount > 0) {
				return Input.touches[0].position;
			} else {
				return null;
			}
		} else {
			return Input.mousePosition;
		}
	}

	public Vector3? InputPosition(int input) {
		if(_multiTouch) {
			if(_platform == RuntimePlatform.Android || _platform == RuntimePlatform.IPhonePlayer) {
				if(Input.touchCount > input) {
					return Input.touches[input].position;
				} else {
					return null;
				}
			}
		}
		return null;
	}

	public bool GetInputDown(int input) {
		if(_platform == RuntimePlatform.Android || _platform == RuntimePlatform.IPhonePlayer) {
			if(Input.touchCount > input) {
				return Input.touches[input].phase == TouchPhase.Began;
			} else {
				return false;
			}
		} else {
			return Input.GetMouseButtonDown(input);
		}
	}

	public bool GetInput(int input) {
		if(_platform == RuntimePlatform.Android || _platform == RuntimePlatform.IPhonePlayer) {
			if(Input.touchCount > input) {
				return Input.touches[input].phase == TouchPhase.Moved || Input.touches[input].phase == TouchPhase.Stationary;
			} else {
				return false;
			}
		} else {
			return Input.GetMouseButton(input);
		}
	}

	public bool GetInputUp(int input) {
		if(_platform == RuntimePlatform.Android || _platform == RuntimePlatform.IPhonePlayer) {
			if(Input.touchCount > input) {
				return Input.touches[input].phase == TouchPhase.Ended;
			} else {
				return false;
			}
		} else {
			return Input.GetMouseButtonUp(input);
		}
	}
}

