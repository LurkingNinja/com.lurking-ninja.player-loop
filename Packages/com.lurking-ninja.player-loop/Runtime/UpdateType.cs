/***
 * Playerloop
 * Copyright (c) 2022-2023 Lurking Ninja.
 *
 * MIT License
 * https://github.com/LurkingNinja/com.lurking-ninja.player-loop
 */
using System;
using System.Collections.Generic;
using UnityPlayerLoop = UnityEngine.PlayerLoop;

namespace LurkingNinja.PlayerloopManagement
{
	public enum UpdateType
	{
		// DO NOT CHANGE INDEXES!
		EarlyUpdate = 2,
		FixedUpdate = 3,
		PreUpdate = 4,
		Update = 5,
		PreLateUpdate = 6,
		PostLateUpdate = 7
	}

	internal static class UpdateTypeExtension
	{
		private static readonly Dictionary<UpdateType, Type> _updates = new()
		{
				{UpdateType.EarlyUpdate, typeof(UnityPlayerLoop.EarlyUpdate)}, 
				{UpdateType.FixedUpdate, typeof(UnityPlayerLoop.FixedUpdate)},
				{UpdateType.PreUpdate, typeof(UnityPlayerLoop.PreUpdate)},
				{UpdateType.Update, typeof(UnityPlayerLoop.Update)},
				{UpdateType.PreLateUpdate, typeof(UnityPlayerLoop.PreLateUpdate)},
				{UpdateType.PostLateUpdate, typeof(UnityPlayerLoop.PostLateUpdate)}
		};

		public static Type ToType(this UpdateType plt) => _updates[plt];
		public static int ToIndex(this UpdateType plt) => (int)plt;
		public static UpdateType FromIndex(int index) => (UpdateType)index;
	}
}