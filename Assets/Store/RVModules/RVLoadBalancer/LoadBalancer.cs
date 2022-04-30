// Created by Ronis Vision. All rights reserved
// 26.02.2021.

using System;
using System.Collections.Generic;

namespace RVModules.RVLoadBalancer
{
    public abstract class LoadBalancer
    {
        #region Fields

        protected int indexToTick;

        protected float time;

//        private IDictionary<Action<float>, ActionPair> actionToApMap = new ConcurrentDictionary<Action<float>, ActionPair>();
        private IDictionary<Action<float>, ActionPair> actionToApMap = new Dictionary<Action<float>, ActionPair>();
        protected Func<float> getDeltaTime;

        private Queue<ActionPair> actionPairPool = new Queue<ActionPair>();

        #endregion

        #region Properties

        public bool UseUnscaledDeltaTime { get; }

        public bool CalculateDeltaTime { get; }

        internal List<ActionPair> Actions { get; }

        public int ActionsCount => Actions.Count;

        #endregion

        protected LoadBalancer(bool _calculateDeltaTime, bool _useUnscaledDeltaTime)
        {
            Actions = new List<ActionPair>();
            CalculateDeltaTime = _calculateDeltaTime;
            UseUnscaledDeltaTime = _useUnscaledDeltaTime;
            if (_calculateDeltaTime)
                getDeltaTime = DeltaTime;
            else
                getDeltaTime = () => -1;
        }

        #region Public methods

        public abstract void Tick(float _deltaTime);

        public virtual void AddObject(Action<float> _action)
        {
            var ap = actionPairPool.Count > 0 ? actionPairPool.Dequeue() : new ActionPair();
            ap.action = _action;
            ap.callTime = time;

            Actions.Add(ap);
            actionToApMap.Add(_action, ap);
        }

        public virtual void RemoveObject(Action<float> _action)
        {
            var ap = actionToApMap[_action];
            Actions.Remove(ap);
            actionToApMap.Remove(_action);
            // return ap to pool
            actionPairPool.Enqueue(ap);
        }

        #endregion

        #region Not public methods

        private float DeltaTime()
        {
            var ap = Actions[indexToTick];
            var ct = ap.callTime;
            ap.callTime = time;
            return time - ct;
        }

        protected void InvokeAction()
        {
            if (Actions.Count == 0) return;
            if (indexToTick >= Actions.Count) indexToTick = 0;
            Actions[indexToTick].action(getDeltaTime());
        }

        #endregion

        internal class ActionPair
        {
            #region Fields

            public Action<float> action;
            public float callTime;
            public int callCount;

            #endregion

//            public ActionPair(Action<float> _action, float _time)
//            {
//                action = _action;
//                callTime = _time;
//            }
        }
        //ObjectPool<ActionPair> apPool = new ObjectPool<ActionPair>(ActionPairGenerator);

        //private static ActionPair ActionPairGenerator() => new ActionPair();
    }
}