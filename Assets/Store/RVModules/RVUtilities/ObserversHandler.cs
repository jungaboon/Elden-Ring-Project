// Created by Ronis Vision. All rights reserved
// 18.10.2019.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// 
    /// </summary>
    public class ObserversHandler
    {
        #region Fields

        private static ObserversHandler instance;
        public RvLogger logger;
        private List<IObserver> observers = new List<IObserver>();

        #endregion

        #region Properties

        public static ObserversHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new ObserversHandler();
                return instance;
            }
        }

        #endregion

        private ObserversHandler()
        {
            logger = new RvLogger {debugMode = false};
        }

        #region Public methods

        public void AddObserver(IObserver _observer)
        {
            logger.Log("Adding new observer to observer list: " + _observer);
            if (observers.Contains(_observer))
            {
                Debug.Log("This observer is already on list!");
                return;
            }

            observers.Add(_observer);
        }

        public void RemoveObserver(IObserver _observer)
        {
            logger.Log("Removing observer from observer list: " + _observer);
            observers.Remove(_observer);
        }

        public void Broadcast(object _event)
        {
            for (var i = 0; i < observers.Count; i++)
            {
                if (observers[i] == null)
                {
                    RemoveObserver(observers[i]);
                    continue;
                }

                if (i >= observers.Count) break;

                try
                {
                    observers[i].OnNotify(_event);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        #endregion
    }
}