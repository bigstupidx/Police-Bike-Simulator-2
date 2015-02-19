using UnityEngine;
using System.Collections;

public class GameSettings{

	public static Complexity currentComplexity = Complexity.Hard;

	public enum Complexity
	{
		Low,
		Middle,
		Hard
	}

	public static int startLives = 3;

	static float[] lowParameters = {4f,4f,1f};
	static float[] middleParameters = {3f,3f,1f};
	static float[] hardParameters = {2f,2f,2f};
	static float[][] parameters = {lowParameters,middleParameters,hardParameters};

	static float[] KMparametersChange = {1.25f,1.5f,1.75f};

	static float[] Ntimes = {120f,90f,60f};

	static int[] correctlyPoints = {100,200,300};
	static float[] factorPoints = {1f,1.25f,1.5f};
	static float[] stepForFactors = {0.25f,0.5f,1.5f};

	public static float[] GetParameters()
	{
		return parameters [(int)currentComplexity];
	}

	public static float GetKMParameter()
	{
		return KMparametersChange [(int)currentComplexity];
	}

	public static float GetNtime()
	{
		return Ntimes [(int)currentComplexity];
	}
	public static int GetPoints()
	{
		return correctlyPoints [(int)currentComplexity];
	}
	public static float GetFactor()
	{
		return factorPoints [(int)currentComplexity];
	}
	public static float GetStep()
	{
		return stepForFactors [(int)currentComplexity];
	}
}
