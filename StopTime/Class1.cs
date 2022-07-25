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
			ModHelper.Console.WriteLine("[In StopTime] :");

			ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
			var events = ModHelper.Events;
			events.OnEvent = (Action<MonoBehaviour, Events>)Delegate.Combine(events.OnEvent, new Action<MonoBehaviour, Events>(this.OnEvent));
			GlobalMessenger.AddListener("LearnLaunchCodes", new Callback(this.SaveGame));

			ModHelper.Console.WriteLine(": Disabling statue...");
			ModHelper.HarmonyHelper.EmptyMethod<MemoryUplinkTrigger>("OnTriggerEnter");

			ModHelper.Console.WriteLine(": Disabling interloper destruction...");
			ModHelper.HarmonyHelper.EmptyMethod<TempCometCollisionFix>("Update");

			ModHelper.Console.WriteLine(": Disabling starfield updates...");
			ModHelper.HarmonyHelper.EmptyMethod<StarfieldController>("Update");

			ModHelper.Console.WriteLine(": Disabling sun expansion...");
			ModHelper.HarmonyHelper.EmptyMethod<SunController>("UpdateScale");

			ModHelper.Console.WriteLine(": Disabling sun logic...");
			ModHelper.HarmonyHelper.EmptyMethod<SunController>("Update");

			ModHelper.Console.WriteLine(": Disabling sun collapse SFX...");
			ModHelper.HarmonyHelper.EmptyMethod<SunController>("OnTriggerSupernova");

			ModHelper.Console.WriteLine(": Disabling End Times music...");
			ModHelper.HarmonyHelper.EmptyMethod<GlobalMusicController>("UpdateEndTimesMusic");

			ModHelper.Console.WriteLine(": Patching GetSecondsElapsed...");
			ModHelper.HarmonyHelper.AddPrefix<TimeLoop>("GetSecondsElapsed", typeof(Patches), "SandLevelPrefix");
		}

		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			var flag = behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart;
			var flag2 = flag;
			if (flag2)
			{
				SaveGame();
				ModHelper.Console.WriteLine(": Starting time loop...");
				TimeLoop.SetTimeLoopEnabled(true);
				ModHelper.Console.WriteLine(": Setting isTimeFlowing to false...");
				TimeLoop._isTimeFlowing = false;
				ModHelper.Console.WriteLine(string.Format(": Sand-loop timescale set to {0}x", _debugTimeScale));
				ModHelper.Console.WriteLine(string.Format(": Sand-loop length set to {0} minutes.", _LoopLength));
				_isStarted = true;
			}
		}

		private void SaveGame()
		{
			var flag = PlayerData.KnowsLaunchCodes() && PlayerData.LoadLoopCount() == 1;
			if (flag)
			{
				PlayerData.SaveLoopCount(5);
				PlayerData.SaveCurrentGame();
			}
		}

		public static float GetSecondsElapsed()
		{
			return _LoopLength * 60f - 2f * Math.Abs(Time.timeSinceLevelLoad * (_debugTimeScale - 0.5f) % (_LoopLength * 60f) - _LoopLength * 30f) + _debugTimeOffset;
		}

		private static float _LoopLength = 22f;

		private static float _debugTimeScale = 1f;

		private bool _isStarted;

		private static float _debugTimeOffset;
	}
}
