using System.Collections.Generic;
using DeltaEngine.Datatypes;
using SysDraw = System.Drawing;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Helper class to keep track of all touches according to their id's.
	/// </summary>
	public class TouchCollection
	{
		public TouchCollection(CursorPositionTranslater positionTranslater)
		{
			this.positionTranslater = positionTranslater;
			states = new State[MaxNumberOfTouches];
			locations = new Point[MaxNumberOfTouches];
			ids = new int[MaxNumberOfTouches];
			for (int index = 0; index < MaxNumberOfTouches; index++)
				ids[index] = -1;
		}

		internal readonly State[] states;
		internal readonly Point[] locations;
		internal readonly int[] ids;
		private const int MaxNumberOfTouches = 10;
		private readonly CursorPositionTranslater positionTranslater;

		internal void UpdateAllTouches(List<NativeTouchInput> newTouches)
		{
			UpdatePreviouslyActiveTouches(newTouches);
			ProcessNewTouches(newTouches);
		}

		internal void UpdatePreviouslyActiveTouches(List<NativeTouchInput> newTouches)
		{
			for (int index = 0; index < MaxNumberOfTouches; index++)
				if (ids[index] != -1)
					UpdateTouchBy(index, newTouches);
		}

		internal void ProcessNewTouches(List<NativeTouchInput> newTouches)
		{
			for (int index = 0; index < newTouches.Count; index++)
			{
				int freeIndex = FindIndexByIdOrGetFreeIndex(newTouches[index].Id);
				ids[freeIndex] = newTouches[index].Id;
				locations[freeIndex] = CalculateQuadraticPosition(newTouches[index].X, newTouches[index].Y);
				states[freeIndex] = State.Pressing;
			}
		}

		internal void UpdateTouchBy(int index, List<NativeTouchInput> newTouches)
		{
			int previousNewTouchesCount = newTouches.Count;
			UpdateTouchIfPreviouslyPresent(index, newTouches);
			if (previousNewTouchesCount == newTouches.Count)
				UpdateTouchStateWithoutNewData(index);
		}

		private void UpdateTouchIfPreviouslyPresent(int index, List<NativeTouchInput> newTouches)
		{
			for (int newTouchIndex = 0; newTouchIndex < newTouches.Count; newTouchIndex++)
			{
				if (newTouches[newTouchIndex].Id != ids[index])
					continue;

				NativeTouchInput newTouch = newTouches[newTouchIndex];
				newTouches.RemoveAt(newTouchIndex);
				UpdateTouchState(index, newTouch.Flags);
				locations[index] = CalculateQuadraticPosition(newTouch.X, newTouch.Y);
			}
		}

		internal void UpdateTouchStateWithoutNewData(int index)
		{
			if (states[index] == State.Releasing)
				states[index] = State.Released;
			else
				ids[index] = -1;
		}

		internal void UpdateTouchState(int touchIndex, int flags)
		{
			states[touchIndex] = states[touchIndex].UpdateOnNativePressing(IsTouchDown(flags));
		}

		internal Point CalculateQuadraticPosition(int x, int y)
		{
			var newPosition = new Point(x / Precision, y / Precision);
			return positionTranslater.FromScreenPositionToScreenSpace(newPosition);
		}

		private const float Precision = 100;

		internal int FindIndexByIdOrGetFreeIndex(int id)
		{
			for (int index = 0; index < MaxNumberOfTouches; index++)
				if (ids[index] == id)
					return index;

			for (int index = 0; index < MaxNumberOfTouches; index++)
				if (ids[index] == -1)
					return index;

			return -1;
		}

		internal static bool IsTouchDown(int flag)
		{
			return (flag & NativeTouchInput.FlagTouchDownOrMoved) != 0;
		}
	}
}