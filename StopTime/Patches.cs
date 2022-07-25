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
