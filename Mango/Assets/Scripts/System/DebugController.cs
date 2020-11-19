using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mango.Game;

public class DebugController : MonoBehaviour
{


    public bool showConsole = true;
    string input;

    public static DebugCommand<string> SPAWN;

    public static DebugCommand GODMODE;

    public static DebugCommand KILLENEMIES;

    public List<object> commandList;

    private Player player;

    void Awake()
    {

        #region Commands
        SPAWN = new DebugCommand<string>("spawn", "Spawns a prefab", "spawn <prefabName>", (x) =>
        {
            if(x.ToUpper() != "PLAYER")
            RoomController.Instance.Spawn(x, transform.position);
        });



        GODMODE = new DebugCommand("godmode", "Toggles player god mode to become invincible", "godmode", () =>
        {
            player.ToggleInvincible();
            player.health = player.maxHealth;
            player.Revive();
        });

        KILLENEMIES = new DebugCommand("killenemies", "Kills all enemies", "killenemies", () =>
        {
            GameObject enemyParent = GameObject.Find("Enemies");
            foreach(Transform child in enemyParent.transform)
            {
                child.gameObject.GetComponent<Enemy>().Die();
            }
        });

        #endregion
        commandList = new List<object>
        {
            SPAWN,
            GODMODE,
            KILLENEMIES
        };
    }

    void Start()
    {
        player = GetComponent<Player>();
    }
    // Update is called once per frame
    void Update()
    {
        if(!showConsole && Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleDebug();
        }

    }
    void ToggleDebug()
    {
        showConsole = !showConsole;
    }

    Vector2 scroll;

    private void OnGUI()
    {
        if(!showConsole) { return; }



        GUI.Box(new Rect(0, Screen.height - 140f, Screen.width, 100), "");
        Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);

        scroll = GUI.BeginScrollView(new Rect(0, Screen.height - 140f + 5f, Screen.width, 90), scroll, viewport);

        for(int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase command = commandList[i] as DebugCommandBase;

            string label = $"{command.commandFormat} - { command.commandDescription}";
            Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
            GUI.Label(labelRect, label);
        }
        GUI.EndScrollView();
        

        GUI.Box(new Rect(0, Screen.height - 40f, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        GUI.SetNextControlName("input");

        
        input = GUI.TextField(new Rect(10, Screen.height - 35f, Screen.width - 20f, 20f), input);

        GUI.FocusControl("input");
        if (Event.current.isKey)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    HandleInput();
                    input = "";
                    Event.current.Use();
                    break;
                case KeyCode.Escape:
                    ToggleDebug();
                    Event.current.Use();
                    break;
            }
        }
    }

    private void HandleInput()
    {
        if (input == null || input == "")
            return;

        string[] properties = input.Split(' ');

        for(int i =0; i< commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;
            if(input.Contains(commandBase.commandId))
            {
                if(commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                    return;
                }else if(commandList[i] as DebugCommand<string> != null)
                {
                    (commandList[i] as DebugCommand<string>).Invoke(properties[1]);
                    return;
                }
            }
        }
        Debug.LogError("Command not found");
    }
}
