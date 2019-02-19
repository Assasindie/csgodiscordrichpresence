using CSGSI;
using DiscordRPC;
using System;
using System.Diagnostics;
using System.IO;
using System.Timers;

namespace csgodiscordrichpresence
{
    class Presence
    {
        static GameStateListener gsl;
        public static DiscordRpcClient client;
        public static DateTime gameOpened = DateTime.UtcNow;
        public static bool stopped = false;
        public static Timer csCheck;

        public static void Start()
        {
            //check if cs is open
            if (!GetCsgo())
            {
                Console.WriteLine("CSGO not open, please try again with CSGO open! Press any key to exit");
                Console.ReadKey();
                return;
            }

            //initialize the presecense 
            Presence p = new Presence();
            p.Initialize();

            //set a timer to check if csgo is open every 10 seconds.
            csCheck = new Timer(10000)
            {
                AutoReset = true
            };
            csCheck.Elapsed += CsCheck_Elapsed;
            csCheck.Start();

            //create the GameStateListener on port 3000
            gsl = new GameStateListener(3000);
            gsl.NewGameState += new NewGameStateHandler(OnNewGameState);
            gsl.EnableRaisingIntricateEvents = true;
            if (!gsl.Start())
            {
                Console.WriteLine("Failed to initialize");
            }
            Console.WriteLine("Started Rich Presence.");
        }

        //timer elapsed event
        private static void CsCheck_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!GetCsgo())
            {
                Console.WriteLine("CSGO is no longer open!");
                client.ClearPresence();
                stopped = true;
            }
            else if (stopped)
            {
                gameOpened = DateTime.UtcNow;
                stopped = false;
            }
        }

        //checks processes for csgo
        public static bool GetCsgo()
        {
            Process[] csgo = Process.GetProcessesByName("csgo");
            if (csgo.Length >= 0)
            {
                string csgodir = csgo[0].MainModule.FileName; //csgo file path
                csgodir = csgodir.Remove(csgodir.Length - 8, 8); //trim csgo.exe
                csgodir += @"csgo\cfg\gamestate_integration_discord.cfg"; //make dir to the config location
                if (!File.Exists(csgodir))
                {
                    CreateCFG(csgodir);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        static void CreateCFG(string path)
        {
            string cfgContents = "";
            if (!File.Exists(Environment.CurrentDirectory + @"\gamestate_integration_discord.cfg"))
            {
                Console.WriteLine("Could not find template CFG file, please redownload it!");
                return;
            }
            using (StreamReader sr = File.OpenText(Environment.CurrentDirectory + @"\gamestate_integration_discord.cfg"))
            {
                cfgContents = sr.ReadToEnd();
            }
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(cfgContents);
            }
        }

        //event when gamestatelistener has a new event
        static void OnNewGameState(GameState gs)
        {
            if (gsl.CurrentGameState.Map.Name == "" && gsl.CurrentGameState.Player.MatchStats.Kills == -1)
            {
                client.SetPresence(new RichPresence()
                {
                    Details = "In Main Menu",
                    State = "Main Menu",
                    Assets = new Assets()
                    {
                        LargeImageKey = "mainmenu",
                        LargeImageText = "Main Menu"
                    },
                    Timestamps = new Timestamps(gameOpened),
                });
            }
            else
            {
                if (gsl.CurrentGameState.Provider.SteamID == gsl.CurrentGameState.Player.SteamID)
                {
                    client.SetPresence(new RichPresence()
                    {
                        Details = gsl.CurrentGameState.Player.MatchStats.Kills + "-" + gsl.CurrentGameState.Player.MatchStats.Assists + "-" + gsl.CurrentGameState.Player.MatchStats.Deaths
                        + " " + gsl.CurrentGameState.Player.Weapons.ActiveWeapon.Name,

                        State = "CT : " + gsl.CurrentGameState.Map.TeamCT.Score + " T : " + gsl.CurrentGameState.Map.TeamT.Score + " " + gsl.CurrentGameState.Map.Phase,
                        Assets = new Assets()
                        {
                            LargeImageKey = gsl.CurrentGameState.Map.Name,
                            LargeImageText = gsl.CurrentGameState.Map.Name + " - " + gsl.CurrentGameState.Map.Mode.ToString(),
                            SmallImageKey = "team" + gsl.CurrentGameState.Player.Team.ToString().ToLower(),
                            SmallImageText = "Team " + gsl.CurrentGameState.Player.Team.ToString() + " - " + gsl.CurrentGameState.Player.State.Health + " Health",
                        },
                        Timestamps = new Timestamps(gameOpened),
                    });
                }
                client.Invoke();
            }
        }

        void Initialize()
        {
            //client containg all the relevant artwork etc for the rich presence.
            client = new DiscordRpcClient("545493713779163147");

            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {

            };

            client.Initialize();
        }
    }
}
