using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
    private static MapLoader _instance;
    private int levelid = 0;
    private string[] levels;
    private List<List<string>> curlevel;
    private Dictionary<string, List<GameObject>> teams;

    public string path = ".";//Application.persistentDataPath;
    public string campaign;
    public Canvas canvas;
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
        Debug.Log("Maploader awoken");

        _instance = this;
        icons = new Dictionary<string, Sprite[]>();

        
        string[] parts;

        //load default icons - no errors are allowed
        StreamReader def = new StreamReader(path + "\\Assets\\Resources/Campaigns/default/000_config.txt");
        parts = def.ReadToEnd().Split('#');
        LoadIcons(parts[0], true);
        def.Close();

        //try to load custom campaign settings
        try
        {
            StreamReader camp = new StreamReader(path + "/Assets/Resources/Campaigns/" + campaign + "/000_config.txt");
            parts = camp.ReadToEnd().Split('#');
            LoadIcons(parts[0]);
            Debug.Log("loaded icons");
            LoadTanks(parts[1]);
            Debug.Log("loaded tanks");
            LoadLevelnames();
            Debug.Log("loaded level names");
            LoadNext();
            Debug.Log("loaded first level");

            /**/
            camp.Close();
        } catch(IOException e)
        {
            Debug.LogError(e);
        }
    }

    private void LateUpdate()
    {
        if (teams.Count <= 1)
        {
            LoadNext();
        }
    }

    //loads the next level based on current level id
    void LoadNext()
    {
        //this will just load the next level and increase the level id
        if(levelid < levels.Length)
        {
            string name = levels[levelid].Split('\\')[5];
            name = name.Substring(4, name.Length-8).Replace('_', ' ');
            Debug.Log(name);
            canvas.GetComponent<Canvas>().enabled = true;
            canvas.GetComponentInChildren<Text>().text = name;
            LoadLevel(levels[levelid]);
            levelid++;
        }
    }

    //loads level from filename
    void LoadLevel(string filename)
    {
        //empty level
        foreach(GameObject o in FindObjectsOfType<GameObject>())
        {
            if (!o.tag.Equals("persistent") && !o.tag.Equals("MainCamera"))
            {
                Destroy(o);
            }
        }

        //the current level is loaded as list of list of strings
        curlevel = new List<List<string>>();
        teams = new Dictionary<string, List<GameObject>>();
        
        StreamReader level = new StreamReader(filename);

        //get all lines from file
        while (!level.EndOfStream)
        {
            //add lines while there are still some
            curlevel.Add(new List<string>(level.ReadLine().Replace(" ", "").Split(',')));
        }
        level.Close();

        //check that all lines have the same size
        for(int i = 1; i < curlevel.Count; i++)
        {
            if (curlevel[i].Count != curlevel[i-1].Count)
            {
                throw new InvalidDataException("row sizes don't match");
            }
        }

        //add borders
        //get level sizes
        float size_x = curlevel[0].Count;
        float size_y = curlevel.Count;

        //place borders
        for (int x = -1; x < size_x + 1; x++)
        {
            for (int y = -1; y < size_y + 1; y++)
            {
                if ((x == -1) || (x == size_x) || (y == -1) || (y == size_y))
                {
                    GameObject curwall = Instantiate(Resources.Load<GameObject>("wall"), new Vector2(x, -y), Quaternion.Euler(0, 0, 0));
                    curwall.name = "wall";
                    curwall.AddComponent<Solid>();
                    curwall.GetComponent<SpriteRenderer>().sprite = icons["block"][Random.Range(0, icons["block"].Length)];
                }
            }
        }

        //place rest of level
        for (int y = 0; y < size_y; y++)
        {
            for(int x = 0; x < size_x; x++)
            {
                name = curlevel[y][x];
                switch (name)
                {
                    //# specifies "normal" blocks (unbreakable, non-driveable, bullets bounce off)
                    case "#":
                        GameObject curwall = Instantiate(Resources.Load<GameObject>("wall"), new Vector2(x, -y), Quaternion.Euler(0, 0, 0));
                        curwall.name = "wall";
                        curwall.AddComponent<Solid>();
                        curwall.GetComponent<SpriteRenderer>().sprite = icons["block"][Random.Range(0, icons["block"].Length)];
                        break;
                    //O specifies holes (unbreakable, non-driveable, bullets unaffected)
                    case "O":
                        GameObject curhole = Instantiate(Resources.Load<GameObject>("wall"), new Vector2(x, -y), Quaternion.Euler(0, 0, 0));
                        curhole.name = "hole";
                        curhole.AddComponent<Solid>();
                        curhole.GetComponent<SpriteRenderer>().sprite = icons["hole"][Random.Range(0, icons["hole"].Length)];
                        break;
                    //# specifies breakable blocks (breakable, non-driveable, bullets bounce off)
                    case "X":
                        GameObject breakable = Instantiate(Resources.Load<GameObject>("wall"), new Vector2(x, -y), Quaternion.Euler(0, 0, 0));
                        breakable.name = "weak_block";
                        breakable.AddComponent<Breakable>();
                        breakable.AddComponent<Solid>();
                        breakable.GetComponent<SpriteRenderer>().sprite = icons["weak_block"][Random.Range(0, icons["weak_block"].Length)];
                        break;
                    default:
                        //if the name matches that of a tank
                        if (tanks.ContainsKey(name.Split('-')[0]))
                        {
                            GameObject curtank;
                            string team;

                            //if the tank is a player tank
                            if (name.StartsWith("p"))
                            {
                                curtank = Instantiate(Resources.Load<GameObject>("playertank"), new Vector2(x, -y), Quaternion.Euler(0, 0, 0));
                                curtank.name = name.Split('-')[0];
                            } else
                            {
                                curtank = Instantiate(Resources.Load<GameObject>("playertank"), new Vector2(x, -y), Quaternion.Euler(0, 0, 0));
                                curtank.name = name.Split('-')[0];
                            }

                            try
                            {
                                team = name.Split('-')[1];
                            } catch (System.IndexOutOfRangeException)
                            {
                                team = teams.Count.ToString();
                            }

                            curtank.GetComponent<Tank>().team = team;

                            if (!teams.ContainsKey(team))
                            {
                                teams.Add(team, new List<GameObject>());
                            }

                            teams[team].Add(curtank);
                        }
                        break;
                }
            }
        }
    }

    //gets all files in current folder matching our filenames
    void LoadLevelnames()
    {
        Regex regex = new Regex(@".*\\[0-9]{3}_.*\.lvl$");
        levels = Directory.GetFiles(path + "\\Assets\\Resources\\Campaigns\\" + campaign).Where(s => regex.IsMatch(s)).ToArray();
    }

    void LoadTanks(string data)
    {
        //JsonUtility.FromJson(data,System.Type.GetType("string"));

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

                if (tankparts.Length == 10)
                {
                    //path + "/Assets/Resources/Campaigns/" + campaign + "/icons/"
                    //get tank base texture
                    tank.tankBase = tankparts[0].Replace("\r", "");
                    
                    //get tank tower texture
                    tank.tower = tankparts[1].Replace("\r", "");

                    //get speed
                    tank.speed = float.Parse(tankparts[2], CultureInfo.InvariantCulture);

                    //bullets:
                    //get bullet limit
                    tank.projLimit = int.Parse(tankparts[3], CultureInfo.InvariantCulture);

                    //get bullet rebounds
                    tank.projBounces = int.Parse(tankparts[4], CultureInfo.InvariantCulture);

                    //get bullet speed
                    tank.projSpeed = float.Parse(tankparts[5], CultureInfo.InvariantCulture);

                    //bombs:
                    //get bomb limit
                    tank.bmbLimit = int.Parse(tankparts[6], CultureInfo.InvariantCulture);

                    //get bomb explosionradius
                    tank.bmbExplosion = float.Parse(tankparts[7], CultureInfo.InvariantCulture);

                    //get bomb detectionradius
                    tank.bmbDetection = float.Parse(tankparts[8], CultureInfo.InvariantCulture);

                    //get bomb timer
                    tank.bmbTimer = float.Parse(tankparts[9], CultureInfo.InvariantCulture);
                }

                if (tank.noUnset())
                {
                    tanks.Add(parts[0], tank);
                }
            }
        }/**/
    }

    void LoadIcons(string data, bool allowNewKeys = false)
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
                }
            }
        }
    }

    public void TankDeath(GameObject tank)
    {
        teams[tank.GetComponent<Tank>().team].Remove(tank);
        if (teams[tank.GetComponent<Tank>().team].Count <= 0)
        {
            teams.Remove(tank.GetComponent<Tank>().team);
        }
    }

    public static Sprite PathnameToSprite(string filename)
    {
        Texture2D text = new Texture2D(1, 1);
        text.LoadImage(File.ReadAllBytes(filename));
        return Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(0.5f, 0.5f), Mathf.Min(text.height, text.width));
    }
}
