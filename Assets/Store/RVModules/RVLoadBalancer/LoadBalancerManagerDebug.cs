// Created by Ronis Vision. All rights reserved
// 05.11.2019.

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVLoadBalancer
{
    public class LoadBalancerManagerDebug : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private LoadBalancerManager loadBalancerManager;

        [SerializeField]
        private string name;

        private bool destroyWhenEmpty;

        private bool createdByCode;

        [SerializeField]
        private List<LoadBalancerDebug> loadBalancers = new List<LoadBalancerDebug>();

        #endregion

        #region Not public methods

        internal void AssignLbm(LoadBalancerManager _lbm, bool _destroyWhenEmpty)
        {
            loadBalancerManager = _lbm;
            name = _lbm.name;
            destroyWhenEmpty = _destroyWhenEmpty;
            createdByCode = true;
        }

        private void Update()
        {
            if (loadBalancerManager == null) return;

            if (destroyWhenEmpty && loadBalancerManager.loadBalancersDict.Count == 0)
            {
                Destroy(this);
                return;
            }

            loadBalancers.Clear();
            foreach (var timeIntervalActionLoadBalancer in loadBalancerManager.loadBalancersDict)
            {
                var lbd = new LoadBalancerDebug();
                lbd.lbc = timeIntervalActionLoadBalancer.Key.ToString();
                foreach (var a in timeIntervalActionLoadBalancer.Value.Actions)
                    lbd.actions.Add(new ActionObjectPair(a.action.Method.Name, a.action.Target.GetType().Name, a.action.Target as Object));

                loadBalancers.Add(lbd);
            }
        }

        private void Start()
        {
            if (createdByCode) return;
            SendMessage("EnableDebug");
            Destroy(this);
        }

        #endregion
    }

    [Serializable] public class LoadBalancerDebug
    {
        #region Fields

        public string lbc;
        public List<ActionObjectPair> actions = new List<ActionObjectPair>();

        #endregion
    }

    [Serializable] public class ActionObjectPair
    {
        #region Fields

        public string info;
        public Object owner;

        public ActionObjectPair(string _method, string _className, Object _owner)
        {
            info = $"{_className}.{_method}";
            if (_owner != null) info += $" - {_owner.name}";

            owner = _owner;
        }

        #endregion
    }
}