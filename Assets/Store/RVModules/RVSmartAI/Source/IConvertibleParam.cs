// Created by Ronis Vision. All rights reserved
// 28.03.2021.

using System;

namespace RVHonorAI
{
    /// <summary>
    /// Allows for assigning non-matching type of ScorerParams to TaskParams, as long as T is convertible to generic type of TaskParams
    /// Implementing class must have implicit or explicit operator allowing for conversion to type T or be assignable to it(via inheritance)
    /// </summary>
    /// <typeparam name="T">Type we provide conversion  to</typeparam>
    public interface IConvertibleParam<out T>
    {
        /// <summary>
        /// Cached type of implementing class
        /// </summary>
        Type MyType { get; }

        /// <summary>
        /// Conversion to T type
        /// </summary>
        T Convert();
    }
}