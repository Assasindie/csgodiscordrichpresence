# CSGO Discord Rich Presence
Integrates Discord Rich Presence with your csgo game!

# Features
Displays Current Map and Mode with a picture. (Only works with official maps not workshop maps!)

Displays what Team (CT or T) with the relevant picture and your current HP.

Displays your Kills Assists and Deaths aswell as your current gun equipped.

Displays the overall score and if the map is live or warmup etc.

Also says how long you have been ingame for!


## Installation
Via NuGet:

```
Install-Package CSGSI
```
Download and compile https://github.com/Lachee/discord-rpc-csharp#building and add a reference to the DiscordRPC.dll

If you ever delete the gamestate_integration_discord.cfg file, you will need to remake it using the following values 
```
"DiscordGSI"
{
	"uri" "http://localhost:3000"
	"timeout" "5.0"
	"data"
	{
		"provider" "1"
		"map" "1"
		"round" "1"
		"player_id" "1"
		"player_weapons" "1"
		"player_match_stats" "1"
		"player_state" "1"
		"allplayers_id" "1"
		"allplayers_state" "1"
		"allplayers_match_stats" "1"
	}
}
```

Not affiliated in any way with either Valve or Discord.
