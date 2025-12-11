using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Log : MonoBehaviour
{
    private static string filePath = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        CreateDirectory();
    }

    // Making directory for saving log.
    private void CreateDirectory()
    {
        filePath = Application.dataPath + "/Log/" + ExperimentCondition.DataTitle + "/";

        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
    }

    // Saving log.
    public static void Output(string fileName, List<string> logs)
    {
        var FullPath = Path.Combine(filePath, fileName);
        File.AppendAllLines(FullPath, logs);
    }
}