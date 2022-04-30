// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// Allows for quick and dirty dump txt files to analyse data
    /// usage:
    /// add DataLogger in scene and set path to file where you wanna save
    /// DataLogger.Log(myTransformPosition)
    /// </summary>
    public class DataLogger : MonoBehaviour
    {
        #region Fields

        private static DataLogger instance;

        [SerializeField]
        private bool logFrames, logFixedFrames, logTime, clearOnStart;
                                                    
        [SerializeField]
        private int saveRate = 3;

        [SerializeField]
        private string outputFilePath;

        [SerializeField]
        private string dataToWrite;
        
        private bool isDirty;

        private int frames;
        private int fixedFrames;
        private float time;
        private float lastTimeCall;

        #endregion

        #region Public methods

        public static void Log(string _data)
        {
            if (instance == null) return;
            instance.AddData(_data);
        }

        #endregion

        #region Not public methods

        private void Awake()
        {
            instance = this;
            if (clearOnStart) SaveFile();
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            if (isDirty) SaveFile(true);
        }

        private void SaveFile(bool _immediate = false)
        {
            if (_immediate)
            {
                File.WriteAllText(outputFilePath, dataToWrite);
                isDirty = false;
                return;
            }

            Task.Run(() =>
            {
                File.WriteAllText(outputFilePath, dataToWrite);
                isDirty = false;
            });
        }

        private void Update()
        {
            frames++;
            time = Time.time;

            if (time - lastTimeCall < saveRate) return;
            if (!isDirty) return;

            SaveFile();
            lastTimeCall = time;
        }

        private void FixedUpdate() => fixedFrames++;

        private void AddData(string _data)
        {
            if (logFrames) dataToWrite += $"f{frames}; ";
            if (logFixedFrames) dataToWrite += $"ff{fixedFrames}; ";
            if (logTime) dataToWrite += $"t{time}; ";
            dataToWrite += _data + "\n";
            isDirty = true;
        }

        #endregion
    }
}