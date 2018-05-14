# GarlicBot
---
## What does GarlicBot do?
I dunno some cool stuff.
Some stuff it currently supports:
- Echo command: barks stuff back at the user
- Roll command: rolls an N sided die
- Quote system:
  - Save the last message sent in a text channel as a quote for later
  - Manually enter a quote with an author and content
  - Get a random quote from the database and send it as a message to the text channel
- Image Processing:
  - Scramble an image
---
## Installation Instructions
1. Download here: https://github.com/Numenization/GarlicBot/releases
2. Unzip it somewhere
3. Run it once to generate config file
   - On windows, run GarlicBot.bat
   - On linux, download dotnet core (varies on distro) and run the bot from the terminal using the command:
     ```
	 dotnet GarlicBot.dll
	 ```
   - You will also need the gdiplus library to use image processing commands. Install with this command:
     ```
	 sudo apt-get install libgdiplus
	 ```
4. Get your bot's authentication token and put it in the config file
5. Run it again. Bot is now ready to go.
