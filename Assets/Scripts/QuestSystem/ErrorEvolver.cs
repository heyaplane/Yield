using System;
using UnityEngine;
using static ErrorType;

public class ErrorEvolver : MonoBehaviour
{
    public WaferDataSO GetNextWaferEvolution(QuestSO currentQuest, WaferDataSO currentWafer, ErrorType errorRecommendation)
    {
        if (errorRecommendation == Passing) return currentWafer;
        return (currentWafer.ErrorType, errorRecommendation) switch
        {
            (Passing, UniformFail) => currentQuest.UniformFailWafer,
            (Passing, RadialFailOutside) => currentQuest.RadialFailOutsideWafer,
            (Passing, RadialFailInside) => currentQuest.RadialFailInsideWafer,
            (UniformFail, UniformFail) => currentQuest.PassingWafer,
            (UniformFail, RadialFailOutside) => currentQuest.RadialFailInsideWafer,
            (UniformFail, RadialFailInside) => currentQuest.RadialFailOutsideWafer,
            (RadialFailOutside, UniformFail) => currentQuest.RadialFailInsideWafer,
            (RadialFailOutside, RadialFailOutside) => currentQuest.PassingWafer,
            (RadialFailOutside, RadialFailInside) => currentQuest.UniformFailWafer,
            (RadialFailInside, UniformFail) => currentQuest.RadialFailOutsideWafer,
            (RadialFailInside, RadialFailInside) => currentQuest.PassingWafer,
            (RadialFailInside, RadialFailOutside) => currentQuest.UniformFailWafer,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum ErrorType
{
    Passing, RadialFailOutside, RadialFailInside, UniformFail
}
