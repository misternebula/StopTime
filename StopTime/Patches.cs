using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;

namespace StopTime
{
	public static class Patches
	{
		public static bool SandLevelPrefix(ref float __result)
		{
			__result = StopTime.GetSecondsElapsed();
			return false;
		}
	}
}
