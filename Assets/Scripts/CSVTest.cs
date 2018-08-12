using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public class CSVTest : MonoBehaviour {

        public TextAsset csvFile;
        public void Start()
        {
            string[,] csvArray = CSVHelper.SplitCsv(csvFile.text);
            Debug.Log("size = " + csvArray.GetLength(0) + "," + csvArray.GetLength(1));

            Debug.Log(CSVHelper.ArrayToString(csvArray));
        }

}
