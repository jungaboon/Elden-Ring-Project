// Created by Ronis Vision. All rights reserved
// 06.11.2019.

namespace RVModules.RVLoadBalancer
{
    /// <summary>
    /// Updates objects every X frame
    /// </summary>
    public sealed class EveryxFramesLoadBalancer : LoadBalancer
    {
        #region Fields

        private int skippedFrames;

        #endregion

        #region Properties

        public int SkippedFrames
        {
            get { return skippedFrames; }
            set { skippedFrames = value; }
        }

        #endregion

        public EveryxFramesLoadBalancer(int _skippedFrames, bool _calculateDeltaTime, bool _useUnscaledDeltaTime = false) : base(_calculateDeltaTime,
            _useUnscaledDeltaTime)
        {
            SkippedFrames = _skippedFrames;
        }

        #region Public methods

        public override void Tick(float _deltaTime)
        {
            if (Actions.Count == 0) return;
            time += _deltaTime;

            foreach (var t in Actions)
            {
                t.callCount++;
            }

            var count = Actions.Count * (1.0f / (skippedFrames + 1.0f));

            for (var i = 0; i < count; i++)
            {
                if (indexToTick >= Actions.Count) indexToTick = 0;
                if (Actions.Count == 0) return;

                var ap = Actions[indexToTick];

                if (ap.callCount > skippedFrames)
                {
                    ap.action.Invoke(getDeltaTime());
                    ap.callCount = 0;
                }

                indexToTick++;
            }
        }

        #endregion
    }
}