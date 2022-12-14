using System;
using System.Collections.Generic;
using UnityPlayerLoop = UnityEngine.PlayerLoop;

namespace LurkingNinja.PlayerLoop
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
		private static readonly Dictionary<UpdateType, Type> Updates = new()
		{
				{UpdateType.EarlyUpdate, typeof(UnityPlayerLoop.EarlyUpdate)}, 
				{UpdateType.FixedUpdate, typeof(UnityPlayerLoop.FixedUpdate)},
				{UpdateType.PreUpdate, typeof(UnityPlayerLoop.PreUpdate)},
				{UpdateType.Update, typeof(UnityPlayerLoop.Update)},
				{UpdateType.PreLateUpdate, typeof(UnityPlayerLoop.PreLateUpdate)},
				{UpdateType.PostLateUpdate, typeof(UnityPlayerLoop.PostLateUpdate)}
		};

		public static Type ToType(this UpdateType plt) => Updates[plt];
		public static int ToIndex(this UpdateType plt) => (int)plt;
		public static UpdateType FromIndex(int index) => (UpdateType)index;
	}
}