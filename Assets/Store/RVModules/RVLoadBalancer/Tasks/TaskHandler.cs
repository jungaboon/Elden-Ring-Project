// Created by Ronis Vision. All rights reserved
// 06.10.2020.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RVModules.RVLoadBalancer.Tasks
{
    /// <summary>
    /// Manages tasks, keep track of actively running tasks and provides useful management with tasks priority
    ///
    /// Layers system: if task has layer, no other task can run on the same layer at the same time, if layer is null it's ignored and you can have as many
    /// running tasks as defined by maxAllowed Tasks
    ///
    /// Queue system: if there is layer or maxAllowedRunningTasks conflict, task will be put into queue and will automatically run when its possible
    /// if maxAllowedQueue is reached losing task will be discarded (finished and removed from queue)
    ///
    /// ConflictBehaviour: 'conflict' is situation when you try to schedule more than one task on one layer or if reach maxRunningTasks or maxQueuedTasks.
    /// You can set three behaviours for conflict: priority means task with higher priority will 'win', OldWins means in case of conflict new task always will be
    /// put into queue or discarded, with NewWins, already running task with lowest priority will be put into queue. 
    /// </summary>
    [Serializable] public partial class TaskHandler
    {
        public enum ConflictBehaviour
        {
            Priority,
            OldWins,
            NewWins
        }

        #region Fields

        private int maxRunningTasks = 1;
        private int minimumTaskPriority;
        private List<ILoadBalancedTask> runningTasks = new List<ILoadBalancedTask>();
        private Dictionary<ILoadBalancedTask, LoadBalancerConfig> queue = new Dictionary<ILoadBalancedTask, LoadBalancerConfig>();
        private int maxQueuedTasks = 999;

        [SerializeField]
        private ConflictBehaviour conflictBehaviour = ConflictBehaviour.Priority;

        private Action<ILoadBalancedTask> onTaskFinished;

        #endregion

        #region Properties

        /// <summary>
        /// Returns tasks waiting queue
        /// </summary>
        public IEnumerable<ILoadBalancedTask> Queue => queue.Keys;

        public int QueueLength => queue.Count;
        public int RunningTasksCount => runningTasks.Count;

        /// <summary>
        /// Returns copy of currently running tasks
        /// </summary>
        public IEnumerable<ILoadBalancedTask> RunningTasks => runningTasks;

        /// <summary>
        /// Returns true if there are any active tasks
        /// </summary>
        public bool IsBusy => runningTasks.Count > 0;

        /// <summary>
        /// Returns running task with the highest priority
        /// Null if there are none running tasks
        /// </summary>
        public ILoadBalancedTask HighestPriorityRunningTask => HighestPriorityTask(runningTasks);

        /// <summary>
        /// Returns running task with the lowest priority
        /// Null if there are none running tasks
        /// </summary>
        public ILoadBalancedTask LowestPriorityRunningTask => LowestPriorityTask(runningTasks);

        /// <summary>
        /// Returns queued task with the lowest priority
        /// Null if there are none queued tasks
        /// </summary>
        public ILoadBalancedTask LowestPriorityQueuedTask => LowestPriorityTask(queue.Keys);

        /// <summary>
        /// Returns queued task with the highest priority
        /// Null if there are none queued tasks
        /// </summary>
        public ILoadBalancedTask HighestPriorityQueuedTask => HighestPriorityTask(queue.Keys);

        /// <summary>
        /// Returns priority of running task with highest priority
        /// </summary>
        public int BusyPriority => HighestPriorityRunningTask?.Priority ?? 0;

        public ConflictBehaviour ConflictBehavior
        {
            get => conflictBehaviour;
            set => conflictBehaviour = value;
        }

        /// <summary>
        /// Maximum concurently running tasks
        /// Default value 1
        /// </summary>
        public int MaxRunningTasks
        {
            get => maxRunningTasks;
            set
            {
                maxRunningTasks = Mathf.Clamp(value, 1, int.MaxValue);
                while (runningTasks.Count > maxRunningTasks)
                {
                    var lowestPriorityRunningTask = LowestPriorityRunningTask;
                    AddToQueueOrFinish(lowestPriorityRunningTask, lowestPriorityRunningTask.BalancerConfig);
                }
            }
        }

        /// <summary>
        /// Tasks with priority below this value will be discarded without putting them into queue
        /// </summary>
        public int MinimumTaskPriority
        {
            get => minimumTaskPriority;
            set => minimumTaskPriority = Mathf.Clamp(value, 0, int.MaxValue);
        }

        /// <summary>
        /// Default value 999 
        /// </summary>
        public int MaxQueuedTasks
        {
            get => maxQueuedTasks;
            set
            {
                maxQueuedTasks = Mathf.Clamp(value, 0, int.MaxValue);
                while (queue.Count > maxQueuedTasks) queue.Remove(LowestPriorityQueuedTask);
            }
        }

        #endregion

        public TaskHandler() => onTaskFinished = TaskFinished;

        #region Public methods

        /// <summary>
        /// Schedules task with once per frame call frequency
        /// </summary>
        public bool ScheduleTask(ILoadBalancedTask _task) => ScheduleTask(_task, new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0));

        /// <summary>
        /// Schedules task that will be updated _frequency times per second
        /// </summary>
        public bool ScheduleTask(ILoadBalancedTask _task, int _frequency) =>
            ScheduleTask(_task, new LoadBalancerConfig(LoadBalancerType.XtimesPerSecond, _frequency));

        /// <summary>
        /// Starts new task, or adds it to waiting queue. If queue is full it finishes task with least priority or discards new one, depending on
        /// ConflictBehaviour configuration.
        /// Returns true if task was succesfully started, false if it was added to queue or discarded
        /// </summary>
        public bool ScheduleTask(ILoadBalancedTask _task, LoadBalancerConfig _frequency)
        {
            for (var i = 0; i < runningTasks.Count; i++)
            {
                var aiTask = runningTasks[i];
                if (aiTask.Action == _task.Action) return false;
            }

            if (queue.ContainsKey(_task)) queue.Remove(_task);

            if (_task.Priority < MinimumTaskPriority) return false;

            if (runningTasks.Count >= MaxRunningTasks)
            {
                var winner = SolveConflict(_task, LowestPriorityRunningTask, _frequency);
                return winner == _task;
            }

            if (!string.IsNullOrEmpty(_task.Layer))
            {
                if (IsLayerBusy(_task.Layer, out var busyTask))
                {
                    var winner = SolveConflict(_task, busyTask, _frequency);
                    return winner == _task;
                }

                return StartTaskInternal(_task, _frequency);
            }

            return StartTaskInternal(_task, _frequency);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsTaskInQueue(ILoadBalancedTask task) => queue.ContainsKey(task);

        /// <summary>
        /// Note that this is not the same as checking ILoadBalancedTask.IsRunning.
        /// This is in TaskHandler context, so if you pass here running task that is not handled by this TaskHandler it will return false
        /// </summary>
        public bool IsTaskRunning(ILoadBalancedTask task) => runningTasks.Contains(task);

        /// <summary>
        /// Note that this is not the same as checking ILoadBalancedTask.IsRunning.
        /// This is in TaskHandler context, so if you pass here running task that is not handled by this TaskHandler it will return false
        /// </summary>
        public bool IsTaskRunning(string taskName)
        {
            for (var i = 0; i < runningTasks.Count; i++)
            {
                var balancedTask = runningTasks[i];
                if (balancedTask.Name == taskName) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if there is any task in RunningTasks that has _layer
        /// </summary>
        public bool IsLayerBusy(string _layer, out ILoadBalancedTask _task)
        {
            _task = null;
            foreach (var loadBalancedTask in RunningTasks)
            {
                if (loadBalancedTask.Layer != _layer) continue;
                _task = loadBalancedTask;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Immediately removes task from queue. Does nothing if there is no such task in queue 
        /// </summary>
        public void RemoveFromQueue(string taskName)
        {
            ILoadBalancedTask queuedTask = null;
            foreach (var task in queue.Keys)
            {
                if (task.Name != taskName) continue;
                queuedTask = task;
                break;
            }

            if (queuedTask == null) return;
            queue.Remove(queuedTask);
        }

        /// <summary>
        /// Immediately finishes task. Does nothing if passed task is not in RunningTasks
        /// </summary>
        public void FinishTask(string taskName)
        {
            ILoadBalancedTask runningTask = null;
            for (var i = 0; i < runningTasks.Count; i++)
            {
                var task = runningTasks[i];
                if (task.Name != taskName) continue;
                runningTask = task;
                break;
            }

            if (runningTask == null) return;
            FinishTask(runningTask);
        }

        /// <summary>
        /// Immediately finishes all active tasks
        /// Note that if there were any tasks on queue, after calling this method there will still be running tasks!
        /// If you want not to run any tasks anymore use FinishAllTasks method, which first clears queue
        /// </summary>
        public void FinishAllRunningTasks()
        {
            var runTasks = new List<ILoadBalancedTask>();
            runTasks.AddRange(runningTasks);
            foreach (var loadBalancedTask in runTasks) loadBalancedTask.FinishTask();
        }

        /// <summary>
        /// Clears queue and finishes all running tasks
        /// </summary>
        public void FinishAllTasks()
        {
            ClearQueue();
            FinishAllRunningTasks();
        }

        /// <summary>
        /// Clears queue
        /// </summary>
        public void ClearQueue() => queue.Clear();

        #endregion

        #region Not public methods

        private ILoadBalancedTask LowestPriorityTask(IEnumerable<ILoadBalancedTask> loadBalancedTasks)
        {
            ILoadBalancedTask task = null;
            foreach (var activeTask in loadBalancedTasks)
                if (task == null || activeTask.Priority < task.Priority)
                    task = activeTask;

            return task;
        }

        private ILoadBalancedTask HighestPriorityTask(IEnumerable<ILoadBalancedTask> loadBalancedTasks)
        {
            ILoadBalancedTask task = null;
            foreach (var activeTask in loadBalancedTasks)
                if (task == null || activeTask.Priority > task.Priority)
                    task = activeTask;

            return task;
        }

        private void FinishTask(ILoadBalancedTask task) => task.FinishTask();

        /// <summary>
        /// conflict is when you try to start task on same layer, or when max running task was reached or when max queue was reached
        /// returns winner or null if there in no more tasks allowed (running and queue full)
        /// </summary>
        private ILoadBalancedTask SolveConflict(ILoadBalancedTask _newTask, ILoadBalancedTask oldTask, LoadBalancerConfig lbc, bool moveLoserToQueue = true)
        {
            if (_newTask == null || oldTask == null) return null;

            switch (ConflictBehavior)
            {
                case ConflictBehaviour.OldWins:
                    return StartWinnerAndQueueOrFinishLosing(oldTask, oldTask.BalancerConfig, _newTask, lbc, moveLoserToQueue);
                case ConflictBehaviour.NewWins:
                    return StartWinnerAndQueueOrFinishLosing(_newTask, lbc, oldTask, oldTask.BalancerConfig, moveLoserToQueue);
                case ConflictBehaviour.Priority:
                    ILoadBalancedTask winningTask;
                    LoadBalancerConfig winnerLbc;
                    LoadBalancerConfig loserLbc;
                    if (_newTask.Priority > oldTask.Priority)
                    {
                        winningTask = _newTask;
                        winnerLbc = lbc;
                        loserLbc = oldTask.BalancerConfig;
                    }
                    else
                    {
                        winningTask = oldTask;
                        winnerLbc = oldTask.BalancerConfig;
                        loserLbc = lbc;
                    }

                    var losingTask = winningTask == _newTask ? oldTask : _newTask;
                    return StartWinnerAndQueueOrFinishLosing(winningTask, winnerLbc, losingTask, loserLbc, moveLoserToQueue);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ILoadBalancedTask StartWinnerAndQueueOrFinishLosing(ILoadBalancedTask winner, LoadBalancerConfig winnerFrequency, ILoadBalancedTask loser,
            LoadBalancerConfig loserFrequency, bool moveLoserToQueue)
        {
            if (winner == null || loser == null) return null;

            if (moveLoserToQueue)
            {
                AddToQueueOrFinish(loser, loserFrequency);
                if (!runningTasks.Contains(winner)) ScheduleTask(winner, winnerFrequency);

                return winner;
            }

            if (queue.ContainsKey(loser))
            {
                //FinishTask(loser);
                queue.Remove(loser);
                //StartTaskInternal(winner, _frequency);
                return winner;
            }

            return null;
        }

        private void AddToQueueOrFinish(ILoadBalancedTask task, LoadBalancerConfig _frequency)
        {
            if (!TryAddToQueue(task, _frequency)) FinishTask(task);
        }

        private bool TryAddToQueue(ILoadBalancedTask task, LoadBalancerConfig _frequency)
        {
            if (queue.Count >= MaxQueuedTasks)
            {
                var winner = SolveConflict(task, LowestPriorityRunningTask, _frequency, false);
            }

            if (queue.Count < MaxQueuedTasks)
            {
                if (queue.ContainsKey(task)) return false;
                queue.Add(task, _frequency);
                return true;
            }

            return false;
        }

        private bool StartTaskInternal(ILoadBalancedTask _task, LoadBalancerConfig _frequency)
        {
            _task.OnTaskFinishInternal = onTaskFinished;
            _task.StartTask(_frequency);
            runningTasks.Add(_task);

#if UNITY_EDITOR
            UpdateDebugData();
#endif
            return true;
        }

        private void TaskFinished(ILoadBalancedTask _task)
        {
            RemoveFromTasks(_task);
            if (queue.ContainsKey(_task)) queue.Remove(_task);
            if (queue.Count > 0)
            {
                var mostImpTaskFromQueue = HighestPriorityQueuedTask;
                ScheduleTask(mostImpTaskFromQueue, mostImpTaskFromQueue.BalancerConfig);
            }
#if UNITY_EDITOR
            UpdateDebugData();
#endif
            _task.OnTaskFinishInternal = null;
        }

        private void RemoveFromTasks(ILoadBalancedTask _task) => runningTasks.Remove(_task);

        #endregion
    }
}