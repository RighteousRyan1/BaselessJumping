using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BaselessJumping.Internals.Common;
using BaselessJumping.Internals.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BaselessJumping.Internals.Audio
{
    public static class AudioModifier
    {
		internal static MethodInfo ReverbInfo
        {
			get
            {
				return typeof(SoundEffectInstance)
				.GetMethod("INTERNAL_applyReverb",
				BindingFlags.Instance | BindingFlags.NonPublic);
			}
        }
		internal static MethodInfo LowPassInfo
		{
			get
			{
				return typeof(SoundEffectInstance)
					.GetMethod("INTERNAL_applyLowPassFilter",
					BindingFlags.Instance | BindingFlags.NonPublic);
			}
		}
		internal static MethodInfo HighPassInfo
		{
			get
			{
				return typeof(SoundEffectInstance)
					.GetMethod("INTERNAL_applyHighPassFilter",
					BindingFlags.Instance | BindingFlags.NonPublic);
			}
		}
		internal static MethodInfo BandPassInfo
		{
			get
			{
				return typeof(SoundEffectInstance)
					.GetMethod("INTERNAL_applyBandPassFilter",
					BindingFlags.Instance | BindingFlags.NonPublic);
			}
		}
		public static float ApplyReverb(this SoundEffectInstance instance, ref float rvGain)
        {
			// cache these soon
			rvGain = MathHelper.Clamp(rvGain, 0f, 1f);
			ReverbInfo?.Invoke(instance, new object[] { rvGain });
			return rvGain;
		}
		public static float ApplyReverb(this SoundEffectInstance instance, float rvGain)
		{
			// cache these soon
			rvGain = MathHelper.Clamp(rvGain, 0f, 1f);
			ReverbInfo?.Invoke(instance, new object[] { rvGain });
			return rvGain;
		}
		public static float ApplyLowPassFilter(this SoundEffectInstance instance, ref float cutoff)
		{
			cutoff = MathHelper.Clamp(cutoff, 0f, 100000f);
			LowPassInfo?.Invoke(instance, new object[] { cutoff });
			return cutoff;
		}
		public static float ApplyLowPassFilter(this SoundEffectInstance instance, float cutoff)
		{
			cutoff = MathHelper.Clamp(cutoff, 0f, 100000f);
			LowPassInfo?.Invoke(instance, new object[] { cutoff });
			return cutoff;
		}
		public static float ApplyHighPassFilter(this SoundEffectInstance instance, ref float cutoff)
		{
			cutoff = MathHelper.Clamp(cutoff, 0f, 100000f);
			HighPassInfo?.Invoke(instance, new object[] { cutoff });
			return cutoff;
		}
		public static float ApplyHighPassFilter(this SoundEffectInstance instance, float cutoff)
		{
			cutoff = MathHelper.Clamp(cutoff, 0f, 100000f);
			HighPassInfo?.Invoke(instance, new object[] { cutoff });
			return cutoff;
		}
		public static float ApplyBandPassFilter(this SoundEffectInstance instance, ref float center)
		{
			center = MathHelper.Clamp(center, 0.01f, 1f);
			BandPassInfo?.Invoke(instance, new object[] { center });
			return center;
		}
		public static float ApplyBandPassFilter(this SoundEffectInstance instance, float center)
		{
			center = MathHelper.Clamp(center, -1f, 1f);
			BandPassInfo?.Invoke(instance, new object[] { center });
			return center;
		}
		public static void SafelySetPan(this SoundEffectInstance sfx, float pan)
        {
			pan = MathHelper.Clamp(pan, -1f, 1f);
			sfx.Pan = pan;
		}
		public static void SafelySetVolume(this SoundEffectInstance sfx, float volume)
		{
			volume = MathHelper.Clamp(volume, -1f, 1f);
			sfx.Volume = volume;
		}
		public static void SafelySetPitch(this SoundEffectInstance sfx, float pitch)
		{
			pitch = MathHelper.Clamp(pitch, -1f, 1f);
			sfx.Pitch = pitch;
		}

		public static float GetPanFrom(Vector2 initial, Vector2 target)
        {
			return -initial.DistanceFrom(target).X * 2 / GameUtils.WindowWidth;
        }
		public static float GetOcclusionFrom(Vector2 initial, Vector2 target, bool applyScreenCentering = true)
        {
			return applyScreenCentering ? 0.5f + initial.DistanceFrom(target).Y / GameUtils.WindowHeight
				: initial.DistanceFrom(target).Y / GameUtils.WindowHeight;
		}
		public static float GetVolumeFrom(Vector2 initial, Vector2 target)
		{
			return 1f - Math.Abs(initial.DistanceFrom(target).Length()) / GameUtils.WindowWidth;
		}
		public static float GetHighPassFrom(Vector2 initial, Vector2 target)
		{
			return MathHelper.Clamp(-initial.DistanceFrom(target).Y, 0, -initial.DistanceFrom(target).Y) / GameUtils.WindowCenter.Y;
		}
		public static float GetLowPassFrom(Vector2 initial, Vector2 target)
		{
			return MathHelper.Clamp(-initial.DistanceFrom(target).Y, 0, -initial.DistanceFrom(target).Y) / -GameUtils.WindowCenter.Y;
		}

		public static void Update()
		{

		}
	}
}