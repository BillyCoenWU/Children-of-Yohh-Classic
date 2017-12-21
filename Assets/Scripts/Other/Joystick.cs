using InControl;
using UnityEngine;

public enum JOYSTICK_COMMAND
{
	NONE = -1,

	BUTTON_ACTION_ONE,
	BUTTON_ACTION_TWO,
	BUTTON_ACTION_THREE,
	BUTTON_ACTION_FOUR,

	LEFT_BUMPER,
	LEFT_TRIGGER,
	LEFT_STICK,

	RIGHT_BUMPER,
	RIGHT_TRIGGER,
	RIGHT_STICK,

	BUTTON_SYSTEM,
	BUTTON_START,
	BUTTON_SELECT,
	BUTTON_PAUSE,
	BUTTON_MENU,
	BUTTON_HOME,
	BUTTON_BACK,
	BUTTON_POWER,
	BUTTON_OPTIONS,
	BUTTON_SHARE,
	BUTTON_VIEW,

	TILT,
	TILT_X,
	TILT_Y,
	TILT_Z,

	TOUCH,

	AXIS_LEFT_X,
	AXIS_LEFT_Y,
	AXIS_RIGHT_X,
	AXIS_RIGHT_Y,

	BUTTON_DPAD_UP,
	BUTTON_DPAD_RIGHT,
	BUTTON_DPAD_DOWN,
	BUTTON_DPAD_LEFT,
	BUTTON_DPAD_CENTER
}

public class Joystick
{
// To Do:
// - Commands: Steam Controller Buttons / Wii U Buttons
// - Touch: PS4 / PS Vita / Wii U / Steam Controller

#region Instance

	private static Joystick s_instance = null;
	public static Joystick Instance
	{
		get
		{
			if (s_instance == null) {
				s_instance = new Joystick (2);

				s_instance.Main (true);
			
				InputManager.OnDeviceAttached += s_instance.OnDeviceAtttached;
				InputManager.OnDeviceDetached += s_instance.OnDeviceDetached;
			}
		
			return s_instance;
		}
	}

	public Joystick (int myMaxGameJoysticks)
	{
		m_maxJoysticks = myMaxGameJoysticks;
	}

#endregion

#region Variables
	
	/// <summary>
	/// The Max Number of Joysticks, Set This Value According to Your Game.
	/// </summary>
	private readonly int m_maxJoysticks = 0;
	public int maxJoysticks
	{
		get
		{
			return m_maxJoysticks;
		}
	}

	/// <summary>
	/// Array With the Game Joysticks.
	/// </summary>
	private InputDevice[] m_devices = null;
	public InputDevice[] devices
	{
		get
		{
			return m_devices;
		}
	}

	/// <summary>
	/// Array Used to Know What Joystick is Vibrating in some moment.
	/// </summary>
	private bool[] isVibrating = null;

//	private float[] 

	/// <summary>
	/// Internal Enumeration. Don't Change It!
	/// </summary>
	public enum INPUT_TYPE
	{
		NULL = -1,

		IS_PRESSED = 0,
		WAS_PRESSED = 1,
		WAS_RELEASED = 2,

		TOTAL = 3
	}

#endregion

#region Helper_Methods

	public void Populate ()
	{
		bool breakIt = false;

		for(int i = 0; i < InputManager.Devices.Count; i++)
		{
			if(!ContainsDevice(InputManager.Devices[i]))
			{
				if(DeviceIsknown(InputManager.Devices[i]))
				{
					if(DeviceIsSupportedOnThisPlatform(InputManager.Devices[i]))
					{
						if(InputManager.Devices[i].AnyButton.WasPressed)
						{
							for(int j = 0; j < m_devices.Length; j++)
							{
								if(!HasJoystick(j))
								{
									m_devices[j] = InputManager.Devices[i];
									breakIt = true;
									break;
								}
							}

							if(breakIt)
							{
								break;
							}
						}
					}
				}
			}
		}
	}

	private void Main (bool populate)
	{
		isVibrating = new bool[m_maxJoysticks];
		
		for(int i = 0; i < isVibrating.Length; i++)
		{
			isVibrating[i] = false;
		}

		if(m_maxJoysticks > 1)
		{
			m_devices = new InputDevice[m_maxJoysticks];

			if(populate)
			{
				int index = 0;
				for(int d = 0; d < InputManager.Devices.Count; d++)
				{
					if (DeviceIsknown(InputManager.Devices[d]))
					{
						m_devices[index] = InputManager.Devices[d];
						index++;

						if(index >= m_maxJoysticks)
						{
							break;
						}
					}
				}
			}
		}
	}

	private void OnDeviceAtttached (InputDevice inputDevice)
	{
		if(DeviceIsknown(inputDevice))
		{
			for(int i = 0; i < m_devices.Length; i++)
			{
				if(m_devices[i] == null)
				{
					m_devices[i] = inputDevice;
					break;
				}
			}
		}
	}

	private void OnDeviceDetached (InputDevice inputDevice)
	{
		for(int i = 0; i < m_devices.Length; i++)
		{
			if(m_devices[i] == inputDevice)
			{
				isVibrating[i] = false;
				m_devices[i] = null;
				return;
			}
		}
	}

	/// <summary>
	/// Get The Number of Joysticks (with ghosts devices) (internal use, DON'T USE IT!!!!!!!!)
	/// </summary>
	/// <returns>Joystick Devices.</returns>
	public int 	DevicesCount
	{
		get
		{
			return InputManager.Devices.Count;
		}
	}

	public int JoysticksCount ()
	{
		int count = 0;
		for(int i = 0; i < m_devices.Length; i++)
		{
			if(m_devices[i] != null)
			{
				count++;
			}
		}

		return count;
	}

	public InputDevice GetJoystick (int joystickID = 0)
	{
		return m_devices[joystickID];
	}

	public InputDevice GetActiveDevice ()
	{
		return InputManager.ActiveDevice;
	}
	
	public bool HasJoystick (int joystickID = 0)
	{
		return GetJoystick(joystickID) != null ? true : false;
	}

	//----IsKnow

	public bool DeviceIsknown (InputDevice inputDevice)
	{
		return inputDevice.IsKnown;
	}

	public bool DeviceIsknown (int joystickID = 0)
	{
		return DeviceIsknown(GetJoystick(joystickID));
	}

	//----IsKnow

	//----IsSupportedOnThisPlatform
	
	public bool DeviceIsSupportedOnThisPlatform (int joystickID = 0)
	{
		return DeviceIsSupportedOnThisPlatform(GetJoystick(joystickID));
	}

	public bool DeviceIsSupportedOnThisPlatform (InputDevice inputDevice)
	{
		return inputDevice.IsSupportedOnThisPlatform;
	}

	//----IsSupportedOnThisPlatform

	//----IsAttached

	public bool DeviceIsAttached (int joystickID = 0)
	{
		return DeviceIsAttached(GetJoystick(joystickID));
	}

	public bool DeviceIsAttached (InputDevice inputDevice)
	{
		return inputDevice.IsAttached;
	}

	//----IsAttached

	//----DeviceName

	public string DeviceName (int joystickID = 0)
	{
		return DeviceName(GetJoystick(joystickID));
	}

	public string DeviceName (InputDevice inputDevice)
	{
		return inputDevice.Name;
	}

	//----DeviceName

	//----IsEqual

	public bool IsEqual (int firstId, int secondId)
	{
		return IsEqual(GetJoystick(firstId), GetJoystick(secondId));
	}

	public bool IsEqual (InputDevice firstDevice, InputDevice secondDevice)
	{
		return (firstDevice ==  secondDevice);
	}

	//----IsEqual

	public bool DeviceIsVibrating (int joystickID = 0)
	{
		return isVibrating[joystickID];
	}

	public bool ContainsDevice (InputDevice inputDevice)
	{
		for(int i = 0; i < m_devices.Length; i++)
		{
			if(HasJoystick(i))
			{
				if(m_devices[i] == inputDevice)
				{
					return true;
				}
			}
		}

		return false;
	}

	public bool AnyDeviceInput (InputControl tempControl, INPUT_TYPE inputType)
	{
		for(int i = 0; i < m_devices.Length; i++)
		{
			if(HasJoystick(i))
			{
				for(int c = 0; c < m_devices[i].Controls.Count; c++)
				{
					if(tempControl == m_devices[i].Controls[c])
					{
						if(inputType == INPUT_TYPE.IS_PRESSED && m_devices[i].Controls[c].IsPressed)
						{
							return true;
						}
						else if(inputType == INPUT_TYPE.WAS_PRESSED && m_devices[i].Controls[c].WasPressed)
						{
							return true;
						}
						else if(inputType == INPUT_TYPE.WAS_RELEASED && m_devices[i].Controls[c].WasReleased)
						{
							return true;
						}
					}
				}
			}
		}
		
		return false;
	}

#endregion

#region Vibrate

	public void Vibrate(float left, float right, int joystickID)
	{
		if(GetJoystick(joystickID) == null)
		{
			return;
		}

		isVibrating[joystickID] = (left == 0.0f && right == 0.0f);
		
		m_devices[joystickID].Vibrate(left, right);
	}

	public void Vibrate(float left, float right)
	{
		if(GetJoystick() == null)
		{
			return;
		}

		isVibrating[0] = (left == 0.0f && right == 0.0f);

		GetActiveDevice().Vibrate(left, right);
	}
	
	public void Vibrate(float intensity, int joystickID)
	{
		if(GetJoystick(joystickID) == null)
		{
			return;
		}

		isVibrating[joystickID] = (intensity == 0.0f);
		
		m_devices[joystickID].Vibrate(intensity);
	}

	public void Vibrate(float intensity)
	{
		if(GetJoystick() == null)
		{
			return;
		}

		isVibrating[0] = (intensity == 0.0f);

		GetActiveDevice().Vibrate(intensity);
	}

	public void ResetAllVibration ()
	{
		for(int i = 0; i < m_maxJoysticks; i++)
		{
			if(m_devices[i] != null)
			{
				StopJoystickVibration(i);
			}
		}
	}

	public void StopJoystickVibration (int joystickID)
	{
		if(GetJoystick(joystickID) == null)
		{
			return;
		}

		isVibrating[joystickID] = false;
		m_devices[joystickID].StopVibration();
	}

	public void StopActiveDeviceVibration ()
	{
		if(GetActiveDevice() == null)
		{
			return;
		}

		isVibrating[0] = false;
		GetActiveDevice().StopVibration();
	}

#endregion

#region GeneralInput

	public bool GetInput (InputControlType command, int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(command), INPUT_TYPE.IS_PRESSED);
		}

		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(command).IsPressed : false;
	}

	public bool GetInput (InputControlType command)
	{
		return GetActiveDevice().GetControl(command).IsPressed;
	}

	public bool GetInputDown (InputControlType command, int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(command), INPUT_TYPE.WAS_PRESSED);
		}

		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(command).WasPressed : false;
	}

	public bool GetInputDown (InputControlType command)
	{
		return GetActiveDevice().GetControl(command).WasPressed;
	}

	public bool GetInputUp (InputControlType command, int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(command), INPUT_TYPE.WAS_RELEASED);
		}

		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(command).WasReleased : false;
	}

	public bool GetInputUp (InputControlType command)
	{
		return GetActiveDevice().GetControl(command).WasReleased;
	}

#endregion

#region Direction

	public Vector2 Direction (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].Direction.Value : Vector2.zero;
	}

	public Vector2 Direction ()
	{
		return GetActiveDevice().Direction.Value;
	}

	public float DirectionX (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].Direction.X : 0.0f;
	}

	public float DirectionX ()
	{
		return GetActiveDevice().Direction.X;
	}
	
	public float DirectionY (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].Direction.Y : 0.0f;
	}

	public float DirectionY ()
	{
		return GetActiveDevice().Direction.Y;
	}

#endregion

#region Left_Axis

	public Vector2 LeftAxis (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].LeftStick.Value : Vector2.zero;
	}

	public Vector2 LeftAxis ()
	{
		return GetActiveDevice().LeftStick.Value;
	}

	public float LeftAxisX (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].LeftStickX : 0.0f;
	}

	public float LeftAxisX ()
	{
		return GetActiveDevice().LeftStickX;
	}
	
	public float LeftAxisY (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].LeftStickY : 0.0f;
	}

	public float LeftAxisY ()
	{
		return GetActiveDevice().LeftStickY;
	}

#endregion

#region Right_Axis

	public Vector2 RightAxis (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].RightStick.Value : Vector2.zero;
	}

	public Vector2 RightAxis ()
	{
		return GetActiveDevice().RightStick.Value;
	}

	public float RightAxisX (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].RightStickX : 0.0f;
	}

	public float RightAxisX ()
	{
		return GetActiveDevice().RightStickX;
	}
	
	public float RightAxisY (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].RightStickY : 0.0f;
	}

	public float RightAxisY ()
	{
		return GetActiveDevice().RightStickY;
	}

#endregion

#region DPad
	
	public Vector2 DPad (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].DPad.Value : Vector2.zero;
	}

	public Vector2 DPad ()
	{
		return GetActiveDevice().DPad.Value;
	}

	public float DPadX (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].DPadX : 0.0f;
	}

	public float DPadX ()
	{
		return GetActiveDevice().DPadX;
	}
	
	public float DPadY (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].DPadY : 0.0f;
	}

	public float DPadY ()
	{
		return GetActiveDevice().DPadY;
	}

	/*
	public bool DPadCenter (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.d).DPad, INPUT_TYPE.IS_PRESSED);
		}

		return HasJoystick(joystickID) ? m_devices[joystickID].DPad.IsPressed : false;
	}

	public bool DPadCenter ()
	{
		return GetActiveDevice().DPad.IsPressed;
	}

	public bool DPadCenterDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().DPad, INPUT_TYPE.WAS_PRESSED);
		}

		return HasJoystick(joystickID) ? m_devices[joystickID].DPad.WasPressed : false;
	}

	public bool DPadCenterDown ()
	{
		return GetActiveDevice().DPad.WasPressed;
	}

	public bool DPadCenterUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().DPad, INPUT_TYPE.WAS_RELEASED);
		}

		return HasJoystick(joystickID) ? m_devices[joystickID].DPad.WasReleased : false;
	}

	public bool DPadCenterUp ()
	{
		return GetActiveDevice().DPad.WasReleased;
	}
	*/

	public bool DPadUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadUp), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadUp).IsPressed : false;
	}

	public bool DPadUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadUp).IsPressed;
	}
	
	public bool DPadUpDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadUp), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadUp).WasPressed : false;
	}

	public bool DPadUpDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadUp).WasPressed;
	}
	
	public bool DPadUpUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadUp), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadUp).WasReleased : false;
	}

	public bool DPadUpUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadUp).WasReleased;
	}

	public bool DPadDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadDown), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadDown).IsPressed : false;
	}

	public bool DPadDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadDown).IsPressed;
	}
	
	public bool DPadDownDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadDown), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadDown).WasPressed : false;
	}

	public bool DPadDownDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadDown).WasPressed;
	}
	
	public bool DPadDownUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadDown), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadDown).WasReleased : false;
	}

	public bool DPadDownUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadDown).WasReleased;
	}

	public bool DPadLeft (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadLeft), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadLeft).IsPressed : false;
	}

	public bool DPadLeft ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadLeft).IsPressed;
	}
	
	public bool DPadLeftDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadLeft), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadLeft).WasPressed : false;
	}

	public bool DPadLeftDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadLeft).WasPressed;
	}
	
	public bool DPadLeftUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadLeft), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadLeft).WasReleased : false;
	}

	public bool DPadLeftUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadLeft).WasReleased;
	}

	public bool DPadRight (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadRight), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadRight).IsPressed : false;
	}

	public bool DPadRight ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadRight).IsPressed;
	}
	
	public bool DPadRightDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadRight), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadRight).WasPressed : false;
	}

	public bool DPadRightDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadRight).WasPressed;
	}
	
	public bool DPadRightUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.DPadRight), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.DPadRight).WasReleased : false;
	}

	public bool DPadRightUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.DPadRight).WasReleased;
	}

#endregion

#region Right_Trigger

	public float RightTriggerValue (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.RightTrigger).Value : 0.0f;
	}

	public float RightTriggerValue ()
	{
		return GetActiveDevice().GetControl(InputControlType.RightTrigger).Value;
	}

	public bool RightTrigger (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.RightTrigger), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.RightTrigger).IsPressed : false;
	}

	public bool RightTrigger ()
	{
		return GetActiveDevice().GetControl(InputControlType.RightTrigger).IsPressed;
	}
	
	public bool RightTriggerDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.RightTrigger), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.RightTrigger).WasPressed : false;
	}

	public bool RightTriggerDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.RightTrigger).WasPressed;
	}
	
	public bool RightTriggerUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.RightTrigger), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.RightTrigger).WasReleased : false;
	}

	public bool RightTriggerUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.RightTrigger).WasReleased;
	}

#endregion

#region Left_Trigger

	public float LeftTriggerValue (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.LeftTrigger).Value : 0.0f;
	}

	public float LeftTriggerValue ()
	{
		return GetActiveDevice().GetControl(InputControlType.LeftTrigger).Value;
	}

	public bool LeftTrigger (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.LeftTrigger), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.LeftTrigger).IsPressed : false;
	}

	public bool LeftTrigger ()
	{
		return GetActiveDevice().GetControl(InputControlType.LeftTrigger).IsPressed;
	}
	
	public bool LeftTriggerDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.LeftTrigger), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.LeftTrigger).WasPressed : false;
	}

	public bool LeftTriggerDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.LeftTrigger).WasPressed;
	}
	
	public bool LeftTriggerUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.LeftTrigger), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.LeftTrigger).WasReleased : false;
	}

	public bool LeftTriggerUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.LeftTrigger).WasReleased;
	}

#endregion

#region Right_Bumper

	public bool RightBumper (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.RightBumper), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.RightBumper).IsPressed : false;
	}

	public bool RightBumper ()
	{
		return GetActiveDevice().GetControl(InputControlType.RightBumper).IsPressed;
	}
	
	public bool RightBumperDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.RightBumper), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.RightBumper).WasPressed : false;
	}

	public bool RightBumperDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.RightBumper).WasPressed;
	}
	
	public bool RightBumperUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.RightBumper), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.RightBumper).WasReleased : false;
	}

	public bool RightBumperUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.RightBumper).WasReleased;
	}

#endregion

#region Left_Bumber

	public bool LeftBumper (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.LeftBumper), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.LeftBumper).IsPressed : false;
	}

	public bool LeftBumper ()
	{
		return GetActiveDevice().GetControl(InputControlType.LeftBumper).IsPressed;
	}
	
	public bool LeftBumperDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.LeftBumper), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.LeftBumper).WasPressed : false;
	}

	public bool LeftBumperDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.LeftBumper).WasPressed;
	}
	
	public bool LeftBumperUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.LeftBumper), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.LeftBumper).WasReleased : false;
	}

	public bool LeftBumperUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.LeftBumper).WasReleased;
	}

#endregion
	
#region Right_Stick
	
	public bool RightStick (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.RightStickButton), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.RightStickButton).IsPressed : false;
	}

	public bool RightStick ()
	{
		return GetActiveDevice().GetControl(InputControlType.RightStickButton).IsPressed;
	}
	
	public bool RightStickDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.RightStickButton), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.RightStickButton).WasPressed : false;
	}

	public bool RightStickDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.RightStickButton).WasPressed;
	}
	
	public bool RightStickUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.RightStickButton), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.RightStickButton).WasReleased : false;
	}

	public bool RightStickUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.RightStickButton).WasReleased;
	}
	
#endregion

#region Left_Stick

	public bool LeftStick (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.LeftStickButton), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.LeftStickButton).IsPressed : false;
	}

	public bool LeftStick ()
	{
		return GetActiveDevice().GetControl(InputControlType.LeftStickButton).IsPressed;
	}
	
	public bool LeftStickDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.LeftStickButton), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.LeftStickButton).WasPressed : false;
	}

	public bool LeftStickDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.LeftStickButton).WasPressed;
	}
	
	public bool LeftStickUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.LeftStickButton), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.LeftStickButton).WasReleased : false;
	}

	public bool LeftStickUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.LeftStickButton).WasReleased;
	}

#endregion

#region Action_One

	public bool ActionOne (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action1), INPUT_TYPE.IS_PRESSED);
		}

		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action1).IsPressed : false;
	}

	public bool ActionOne ()
	{
		return GetActiveDevice().GetControl(InputControlType.Action1).IsPressed;
	}
	
	public bool ActionOneDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action1), INPUT_TYPE.WAS_PRESSED);
		}

		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action1).WasPressed : false;
	}

	public bool ActionOneDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Action1).WasPressed;
	}
	
	public bool ActionOneUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action1), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action1).WasReleased : false;
	}

	public bool ActionOneUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Action1).WasReleased;
	}

#endregion

#region Action_Two

	public bool ActionTwo (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action2), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action2).IsPressed : false;
	}

	public bool ActionTwo ()
	{
		return GetActiveDevice().GetControl(InputControlType.Action2).IsPressed;
	}
	
	public bool ActionTwoDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action2), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action2).WasPressed : false;
	}

	public bool ActionTwoDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Action2).WasPressed;
	}
	
	public bool ActionTwoUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action2), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action2).WasReleased : false;
	}

	public bool ActionTwoUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Action2).WasReleased;
	}

#endregion

#region Action_Three

	public bool ActionThree (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action3), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action3).IsPressed : false;
	}

	public bool ActionThree ()
	{
		return GetActiveDevice().GetControl(InputControlType.Action3).IsPressed;
	}
	
	public bool ActionThreeDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action3), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action3).WasPressed : false;
	}

	public bool ActionThreeDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Action3).WasPressed;
	}
	
	public bool ActionThreeUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action3), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action3).WasReleased : false;
	}

	public bool ActionThreeUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Action3).WasReleased;
	}

#endregion

#region Action_Four

	public bool ActionFour (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action4), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action4).IsPressed : false;
	}

	public bool ActionFour ()
	{
		return GetActiveDevice().GetControl(InputControlType.Action4).IsPressed;
	}
	
	public bool ActionFourDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action4), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action4).WasPressed : false;
	}

	public bool ActionFourDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Start).WasPressed;
	}
	
	public bool ActionFourUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Action4), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Action4).WasReleased : false;
	}

	public bool ActionFourUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Start).WasReleased;
	}

#endregion

#region Geral

	public bool GeralStart (int joystickID)
	{
		return (StartDown(joystickID) ||  PauseDown(joystickID) || OptionsDown(joystickID) || MenuDown(joystickID));
	}

#endregion

#region Start

	public bool Start (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Start), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Start).IsPressed : false;
	}

	public bool Start ()
	{
		return GetActiveDevice().GetControl(InputControlType.Start).IsPressed;
	}
	
	public bool StartDown (int joystickID)
	{
//		if(joystickID == -1)
//		{
//			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Start), INPUT_TYPE.WAS_PRESSED);
//		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Start).WasPressed : false;
	}

	public bool StartDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Start).WasPressed;
	}
	
	public bool StartUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Start), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Start).WasReleased : false;
	}

	public bool StartUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Start).WasReleased;
	}

#endregion

#region Select

	public bool Select (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Select), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Select).IsPressed : false;
	}

	public bool Select ()
	{
		return GetActiveDevice().GetControl(InputControlType.Select).IsPressed;
	}
	
	public bool SelectDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Select), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Select).WasPressed : false;
	}

	public bool SelectDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Select).WasPressed;
	}
	
	public bool SelectUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Select), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Select).WasReleased : false;
	}

	public bool SelectUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Select).WasReleased;
	}

#endregion

#region Pause

	public bool Pause (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Pause), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Pause).IsPressed : false;
	}

	public bool Pause ()
	{
		return GetActiveDevice().GetControl(InputControlType.Pause).IsPressed;
	}
	
	public bool PauseDown (int joystickID)
	{
//		if(joystickID == -1)
//		{
//			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Pause), INPUT_TYPE.WAS_PRESSED);
//		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Pause).WasPressed : false;
	}

	public bool PauseDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Pause).WasPressed;
	}
	
	public bool PauseUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Pause), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Pause).WasReleased : false;
	}

	public bool PauseUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Pause).WasReleased;
	}

#endregion

#region Home

	public bool Home (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Home), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Home).IsPressed : false;
	}

	public bool Home ()
	{
		return GetActiveDevice().GetControl(InputControlType.Home).IsPressed;
	}
	
	public bool HomeDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Home), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Home).WasPressed : false;
	}

	public bool HomeDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Home).WasPressed;
	}
	
	public bool HomeUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Home), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Home).WasReleased : false;
	}

	public bool HomeUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Home).WasReleased;
	}

#endregion

#region Back

	public bool Back (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Back), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Back).IsPressed : false;
	}

	public bool Back ()
	{
		return GetActiveDevice().GetControl(InputControlType.Back).IsPressed;
	}
	
	public bool BackDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Back), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Back).WasPressed : false;
	}

	public bool BackDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Back).WasPressed;
	}
	
	public bool BackUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Back), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Back).WasReleased : false;
	}

	public bool BackUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Back).WasReleased;
	}

#endregion

#region Menu

	public bool Menu (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Menu), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Menu).IsPressed : false;
	}

	public bool Menu ()
	{
		return GetActiveDevice().GetControl(InputControlType.Menu).IsPressed;
	}
	
	public bool MenuDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Menu), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Menu).WasPressed : false;
	}

	public bool MenuDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Menu).WasPressed;
	}
	
	public bool MenuUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Menu), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Menu).WasReleased : false;
	}

	public bool MenuUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Menu).WasReleased;
	}

#endregion

#region System

	public bool System (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.System), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.System).IsPressed : false;
	}

	public bool System ()
	{
		return GetActiveDevice().GetControl(InputControlType.System).IsPressed;
	}
	
	public bool SystemDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.System), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.System).WasPressed : false;
	}

	public bool SystemDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.System).WasPressed;
	}
	
	public bool SystemUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.System), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.System).WasReleased : false;
	}

	public bool SystemUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.System).WasReleased;
	}

#endregion

#region Power

	public bool Power (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Power), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Power).IsPressed : false;
	}

	public bool Power ()
	{
		return GetActiveDevice().GetControl(InputControlType.Power).IsPressed;
	}
	
	public bool PowerDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Power), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Power).WasPressed : false;
	}

	public bool PowerDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Power).WasPressed;
	}
	
	public bool PowerUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Power), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Power).WasReleased : false;
	}

	public bool PowerUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Power).WasReleased;
	}

#endregion

#region Options

	public bool Options (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Options), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Options).IsPressed : false;
	}

	public bool Options ()
	{
		return GetActiveDevice().GetControl(InputControlType.Options).IsPressed;
	}
	
	public bool OptionsDown (int joystickID)
	{
//		if(joystickID == -1)
//		{
//			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Options), INPUT_TYPE.WAS_PRESSED);
//		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Options).WasPressed : false;
	}

	public bool OptionsDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Options).WasPressed;
	}
	
	public bool OptionsUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Options), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Options).WasReleased : false;
	}

	public bool OptionsUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Options).WasReleased;
	}

#endregion

#region Share

	public bool Share (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Share), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Share).IsPressed : false;
	}

	public bool Share ()
	{
		return GetActiveDevice().GetControl(InputControlType.Share).IsPressed;
	}
	
	public bool ShareDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Share), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Share).WasPressed : false;
	}

	public bool ShareDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.Share).WasPressed;
	}
	
	public bool ShareUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.Share), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.Share).WasReleased : false;
	}

	public bool ShareUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.Share).WasReleased;
	}

#endregion

#region View

	public bool View (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.View), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.View).IsPressed : false;
	}

	public bool View ()
	{
		return GetActiveDevice().GetControl(InputControlType.View).IsPressed;
	}

	public bool ViewDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.View), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.View).WasPressed : false;
	}

	public bool ViewDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.View).WasPressed;
	}
	
	public bool ViewUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.View), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.View).WasReleased : false;
	}

	public bool ViewUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.View).WasReleased;
	}

#endregion

#region AnyKey

	public bool AnyKey (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().AnyButton, INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].AnyButton.IsPressed : false;
	}

	public bool AnyKey ()
	{
		return GetActiveDevice().AnyButton.IsPressed;
	}
	
	public bool AnyKeyDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().AnyButton, INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].AnyButton.WasPressed : false;
	}

	public bool AnyKeyDown ()
	{
		return GetActiveDevice().AnyButton.WasPressed;
	}
	
	public bool AnyKeyUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().AnyButton, INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].AnyButton.WasReleased : false;
	}

	public bool AnyKeyUp ()
	{
		return GetActiveDevice().AnyButton.WasReleased;
	}

#endregion

#region TouchPad

	public bool TouchPadTap (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.TouchPadButton), INPUT_TYPE.IS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.TouchPadButton).IsPressed : false;
	}

	public bool TouchPadTap ()
	{
		return GetActiveDevice().GetControl(InputControlType.TouchPadButton).IsPressed;
	}
	
	public bool TouchPadTapDown (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.TouchPadButton), INPUT_TYPE.WAS_PRESSED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.TouchPadButton).WasPressed : false;
	}

	public bool TouchPadTapDown ()
	{
		return GetActiveDevice().GetControl(InputControlType.TouchPadButton).WasPressed;
	}
	
	public bool TouchPadTapUp (int joystickID)
	{
		if(joystickID == -1)
		{
			return AnyDeviceInput (GetActiveDevice().GetControl(InputControlType.TouchPadButton), INPUT_TYPE.WAS_RELEASED);
		}
		
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.TouchPadButton).WasReleased : false;
	}

	public bool TouchPadTapUp ()
	{
		return GetActiveDevice().GetControl(InputControlType.TouchPadButton).WasReleased;
	}

	public Vector2 TouchPadAxis (int joystickID)
	{
		return HasJoystick(joystickID) ? new Vector2(TouchPadAxisX(joystickID), TouchPadAxisY(joystickID)) : Vector2.zero;
	}

	public Vector2 TouchPadAxis ()
	{
		return new Vector2(TouchPadAxisX(), TouchPadAxisY());
	}
	
	public float TouchPadAxisX (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.TouchPadXAxis).Value : 0.0f;
	}

	public float TouchPadAxisX ()
	{
		return GetActiveDevice().GetControl(InputControlType.TouchPadXAxis);
	}
	
	public float TouchPadAxisY (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.TouchPadYAxis).Value : 0.0f;
	}

	public float TouchPadAxisY ()
	{
		return GetActiveDevice().GetControl(InputControlType.TouchPadYAxis);
	}

#endregion

#region Tilt

	public float TiltX (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.TiltX).Value : 0.0f;
	}

	public float TiltX ()
	{
		return GetActiveDevice().GetControl(InputControlType.TiltX).Value;
	}
	
	public float TiltY (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.TiltY).Value : 0.0f;
	}

	public float TiltY ()
	{
		return GetActiveDevice().GetControl(InputControlType.TiltY).Value;
	}
	
	public float TiltZ (int joystickID)
	{
		return HasJoystick(joystickID) ? m_devices[joystickID].GetControl(InputControlType.TiltZ).Value : 0.0f;
	}

	public float TiltZ ()
	{
		return GetActiveDevice().GetControl(InputControlType.TiltZ).Value;
	}
	
	public Vector2 TiltXY (int joystickID)
	{
		return HasJoystick(joystickID) ? new Vector2(TiltX(joystickID), TiltY(joystickID)) : Vector2.zero;
	}

	public Vector2 TiltXY ()
	{
		return new Vector2(TiltX(), TiltY());
	}
	
	public Vector3 Tilt (int joystickID)
	{
		return HasJoystick(joystickID) ? new Vector3(TiltX(joystickID), TiltY(joystickID), TiltZ(joystickID)) : Vector3.zero;
	}

	public Vector3 Tilt()
	{
		return new Vector3(TiltX(), TiltY(), TiltZ());
	}

#endregion
}
