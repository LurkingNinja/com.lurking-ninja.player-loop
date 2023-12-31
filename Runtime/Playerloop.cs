/***
 * Playerloop
 * Copyright (c) 2022-2023 Lurking Ninja.
 *
 * MIT License
 * https://github.com/LurkingNinja/com.lurking-ninja.player-loop
 */
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using LowLevelPlayerLoop = UnityEngine.LowLevel;

namespace LurkingNinja.PlayerloopManagement
{
    public static class Playerloop
    {
		public static ulong Frame { get; private set; }
		
#if UNITY_EDITOR
		private static readonly List<IEarlyUpdate> _earlyUpdates = new();
		private static readonly List<IFixedUpdate> _fixedUpdates = new();
		private static readonly List<IPreUpdate> _preUpdates = new();
		private static readonly List<IUpdate> _updates = new();
		private static readonly List<IPreLateUpdate> _preLateUpdates = new();
		private static readonly List<IPostLateUpdate> _postLateUpdates = new();
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
			foreach(var earlyUpdate in _earlyUpdates) defPls.subSystemList[UpdateType.EarlyUpdate.ToIndex()]
					.updateDelegate -= earlyUpdate.OnEarlyUpdate; 
			foreach(var fixedUpdate in _fixedUpdates) defPls.subSystemList[UpdateType.FixedUpdate.ToIndex()]
					.updateDelegate -= fixedUpdate.OnFixedUpdate; 
			foreach(var preUpdate in _preUpdates) defPls.subSystemList[UpdateType.PreUpdate.ToIndex()]
					.updateDelegate -= preUpdate.OnPreUpdate; 
			foreach(var update in _updates) defPls.subSystemList[UpdateType.Update.ToIndex()]
					.updateDelegate -= update.OnUpdate; 
			foreach(var preLateUpdate in _preLateUpdates) defPls.subSystemList[UpdateType.PreLateUpdate.ToIndex()]
					.updateDelegate -= preLateUpdate.OnPreLateUpdate; 
			foreach(var postLateUpdate in _postLateUpdates) defPls.subSystemList[UpdateType.PostLateUpdate.ToIndex()]
					.updateDelegate -= postLateUpdate.OnPostLateUpdate; 
			_earlyUpdates.Clear();
			_fixedUpdates.Clear();
			_preUpdates.Clear();
			_updates.Clear();
			_preLateUpdates.Clear();
			_postLateUpdates.Clear();
		}
#endif
	
		private static void FrameCounter() => Frame++;
		
		public static void AddListener(IEarlyUpdate client)
		{
#if UNITY_EDITOR
			_earlyUpdates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.EarlyUpdate.ToIndex()].updateDelegate += client.OnEarlyUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void AddListener(IFixedUpdate client)
		{
#if UNITY_EDITOR
			_fixedUpdates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.FixedUpdate.ToIndex()].updateDelegate += client.OnFixedUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void AddListener(IPreUpdate client)
		{
#if UNITY_EDITOR
			_preUpdates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PreUpdate.ToIndex()].updateDelegate += client.OnPreUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void AddListener(IUpdate client)
		{
#if UNITY_EDITOR
			_updates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.Update.ToIndex()].updateDelegate += client.OnUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void AddListener(IPreLateUpdate client)
		{
#if UNITY_EDITOR
			_preLateUpdates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PreLateUpdate.ToIndex()].updateDelegate += client.OnPreLateUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void AddListener(IPostLateUpdate client)
		{
#if UNITY_EDITOR
			_postLateUpdates.Add(client);
#endif
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PostLateUpdate.ToIndex()].updateDelegate += client.OnPostLateUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
		}

		public static void RemoveListener(IEarlyUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.EarlyUpdate.ToIndex()].updateDelegate -= client.OnEarlyUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			_earlyUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IFixedUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.FixedUpdate.ToIndex()].updateDelegate -= client.OnFixedUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			_fixedUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IPreUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PreUpdate.ToIndex()].updateDelegate -= client.OnPreUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			_preUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.Update.ToIndex()].updateDelegate -= client.OnUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			_updates.Remove(client);
#endif
		}

		public static void RemoveListener(IPreLateUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PreLateUpdate.ToIndex()].updateDelegate -= client.OnPreLateUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			_preLateUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IPostLateUpdate client)
		{
			var defPls = LowLevelPlayerLoop.PlayerLoop.GetCurrentPlayerLoop();
			defPls.subSystemList[UpdateType.PostLateUpdate.ToIndex()].updateDelegate -= client.OnPostLateUpdate;
			LowLevelPlayerLoop.PlayerLoop.SetPlayerLoop(defPls);
#if UNITY_EDITOR
			_postLateUpdates.Remove(client);
#endif
		}
	}
}