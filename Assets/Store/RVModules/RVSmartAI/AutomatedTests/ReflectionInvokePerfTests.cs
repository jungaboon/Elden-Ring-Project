// Created by Ronis Vision. All rights reserved
// 01.01.2020.

using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RVModules.RVSmartAI.AutomatedTests
{
    public class ReflectionInvokePerfTests : MonoBehaviour
    {
        public int iterationCount = 1000 * 1000;

        private void Start()
        {
            //DirectCall();
            MethodInfoInvoke();
            //FastProperty();
        }

        private void DirectCall()
        {
            Test test = new Test();
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < iterationCount; i++)
            {
                test.FloatProp = 5;
            }

            Debug.Log("Direct call " + sw.ElapsedMilliseconds + "ms");
        }

        private void MethodInfoInvoke()
        {
            Test test = new Test();
            var m = Helpers.BuildCallMethodAction(test, "FloatProp");
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < iterationCount; i++)
            {
                m.Invoke(test);
            }
            Debug.Log("Method info invoke " + sw.ElapsedMilliseconds + "ms");
        }

        private void FastProperty()
        {
            Test test = new Test();
            var setter = Helpers.BuildPropertySetter(test, "FloatProp");
            var getter = Helpers.BuildPropertyGetter(test, "FloatProp");

            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < iterationCount; i++)
            {
                getter.Invoke(test);
                setter.Invoke(test, 5);
            }

            Debug.Log("FastProperty " + sw.ElapsedMilliseconds + "ms");
        }

        class Test
        {
            public float FloatProp { get; set; }

            public void Method()
            {
            }
        }
    }
}