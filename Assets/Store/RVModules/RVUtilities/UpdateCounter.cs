// Created by Ronis Vision. All rights reserved
// 06.08.2020.

namespace RVModules.RVUtilities
{
    // usefull to allow set frequency for monobehaviours 
    public class UpdateCounter
    {
        #region Fields

        public float allowUpdatePerCalls;
        private int counter;

        #endregion

        public UpdateCounter(int _desiredFrequency, int _expectedCallFrequency = 60) => allowUpdatePerCalls = _expectedCallFrequency * 1.0f / _desiredFrequency;

        #region Public methods

        public bool Update()
        {
            counter++;
            if (counter >= allowUpdatePerCalls)
            {
                counter = 0;
                return true;
            }

            return false;
        }

        #endregion
    }
}