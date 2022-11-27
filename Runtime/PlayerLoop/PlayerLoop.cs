using System;
#if UNITY_EDITOR
using System.Collections.Generic;
#endif
using UnityEditor;
using UnityEngine;
using LowLevelPlayerLoop = UnityEngine.LowLevel;

namespace LurkingNinja.PlayerLoop
{
    public static class PlayerLoop
    {
		public static ulong Frame { get; private set; }
		
#if UNITY_EDITOR
#region EditorCallbackStorage
		private static readonly List<IEarlyUpdate> EarlyUpdates = new List<IEarlyUpdate>();
		private static readonly List<IFixedUpdate> FixedUpdates = new List<IFixedUpdate>();
		private static readonly List<IPreUpdate> PreUpdates = new List<IPreUpdate>();
		private static readonly List<IUpdate> Updates = new List<IUpdate>();
		private static readonly List<IPreLateUpdate> PreLateUpdates = new List<IPreLateUpdate>();
		private static readonly List<IPostLateUpdate> PostLateUpdates = new List<IPostLateUpdate>();
#endregion
#endif
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			Frame = 0;
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
#if UNITY_EDITOR
			CheckIntegrity(defPls);
			// Make sure we unregister before we do register.
			EditorApplication.playModeStateChanged -= OnPlayModeState;
			EditorApplication.playModeStateChanged += OnPlayModeState;
#endif
			defPls.subSystemList[UpdateType.Update.ToIndex()].updateDelegate += FrameCounter;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

#if UNITY_EDITOR
#region EditorHandling
		private static void CheckIntegrity(LowLevelPlayerLoop.PlayerLoopSystem defPls)
		{
			for(var uType = UpdateType.EarlyUpdate; uType <= UpdateType.PostLateUpdate; uType++)
				Debug.Assert(defPls.subSystemList[uType.ToIndex()].type == uType.ToType(),
					$"Fatal Error: Unity player-loop incompatible ({uType})!");
		}
		
		private static void OnPlayModeState(PlayModeStateChange state)
		{
			switch (state)
			{
				case PlayModeStateChange.EnteredEditMode:
					var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
					UnregisterAll(ref defPls);
					LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
					EditorApplication.playModeStateChanged -= OnPlayModeState;
					break;

				case PlayModeStateChange.EnteredPlayMode: break;
				case PlayModeStateChange.ExitingEditMode: break;
				case PlayModeStateChange.ExitingPlayMode: break;
				default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}

		private static void UnregisterAll(ref LowLevelPlayerLoop.PlayerLoopSystem defPls)
		{
			defPls.subSystemList[UpdateType.Update.ToIndex()].updateDelegate -= FrameCounter;
			foreach(var earlyUpdate in EarlyUpdates) defPls.subSystemList[UpdateType.EarlyUpdate.ToIndex()]
					.updateDelegate -= earlyUpdate.OnEarlyUpdate; 
			foreach(var fixedUpdate in FixedUpdates) defPls.subSystemList[UpdateType.FixedUpdate.ToIndex()]
					.updateDelegate -= fixedUpdate.OnFixedUpdate; 
			foreach(var preUpdate in PreUpdates) defPls.subSystemList[UpdateType.PreUpdate.ToIndex()]
					.updateDelegate -= preUpdate.OnPreUpdate; 
			foreach(var update in Updates) defPls.subSystemList[UpdateType.Update.ToIndex()]
					.updateDelegate -= update.OnUpdate; 
			foreach(var preLateUpdate in PreLateUpdates) defPls.subSystemList[UpdateType.PreLateUpdate.ToIndex()]
					.updateDelegate -= preLateUpdate.OnPreLateUpdate; 
			foreach(var postLateUpdate in PostLateUpdates) defPls.subSystemList[UpdateType.PostLateUpdate.ToIndex()]
					.updateDelegate -= postLateUpdate.OnPostLateUpdate; 
			EarlyUpdates.Clear();
			FixedUpdates.Clear();
			PreUpdates.Clear();
			Updates.Clear();
			PreLateUpdates.Clear();
			PostLateUpdates.Clear();
		}
#endregion
#endif
	
		private static void FrameCounter() => Frame++;
		
#region AddListeners
		public static void AddListener(IEarlyUpdate client)
		{
#if UNITY_EDITOR
			EarlyUpdates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.EarlyUpdate.ToIndex()].updateDelegate += client.OnEarlyUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void AddListener(IFixedUpdate client)
		{
#if UNITY_EDITOR
			FixedUpdates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.FixedUpdate.ToIndex()].updateDelegate += client.OnFixedUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void AddListener(IPreUpdate client)
		{
#if UNITY_EDITOR
			PreUpdates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PreUpdate.ToIndex()].updateDelegate += client.OnPreUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void AddListener(IUpdate client)
		{
#if UNITY_EDITOR
			Updates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.Update.ToIndex()].updateDelegate += client.OnUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void AddListener(IPreLateUpdate client)
		{
#if UNITY_EDITOR
			PreLateUpdates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PreLateUpdate.ToIndex()].updateDelegate += client.OnPreLateUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void AddListener(IPostLateUpdate client)
		{
#if UNITY_EDITOR
			PostLateUpdates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PostLateUpdate.ToIndex()].updateDelegate += client.OnPostLateUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}
#endregion

#region RemoveListeners
		public static void RemoveListener(IEarlyUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.EarlyUpdate.ToIndex()].updateDelegate -= client.OnEarlyUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			EarlyUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IFixedUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.FixedUpdate.ToIndex()].updateDelegate -= client.OnFixedUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			FixedUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IPreUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PreUpdate.ToIndex()].updateDelegate -= client.OnPreUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			PreUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.Update.ToIndex()].updateDelegate -= client.OnUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			Updates.Remove(client);
#endif
		}

		public static void RemoveListener(IPreLateUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PreLateUpdate.ToIndex()].updateDelegate -= client.OnPreLateUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			PreLateUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IPostLateUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PostLateUpdate.ToIndex()].updateDelegate -= client.OnPostLateUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			PostLateUpdates.Remove(client);
#endif
		}
#endregion
	}
}