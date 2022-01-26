public static class MathfExtensions 
{
    public static int Min(params int[] numbers)
    {
        if (numbers.Length < 1)
            throw new System.Exception("There must be at least a single number when comparing minimums");

        var min = numbers[0];

        foreach (int number in numbers)
        {
            if (number < min)
            {
                min = number;
            }
        }

        return min;
    }

    public static float Min(params float[] numbers)
    {
        if (numbers.Length < 1)
            throw new System.Exception("There must be at least a single number when comparing minimums");

        var min = numbers[0];

        foreach (float number in numbers)
        {
            if (number < min)
            {
                min = number;
            }
        }

        return min;
    }

    public static int Max(params int[] numbers)
    {
        if (numbers.Length < 1)
            throw new System.Exception("There must be at least a single number when comparing maximums");

        var max = numbers[0];

        foreach (int number in numbers)
        {
            if (number > max)
            {
                max = number;
            }
        }

        return max;
    }

    public static float Max(params float[] numbers)
    {
        if (numbers.Length < 1)
            throw new System.Exception("There must be at least a single number when comparing maximums");

        var max = numbers[0];

        foreach (float number in numbers)
        {
            if (number > max)
            {
                max = number;
            }
        }

        return max;
    }
}
