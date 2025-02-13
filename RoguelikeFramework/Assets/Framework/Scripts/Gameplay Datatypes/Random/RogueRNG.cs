﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RogueRNG
{
    #region Continious
    //Remapping of Random.Range, for completeness
    public static float Linear(float min, float max)
    {
        return Random.Range(min, max);
    }

    //Remapping of Random.Range, for completeness
    public static int Linear(int min, int max)
    {
        return Random.Range(min, max);
    }

    //Simple exponential, with mean that matches given mean!
    //WILL NOT RETURN INFINITY I PROMISE
    public static float Exponential(float mean)
    {
        float val = Mathf.Log(1 - Random.Range(0.0f, .99999999f)) * (-1 * mean);
        if (val == Mathf.Infinity)
        {
            val = Exponential(mean);
        }
        return val;
    }

    

    //Bounded exponential, where the distribution was 'cut out' of the graph - no upscaling or correction
    //Because of this, the mean is not where you put it, because you wildly altered the distribution
    //Less useful, but mean is not tied to area min/max so you can make more interesting curves
    public static float BoundedExponentialCut(float min, float max, float mean)
    {
        float val = Mathf.Log(1 - Random.Range(1 - Mathf.Exp(-min / mean), 1 - Mathf.Exp(-max / mean))) * (-1 * mean);
        return val;
    }

    public static int BoundedExponentialCut(int min, int max, float mean)
    {
        return (int)BoundedExponentialCut((float)min, (float)max, mean);
    }
    
    //More useful version of the bounded exponential. Mean will be preserved through the cut, and distribution will still be observed
    public static float BoundedExponential(float min, float max, float mean, float precisionBound = 1000000000)
    {
        min = Mathf.Max(0, min); //Gotta be positive
        max = Mathf.Max(min + 0.5f, max); //Gotta be above min
        mean = Mathf.Clamp(mean, min, max); //Gotta be between the two

        //Convert mean to 0-1 space
        float movedMean = (mean - min) / (max - min);

        //Convert 0-1 space to 0-(very large number) space (approximating infitity)
        float bigMean = movedMean * precisionBound;
        //Generate a random number from 0-(float.MaxValue)
        float bigVal = Exponential(bigMean);

        //Slice it if it's above our precision bound - this is where we maintain our bounds, but lose accuracy as a cost
        if (bigVal > precisionBound)
        {
            bigVal = precisionBound;
        }

        //Convert val back into min-max space
        float val = ((bigVal / precisionBound) * (max - min)) + min;

        return val;
    }

    
    public static int BoundedExponential(int min, int max, float mean)
    {
        return (int)BoundedExponential((float)min, (float)max, mean);
    }

    public static float Pareto(float min, float a)
    {
        float exp = -1f / a;
        return min * Mathf.Pow(1 - Random.Range(0.0f, .99999999f), exp);
    }

    public static float Pareto(float a)
    {
        return Pareto(1f, a);
    }

    public static float BoundedParetoCut(float min, float max, float xMin, float a)
    {
        float low = 1 - Mathf.Pow(xMin / min, a);
        float high = 1 - Mathf.Pow(xMin / max, a);
        return xMin * Mathf.Pow(1 - Random.Range(low, high), -1f/a);
    }

    //Chops off the pareto bound past cutoff, and then scales to match range. Higher cutoffs make the effect more dramatic, but require less loops.
    //Making min = 1 and max=scale will match the regular pareto up to scale (mean will NOT scale, because of heavy tail)
    //Mess with this until you feel like the numbers look alright
    public static float BoundedPareto(float min, float max, float xMin = 1f, float a = 2f, float cutoff = 10)
    {
        float bigVal = Pareto(xMin, a);

        while (bigVal > cutoff) //More expensive while loop, but better numbers in general
        {
            bigVal = Pareto(xMin, a);
        }

        float val = (((bigVal - xMin) / cutoff) * (max - min)) + min;
        return val;
    }

    public static int BoundedPareto(int min, int max, float xMin = 1f, float a = 2f, float cutoff = 10)
    {
        return (int)BoundedPareto((float)min, (float)max, xMin, a, cutoff);
    }
    #endregion

    #region Discrete
    public static int Binomial(int n, float p)
    {
        int sum = 0;
        for (int c = 0; c < n; c++)
        {
            if (Random.value <= p) sum++;
        }
        return sum;
    }

    public static int Geometric(int mean, int max = 1000)
    {
        float fMean = 1f / mean;
        int sum = 1;
        for (int c = 1; c < max; c++)
        {
            if (Random.value <= fMean)
            {
                break;
            }
            sum++;
        }
        return sum;
    }
    #endregion
}
