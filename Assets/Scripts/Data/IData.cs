using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public abstract class IData<T> where T : IData<T>, new()
{
    protected abstract string name { get; set; }
    protected TextAsset csvFile;

    protected string[,] csvArray;

    protected static T _instance = null;
    protected static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.Init();
            }

            return _instance;
        }
    }
    protected virtual void Init()
    {
        csvFile = Resources.Load<TextAsset>("Texts/" + name) as TextAsset;
        csvArray = CSVHelper.SplitCsv(csvFile.text);

        DebugLog();
    }

    public virtual void DebugLog()
    {
        Debug.Log(name + ": size = " + csvArray.GetLength(0) + "," + csvArray.GetLength(1) + "\n" + CSVHelper.ArrayToString(csvArray));
    }

}
