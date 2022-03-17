using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Drawing;
using System.Globalization;

public class MapLoader : MonoBehaviour
{
    private static MapLoader _instance;

    public string path = ".";//Application.persistentDataPath;
    public string campaign;
    public Dictionary<string, Sprite[]> icons;
    public Dictionary<string, TankData> tanks;

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
            loadTanks(parts[1]);
            Debug.Log(tanks["p1"]);
            camp.Close();
        } catch(IOException e)
        {
            Debug.LogError(e);
        }
    }

    void loadTanks(string data)
    {
        tanks = new Dictionary<string, TankData>();
        
        //create tankdata from file, by name
        string[] parts;
        foreach (string part in data.Split('\n'))
        {
            //ignore comments and lines without suffie
            if (!part.StartsWith("---") && part.Contains(":"))
            {
                parts = part.Split(':');
                string[] tankparts = parts[1].Split(',');
                TankData tank = new TankData();

                if (tankparts.Length == 12)
                {
                    //get tank base texture
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(File.ReadAllBytes(path + "/Assets/Resources/Campaigns/" + campaign + "/icons/" + tankparts[0].Replace("\r", "")));
                    tank.tankBase = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);

                    //get tank tower texture
                    texture.LoadImage(File.ReadAllBytes(path + "/Assets/Resources/Campaigns/" + campaign + "/icons/" + tankparts[1].Replace("\r", "")));
                    tank.tower = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);

                    //get speed
                    tank.speed = float.Parse(tankparts[2], CultureInfo.InvariantCulture);

                    //bullets:
                    //get bullet limit
                    tank.bltLimit = int.Parse(tankparts[3], CultureInfo.InvariantCulture);

                    //get bullet rebounds
                    tank.bltBounces = int.Parse(tankparts[4], CultureInfo.InvariantCulture);

                    //get bullet speed
                    tank.bltSpeed = float.Parse(tankparts[5], CultureInfo.InvariantCulture);

                    //get bullet acceleration
                    tank.bltAccel = float.Parse(tankparts[6], CultureInfo.InvariantCulture);

                    //get bullet size
                    tank.bltSize = float.Parse(tankparts[7], CultureInfo.InvariantCulture);

                    //bombs:
                    //get bomb limit
                    tank.bmbLimit = int.Parse(tankparts[8], CultureInfo.InvariantCulture);

                    //get bomb explosionradius
                    tank.bmbExplosion = float.Parse(tankparts[9], CultureInfo.InvariantCulture);

                    //get bomb detectionradius
                    tank.bmbDetection = float.Parse(tankparts[10], CultureInfo.InvariantCulture);

                    //get bomb timer
                    tank.bmbTimer = float.Parse(tankparts[11], CultureInfo.InvariantCulture);
                }

                if (tank.noNull())
                {
                    tanks.Add(parts[0], tank);
                }
            }
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
