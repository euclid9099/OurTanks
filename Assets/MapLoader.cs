using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Drawing;

public class MapLoader : MonoBehaviour
{
    private static MapLoader _instance;

    public string path = ".";//Application.persistentDataPath;
    public string campaign;
    public Dictionary<string, Sprite[]> icons;

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
        icons = new Dictionary<string, Sprite[]>();

        
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
                    List<Sprite> sprites = new List<Sprite>();
                    foreach(string filename in parts[1].Split(','))
                    {
                        

                        Texture2D texture = new Texture2D(100, 100);
                        texture.LoadImage(File.ReadAllBytes(path + "/Assets/Resources/Campaigns/" + (allowNewKeys ? "default" : campaign) + "/icons/" + filename.Replace("\r", "")));
                        Debug.Log(texture.width + " x " + texture.height);
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
                        sprites.Add(sprite);
                    }
                    icons.Add(parts[0], sprites.ToArray());
                    Debug.Log(icons[parts[0]]);
                }
            }
        }
    }
}
