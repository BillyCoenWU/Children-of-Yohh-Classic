using InControl;
using UnityEngine;

public class Blinding
{
	#region CoY Actions

	public class CoyPlayerActions : PlayerActionSet
	{
		public PlayerAction up = null;
		public PlayerAction hold = null;
		public PlayerAction down = null;
		public PlayerAction jump = null;
		public PlayerAction left = null;
		public PlayerAction pause = null;
		public PlayerAction right = null;
		public PlayerAction select = null;
		public PlayerAction action = null;
		public PlayerAction cancel = null;
		public PlayerTwoAxisAction axis = null;

		public CoyPlayerActions ()
		{
			up = CreatePlayerAction( "Up" );

			down = CreatePlayerAction( "Down" );
			left = CreatePlayerAction( "Left" );
			jump = CreatePlayerAction( "Jump" );
			hold = CreatePlayerAction( "Hold" );

			right = CreatePlayerAction( "Right" );
			pause = CreatePlayerAction( "Pause" );

			select = CreatePlayerAction( "Select" );
			cancel = CreatePlayerAction( "Cancel" );
			action = CreatePlayerAction( "Action" );

			axis = CreateTwoAxisPlayerAction( left, right, down, up );
		}

		public static CoyPlayerActions CreatePlayerOneBindings(InputDevice device)
		{
			CoyPlayerActions playerActions = new CoyPlayerActions();

			playerActions.Device = device;

			playerActions.select.AddDefaultBinding( Key.Tab );
			playerActions.select.AddDefaultBinding( Key.Backspace );
			playerActions.select.AddDefaultBinding( InputControlType.Back );
			playerActions.select.AddDefaultBinding( InputControlType.Select );

			playerActions.pause.AddDefaultBinding( Key.Escape );
			playerActions.pause.AddDefaultBinding( Key.Return );
			playerActions.pause.AddDefaultBinding( InputControlType.Menu );
			playerActions.pause.AddDefaultBinding( InputControlType.Start );
			playerActions.pause.AddDefaultBinding( InputControlType.Pause );
			playerActions.pause.AddDefaultBinding( InputControlType.Options );

			playerActions.hold.AddDefaultBinding( Mouse.RightButton );
			playerActions.hold.AddDefaultBinding( InputControlType.RightBumper );

			playerActions.action.AddDefaultBinding( Mouse.LeftButton );
			playerActions.action.AddDefaultBinding( InputControlType.Action3 );

			playerActions.cancel.AddDefaultBinding( Mouse.MiddleButton );
			playerActions.cancel.AddDefaultBinding( InputControlType.Action2 );

			playerActions.jump.AddDefaultBinding( Key.Space );
			playerActions.jump.AddDefaultBinding( InputControlType.Action1 );

			playerActions.up.AddDefaultBinding( Key.W );
			playerActions.up.AddDefaultBinding( Key.UpArrow );
			playerActions.up.AddDefaultBinding( InputControlType.DPadUp );
			playerActions.up.AddDefaultBinding( InputControlType.LeftStickUp );

			playerActions.down.AddDefaultBinding( Key.S );
			playerActions.down.AddDefaultBinding( Key.DownArrow );
			playerActions.down.AddDefaultBinding( InputControlType.DPadDown );
			playerActions.down.AddDefaultBinding( InputControlType.LeftStickDown );

			playerActions.left.AddDefaultBinding( Key.A );
			playerActions.left.AddDefaultBinding( Key.LeftArrow );
			playerActions.left.AddDefaultBinding( InputControlType.DPadLeft );
			playerActions.left.AddDefaultBinding( InputControlType.LeftStickLeft );

			playerActions.right.AddDefaultBinding( Key.D );
			playerActions.right.AddDefaultBinding( Key.RightArrow );
			playerActions.right.AddDefaultBinding( InputControlType.DPadRight );
			playerActions.right.AddDefaultBinding( InputControlType.LeftStickRight );

			playerActions.ListenOptions.IncludeUnknownControllers = false;
			playerActions.ListenOptions.MaxAllowedBindings = 6;

			playerActions.ListenOptions.OnBindingFound = ( action, binding ) => {
				if (binding == new KeyBindingSource( Key.Escape ))
				{
					action.StopListeningForBinding();
					return false;
				}
				return true;
			};

			playerActions.ListenOptions.OnBindingAdded += ( action, binding ) => {
				Debug.Log( "Binding added... " + binding.DeviceName + ": " + binding.Name );
			};

			playerActions.ListenOptions.OnBindingRejected += ( action, binding, reason ) => {
				Debug.Log( "Binding rejected... " + reason );
			};

			return playerActions;
		}

		public static CoyPlayerActions CreateOtherPlayersBindings(InputDevice device)
		{
			CoyPlayerActions playerActions = new CoyPlayerActions();

			playerActions.Device = device;

			playerActions.select.AddDefaultBinding( InputControlType.Back );
			playerActions.select.AddDefaultBinding( InputControlType.Select );

			playerActions.pause.AddDefaultBinding( InputControlType.Menu );
			playerActions.pause.AddDefaultBinding( InputControlType.Start );
			playerActions.pause.AddDefaultBinding( InputControlType.Pause );
			playerActions.pause.AddDefaultBinding( InputControlType.Options );

			playerActions.hold.AddDefaultBinding( InputControlType.RightBumper );

			playerActions.action.AddDefaultBinding( InputControlType.Action3 );

			playerActions.cancel.AddDefaultBinding( InputControlType.Action2 );

			playerActions.jump.AddDefaultBinding( InputControlType.Action1 );

			playerActions.up.AddDefaultBinding( InputControlType.DPadUp );
			playerActions.up.AddDefaultBinding( InputControlType.LeftStickUp );

			playerActions.down.AddDefaultBinding( InputControlType.DPadDown );
			playerActions.down.AddDefaultBinding( InputControlType.LeftStickDown );

			playerActions.left.AddDefaultBinding( InputControlType.DPadLeft );
			playerActions.left.AddDefaultBinding( InputControlType.LeftStickLeft );

			playerActions.right.AddDefaultBinding( InputControlType.DPadRight );
			playerActions.right.AddDefaultBinding( InputControlType.LeftStickRight );

			playerActions.ListenOptions.IncludeUnknownControllers = false;
			playerActions.ListenOptions.MaxAllowedBindings = 4;

			playerActions.ListenOptions.OnBindingFound = ( action, binding ) => {
				if (binding == new KeyBindingSource( Key.Escape ))
				{
					action.StopListeningForBinding();
					return false;
				}
				return true;
			};

			playerActions.ListenOptions.OnBindingAdded += ( action, binding ) => {
				Debug.Log( "Binding added... " + binding.DeviceName + ": " + binding.Name );
			};

			playerActions.ListenOptions.OnBindingRejected += ( action, binding, reason ) => {
				Debug.Log( "Binding rejected... " + reason );
			};

			return playerActions;
		}
	}

	#endregion

	#region Instance

	private static Blinding s_instance = null;
	public static Blinding Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new Blinding();

				s_instance.Main();
			}

			return s_instance;
		}
	}

	#endregion

	#region Variables

	private CoyPlayerActions[] m_actions = null;
	public CoyPlayerActions[] actions
	{
		get
		{
			return m_actions;


		}
	}

	#endregion

	#region Basic Methods

	private void Main ()
	{
		m_actions = new CoyPlayerActions[Joystick.Instance.maxJoysticks];
	}

	public void CreateNewPlayerAction (int characterJoystickId)
	{
		if(characterJoystickId > 3)
		{
			Debug.LogWarning ("Seu Character Joystick id \"" + characterJoystickId + "\" é maior q o número permitido");
			return;
		}

		switch(characterJoystickId)
		{
			case 0:
				m_actions[characterJoystickId] = CoyPlayerActions.CreatePlayerOneBindings(Joystick.Instance.GetJoystick(characterJoystickId));
				break;

			default:
				m_actions[characterJoystickId] = CoyPlayerActions.CreateOtherPlayersBindings(Joystick.Instance.GetJoystick(characterJoystickId));
				break;
		}
	}

	#endregion

	#region Axis

	public Vector2 Direction (int joystickId = 0)
	{
		return m_actions[joystickId].axis.Value;
	}

	public float AxisY (int joystickId = 0)
	{
		return m_actions[joystickId].axis.Y;
	}

	public float AxisX (int joystickId = 0)
	{
		return m_actions[joystickId].axis.X;
	}

	#endregion

	#region Jump

	public bool JumpWasPressed (int joystickId = 0)
	{
		return m_actions[joystickId].jump.WasPressed;
	}

	public bool JumpIsPressed (int joystickId = 0)
	{
		return m_actions[joystickId].jump.IsPressed;
	}

	public bool JumpWasReleased (int joystickId = 0)
	{
		return m_actions[joystickId].jump.WasReleased;
	}

	#endregion

	#region Cancel

	public bool CancelWasPressed (int joystickId = 0)
	{
		return m_actions[joystickId].cancel.WasPressed;
	}

	public bool CancelIsPressed (int joystickId = 0)
	{
		return m_actions[joystickId].cancel.IsPressed;
	}

	public bool CancelWasReleased (int joystickId = 0)
	{
		return m_actions[joystickId].cancel.WasReleased;
	}

	#endregion

	#region Action

	public bool ActionWasPressed (int joystickId = 0)
	{
		return m_actions[joystickId].action.WasPressed;
	}

	public bool ActionIsPressed (int joystickId = 0)
	{
		return m_actions[joystickId].action.IsPressed;
	}

	public bool ActionWasReleased (int joystickId = 0)
	{
		return m_actions[joystickId].action.WasReleased;
	}

	#endregion

	#region Hold

	public bool HoldWasPressed (int joystickId = 0)
	{
		return m_actions[joystickId].hold.WasPressed;
	}

	public bool HoldIsPressed (int joystickId = 0)
	{
		return m_actions[joystickId].hold.IsPressed;
	}

	public bool HoldWasReleased (int joystickId = 0)
	{
		return m_actions[joystickId].hold.WasReleased;
	}

	#endregion

	#region Start

	public bool StartWasPressed (int joystickId = 0)
	{
		return m_actions[joystickId].pause.WasPressed;
	}

	public bool StartIsPressed (int joystickId = 0)
	{
		return m_actions[joystickId].pause.IsPressed;
	}

	public bool StartWasReleased (int joystickId = 0)
	{
		return m_actions[joystickId].pause.WasReleased;
	}

	#endregion

	#region Select

	public bool SelectWasPressed (int joystickId = 0)
	{
		return m_actions[joystickId].select.WasPressed;
	}

	public bool SelectIsPressed (int joystickId = 0)
	{
		return m_actions[joystickId].select.IsPressed;
	}

	public bool SelectWasReleased (int joystickId = 0)
	{
		return m_actions[joystickId].select.WasReleased;
	}

	#endregion
}
