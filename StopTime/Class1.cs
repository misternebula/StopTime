using System;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using UnityEngine;

namespace StopTime
{
	public class StopTime : ModBehaviour
	{
		private void Start()
		{
			base.ModHelper.Console.WriteLine("[In StopTime] :");
			base.ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
			IModEvents events = base.ModHelper.Events;
			events.OnEvent = (Action<MonoBehaviour, Events>)Delegate.Combine(events.OnEvent, new Action<MonoBehaviour, Events>(this.OnEvent));
			GlobalMessenger.AddListener("LearnLaunchCodes", new Callback(this.SaveGame));
			base.ModHelper.Console.WriteLine(": Disabling statue...");
			base.ModHelper.HarmonyHelper.EmptyMethod<MemoryUplinkTrigger>("OnTriggerEnter");
			base.ModHelper.Console.WriteLine(": Disabling interloper destruction...");
			base.ModHelper.HarmonyHelper.EmptyMethod<TempCometCollisionFix>("Update");
			base.ModHelper.Console.WriteLine(": Disabling starfield updates...");
			base.ModHelper.HarmonyHelper.EmptyMethod<StarfieldController>("Update");
			base.ModHelper.Console.WriteLine(": Disabling sun expansion...");
			base.ModHelper.HarmonyHelper.EmptyMethod<SunController>("UpdateScale");
			base.ModHelper.Console.WriteLine(": Disabling sun logic...");
			base.ModHelper.HarmonyHelper.EmptyMethod<SunController>("Update");
			base.ModHelper.Console.WriteLine(": Disabling sun collapse SFX...");
			base.ModHelper.HarmonyHelper.EmptyMethod<SunController>("OnTriggerSupernova");
			base.ModHelper.Console.WriteLine(": Disabling End Times music...");
			base.ModHelper.HarmonyHelper.EmptyMethod<GlobalMusicController>("UpdateEndTimesMusic");
			base.ModHelper.Console.WriteLine(": Patching GetSecondsElapsed...");
			base.ModHelper.HarmonyHelper.AddPrefix<TimeLoop>("GetSecondsElapsed", typeof(Patches), "SandLevelPrefix");
		}

		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			bool flag = behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart;
			bool flag2 = flag;
			if (flag2)
			{
				this.SaveGame();
				base.ModHelper.Console.WriteLine(": Starting time loop...");
				TimeLoop.SetTimeLoopEnabled(true);
				base.ModHelper.Console.WriteLine(": Setting isTimeFlowing to false...");
				typeof(TimeLoop).GetAnyField("_isTimeFlowing").SetValue(null, false);
				base.ModHelper.Console.WriteLine(string.Format(": Sand-loop timescale set to {0}x", StopTime._debugTimeScale));
				base.ModHelper.Console.WriteLine(string.Format(": Sand-loop length set to {0} minutes.", StopTime._LoopLength));
				this._isStarted = true;
			}
		}

		private void SaveGame()
		{
			bool flag = PlayerData.KnowsLaunchCodes() && PlayerData.LoadLoopCount() == 1;
			if (flag)
			{
				PlayerData.SaveLoopCount(5);
				PlayerData.SaveCurrentGame();
			}
		}

		public static float GetSecondsElapsed()
		{
			return StopTime._LoopLength * 60f - 2f * Math.Abs(Time.timeSinceLevelLoad * (StopTime._debugTimeScale - 0.5f) % (StopTime._LoopLength * 60f) - StopTime._LoopLength * 30f) + StopTime._debugTimeOffset;
		}

		private static float _LoopLength = 22f;

		private static float _debugTimeScale = 1f;

		private bool _isStarted;

		private static float _debugTimeOffset;
	}
}
