﻿using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	/// <summary>
	/// Test sound playback. Xna sound loading won't work from ReSharper, use Program.cs instead.
	/// </summary>
	public class SoundTests : TestStarter
	{
		[IntegrationTest]
		public void PlaySoundAndDispose(Type resolver)
		{
			Start(resolver, (Content content) =>
			{ 
				var sound = content.Load<Sound>("DefaultSound");
				sound.Dispose();
			});
		}

		[IntegrationTest]
		public void PlaySoundLeft(Type resolver)
		{
			Start(resolver, (Content content) => content.Load<Sound>("DefaultSound").Play(1, -1));
		}

		[IntegrationTest]
		public void PlaySoundRightAndPitched(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				var instance = sound.CreateSoundInstance();
				instance.Panning = 1.0f;
				instance.Pitch = 2.0f;
				instance.Play();
			});
		}

		[IntegrationTest]
		public void PlaySoundInstance(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				var instance = sound.CreateSoundInstance();
				Assert.AreEqual(false, instance.IsPlaying);
				instance.Play();
				Assert.AreEqual(true, instance.IsPlaying);
			});
		}

		[IntegrationTest]
		public void PlayMultipleSoundInstances(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				var instance1 = sound.CreateSoundInstance();
				var instance2 = sound.CreateSoundInstance();
				Assert.AreEqual(false, instance1.IsPlaying);
				instance1.Play();
				Assert.AreEqual(true, instance1.IsPlaying);
				Assert.AreEqual(false, instance2.IsPlaying);
				instance2.Volume = 0.5f;
				instance2.Panning = -1.0f;
				instance2.Play();
				Assert.AreEqual(true, instance2.IsPlaying);
			});
		}

		[IntegrationTest]
		public void NumberOfPlayingInstances(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				Assert.AreEqual(0, sound.NumberOfPlayingInstances);
				sound.Play();
				Assert.AreEqual(1, sound.NumberOfPlayingInstances);
				sound.Play();
				Assert.AreEqual(2, sound.NumberOfPlayingInstances);
			});
		}

		[IntegrationTest]
		public void PlayAndStop(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				Assert.IsFalse(sound.IsAnyInstancePlaying);
				sound.Play();
				Assert.IsTrue(sound.IsAnyInstancePlaying);
				sound.StopAll();
				WaitUntilSoundStateIsUpdated();
				Assert.IsFalse(sound.IsAnyInstancePlaying);
				sound.Play();
			});
		}

		[IntegrationTest]
		public void PlayAndStopInstance(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				var instance = sound.CreateSoundInstance();
				Assert.IsFalse(sound.IsAnyInstancePlaying);
				instance.Play();
				Assert.IsTrue(sound.IsAnyInstancePlaying);
				sound.StopAll();
				WaitUntilSoundStateIsUpdated();
				Assert.IsFalse(sound.IsAnyInstancePlaying);
			});
		}

		private static void WaitUntilSoundStateIsUpdated()
		{
			System.Threading.Thread.Sleep(20);
		}

		[IntegrationTest]
		public void DisposeSoundInstance(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				var instance = sound.CreateSoundInstance();
				Assert.AreEqual(1, sound.NumberOfInstances);
				Assert.AreEqual(0, sound.NumberOfPlayingInstances);
				instance.Dispose();
				Assert.AreEqual(0, sound.NumberOfInstances);
				Assert.AreEqual(0, sound.NumberOfPlayingInstances);
			});
		}

		[IntegrationTest]
		public void DisposeSoundInstancesFromSoundClass(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				sound.CreateSoundInstance();
				sound.Play();
				Assert.AreEqual(2, sound.NumberOfInstances);
				Assert.AreEqual(1, sound.NumberOfPlayingInstances);
				sound.Dispose();
				Assert.AreEqual(0, sound.NumberOfInstances);
				Assert.AreEqual(0, sound.NumberOfPlayingInstances);
			});
		}
	}
}