using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public bool GameIsPaused;
    private int levelid = 0;
    private string[] levels;
    private List<List<string>> curlevel;
    public int[] levelsize = new int[2];
    public Dictionary<string, List<GameObject>> teams;
    public HashSet<string> teamsToProgress;

    public string path = ".";//Application.persistentDataPath;
    public string campaign;
    public Canvas canvas;
    public Dictionary<string, Sprite[]> icons;
    public Dictionary<string, TankData> tanks;
    public bool godMode = false;

    public bool getGameIsPaused () { return GameIsPaused; }

    public void toggleGameIsPaused () { GameIsPaused = !GameIsPaused; } 

    //from this page https://simonleen.medium.com/game-manager-in-unity-part-1-1aafae6670ec, originally for gamemanager
    public static GameManager Instance
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

        GameIsPaused = false;

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

            //setup for canvas
            foreach (Image i in canvas.GetComponentsInChildren<Image>())
            {
                Color c = i.color;
                c.a = 1;
                i.CrossFadeAlpha(0, 0, true);
            }

            StartDisplay();
            Invoke(nameof(LoadNext), 1f * Time.timeScale);
            Invoke(nameof(StopDisplay), 3f * Time.timeScale);
            Debug.Log("loaded first level");

            /**/
            camp.Close();
        }
        catch (IOException e)
        {
            Debug.LogError(e);
        }
    }

    //loads the next level based on current level id
    void LoadNext()
    {
        //this will just load the next level and increase the level id
        if (levelid < levels.Length)
        {
            LoadLevel(levels[levelid]);
            levelid++;
        }
        else
        {
            EmptyLevel();
        }
    }

    void StartDisplay()
    {
        string name = levels[levelid].Split('\\')[5];
        name = name.Substring(4, name.Length - 8).Replace('_', ' ');

        canvas.GetComponentInChildren<Text>().text = name;
        canvas.GetComponentsInChildren<Image>().Where(img => img.name == "NextLevel").ElementAt(0).CrossFadeAlpha(1, 1, true);
        Time.timeScale = 1f / 64f;
    }

    void StopDisplay()
    {
        canvas.GetComponentInChildren<Text>().text = "";
        canvas.GetComponentsInChildren<Image>().Where(img => img.name == "NextLevel").ElementAt(0).CrossFadeAlpha(0, 0, true);

        Time.timeScale = 1;
    }

    void WinScreen()
    {
        canvas.GetComponentsInChildren<Image>().Where(img => img.name == "Win").ElementAt(0).CrossFadeAlpha(1, 1, true);
        canvas.GetComponentInChildren<Button>().gameObject.SetActive(true);

        toggleGameIsPaused();

        EmptyLevel();
        
        Time.timeScale = 0f;
    }

    void LoseScreen()
    {
        canvas.GetComponentsInChildren<Image>().Where(img => img.name == "Lose").ElementAt(0).CrossFadeAlpha(1, 1, true);

        toggleGameIsPaused();

        EmptyLevel();
        
        Time.timeScale = 0f;
    }

    //loads level from filename
    void LoadLevel(string filename)
    {
        //empty level
        EmptyLevel();

        //the current level is loaded as list of list of strings
        curlevel = new List<List<string>>();
        teams = new Dictionary<string, List<GameObject>>();
        teamsToProgress = new HashSet<string>();

        StreamReader level = new StreamReader(filename);

        //get all lines from file
        while (!level.EndOfStream)
        {
            //add lines while there are still some
            curlevel.Add(new List<string>(level.ReadLine().Replace(" ", "").Split(',')));
        }
        level.Close();

        //check that all lines have the same size
        for (int i = 1; i < curlevel.Count; i++)
        {
            if (curlevel[i].Count != curlevel[i - 1].Count)
            {
                throw new InvalidDataException("row sizes don't match");
            }
        }

        //add borders
        //x size = width of level
        levelsize[0] = curlevel[0].Count;

        //y size = height of level
        levelsize[1] = curlevel.Count;

        //place borders
        for (int x = -1; x < levelsize[0] + 1; x++)
        {
            for (int y = -1; y < levelsize[1] + 1; y++)
            {
                if ((x == -1) || (x == levelsize[0]) || (y == -1) || (y == levelsize[1]))
                {
                    GameObject curwall = Instantiate(Resources.Load<GameObject>("wall"), new Vector2(x, -y), Quaternion.Euler(0, 0, 0));
                    curwall.name = "wall";
                    curwall.AddComponent<Solid>();
                    curwall.AddComponent<ProjBounces>();
                    curwall.GetComponent<SpriteRenderer>().sprite = icons["block"][Random.Range(0, icons["block"].Length)];
                }
            }
        }

        //place rest of level
        for (int y = 0; y < levelsize[1]; y++)
        {
            for (int x = 0; x < levelsize[0]; x++)
            {
                name = curlevel[y][x];
                switch (name)
                {
                    //# specifies "normal" blocks (unbreakable, non-driveable, bullets bounce off)
                    case "#":
                        GameObject curwall = Instantiate(Resources.Load<GameObject>("wall"), new Vector2(x, -y), Quaternion.Euler(0, 0, 0));
                        curwall.name = "wall";
                        curwall.AddComponent<Solid>();
                        curwall.AddComponent<ProjBounces>();
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
                        breakable.AddComponent<ProjBounces>();
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

                                if (name.StartsWith("p1"))
                                {
                                    Camera.main.GetComponent<CameraScript>().target = curtank;
                                }
                            }
                            else
                            {
                                curtank = Instantiate(Resources.Load<GameObject>("bottank"), new Vector2(x, -y), Quaternion.Euler(0, 0, 0));
                            }

                            try
                            {
                                team = name.Split('-')[1];
                            }
                            catch (System.IndexOutOfRangeException)
                            {
                                team = teams.Count.ToString();
                            }

                            curtank.name = name.Split('-')[0];
                            curtank.GetComponent<Tank>().team = team;

                            if (name.StartsWith("p"))
                            {
                                teamsToProgress.Add(team);
                            }

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

    void EmptyLevel()
    {
        levelsize[0] = 0;
        levelsize[1] = 0;
        foreach (GameObject o in FindObjectsOfType<GameObject>())
        {
            if (!o.tag.Equals("persistent") && !o.tag.Equals("MainCamera"))
            {
                Destroy(o);
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

                if (tankparts.Length == 10 || tankparts.Length == 17)
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

                    if (tankparts.Length == 17)
                    {
                        tank.botProjFrequncy = float.Parse(tankparts[10], CultureInfo.InvariantCulture);
                        tank.botCalcRebounds = int.Parse(tankparts[11], CultureInfo.InvariantCulture);
                        tank.botBmbFrequncy = float.Parse(tankparts[12], CultureInfo.InvariantCulture);
                        tank.botPathfinding = float.Parse(tankparts[13], CultureInfo.InvariantCulture);
                        tank.botRandomness = float.Parse(tankparts[14], CultureInfo.InvariantCulture);
                        tank.botFear = float.Parse(tankparts[15], CultureInfo.InvariantCulture);
                        tank.botPositionFocus = float.Parse(tankparts[16], CultureInfo.InvariantCulture);
                    }
                }

                if (tank.noUnset() || tank.noUnsetBot())
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
                    foreach (string filename in parts[1].Split(','))
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

        if (teams.Count == 1 && teamsToProgress.Contains(teams.Keys.ToArray()[0]))
        {
            if (levelid == levels.Length)
            {
                WinScreen();
            }
            else
            {
                Debug.Log("Loading next level (" + levelid + ").");
                StartDisplay();
                Invoke(nameof(LoadNext), 2f * Time.timeScale);
                Invoke(nameof(StopDisplay), 3f * Time.timeScale);
            }
        }
        else if (teams.Count <= 1)
        {
            Debug.Log("Game Over");
            LoseScreen();
            CancelInvoke();
        }
    }

    public static Sprite PathnameToSprite(string filename)
    {
        Texture2D text = new Texture2D(1, 1);
        text.LoadImage(File.ReadAllBytes(filename));
        return Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(0.5f, 0.5f), Mathf.Min(text.height, text.width));
    }
}
