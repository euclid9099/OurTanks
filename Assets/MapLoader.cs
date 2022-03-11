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

        //load default icons - no errors are allowed
        StreamReader def = new StreamReader(path + "/Assets/Resources/Campaigns/default/000_config.txt");
        parts = def.ReadToEnd().Split('#');
        loadIcons(parts[0], true);
        def.Close();

        //try to load custom campaign settings
        try
        {
            StreamReader camp = new StreamReader(path + "/Assets/Resources/Campaigns/" + campaign + "/000_config.txt");
            parts = camp.ReadToEnd().Split('#');
            loadIcons(parts[0]);
            camp.Close();
        } catch(IOException e)
        {
            Debug.LogError(e);
        }
    }

    void loadIcons(string data, bool allowNewKeys = false)
    {
        //for each line assign key and value in dictionary
        string[] parts;
        foreach (string part in data.Split('\n'))
        {
            if (!part.StartsWith("---") && part.Contains(":"))
            {
                parts = part.Split(':');
                if (icons.ContainsKey(parts[0]) || allowNewKeys)
                {
                    //overwrite old value asignment with new assignment
                    icons.Remove(parts[0]);
                    icons.Add(parts[0], "Campaigns/" + (allowNewKeys? "default" : campaign) + "/icons/" + parts[1].Replace("\r", ""));
                    Debug.Log(icons[parts[0]]);
                }
            }
        }
    }
}
