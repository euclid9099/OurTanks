using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapLoader : MonoBehaviour
{
    private static MapLoader _instance;

    public string campaign;
    public Dictionary<string, string> icons;

    //from this page https://simonleen.medium.com/game-manager-in-unity-part-1-1aafae6670ec, originally for gamemanager
    public static MapLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("instance is null");
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        icons = new Dictionary<string, string>();

        string path = ".";//Application.persistentDataPath;
        string[] parts;

        StreamReader def = new StreamReader(path + "/Assets/Resources/Campaigns/default/000_config.txt");
        foreach (string part in def.ReadToEnd().Split('\n'))
        {
            if (part.Contains(":"))
            {
                parts = part.Split(':');
                Debug.Log(((byte)parts[1][parts[1].Length - 1]));
                icons.Add(parts[0], "Campaigns/default/icons/" + parts[1].Replace("\r", ""));
            }
        }
        def.Close();

        try
        {
            StreamReader camp = new StreamReader(path + "/Assets/Resources/Campaigns/" + campaign + "/000_config.txt");
            string[] fileStr = camp.ReadToEnd().Split('#');
            camp.Close();

            foreach (string part in fileStr[0].Split('\n'))
            {
                if (!part.StartsWith("---") && part.Contains(":"))
                {
                    parts = part.Split(':');
                    if (icons.ContainsKey(parts[0]))
                    {
                        icons.Remove(parts[0]);
                        icons.Add(parts[0], "Campaigns/" + campaign + "/icons/" + parts[1].Replace("\r", ""));
                        Debug.Log(icons[parts[0]]);
                    }
                }
            }
        } catch(IOException e)
        {
            Debug.LogError(e);
        }
    }
}
