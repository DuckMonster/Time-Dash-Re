using System;

public class Timer
{
	float timer, timerMax;
	public bool realTimer = false;

	public Timer(float currentTimer, bool startFinished)
	{
		this.timerMax = currentTimer;
		timer = startFinished ? 0 : timerMax;
	}

	public bool IsDone
	{
		get
		{
			return timer <= 0;
		}
		set
		{
			if (value) timer = 0;
		}
	}

	public float PercentageDone
	{
		get
		{
			return Math.Min(1f, 1f - (timer / timerMax));
		}
		set
		{
			timer = timerMax * (1-value);
		}
	}

	public float TimerLength
	{
		get
		{
			return timerMax;
		}
	}

	public int MinutesLeft
	{
		get
		{
			return Math.Max(0, (int)Math.Floor(timer / 60f));
		}
	}

	public int SecondsLeft
	{
		get
		{
			return Math.Max(0, (int)Math.Floor(timer));
		}
	}

	public void Reset()
	{
		timer = timerMax;
	}

	public void Reset(float max)
	{
		timerMax = max;
		Reset();
	}

	public void Logic()
	{
		if (IsDone) return;

		timer -= realTimer ? Game.delta : Game.delta;
	}

	public void Logic(float factor)
	{
		if (IsDone) return;

		timer -= (realTimer ? Game.delta : Game.delta) * factor;
	}
}
