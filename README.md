### A C# NetCore OpenTibia server, for client version 7.72.

# Repo guidelines and content map:

If you're looking for the TL;DR trying to check out what this is about, see [how to setup and run the Server](docs/Setup.md).

For more complex setup and configuration information check out the [server configuration](docs/Configuration.md) section.

For some ramblings about me, and why I started this project, visit this [project's motivation](docs/Motivation.md) page.

And so... what's the status and overview of things implemented? Here's the [project feature overview / roadmap](docs/Roadmap.md).

Amiroslo was kind enough to set up a channel on his Discord server: so join the conversation there: https://discord.gg/GpMJt9

###### More docs to (probably) come soon!

# Some Frequently Asked Questions
## Why C#/NetCore?
Because it is **awesome**. It is fast, simple and way more maintainable than C++. 
It's easier for newbies to pick up and learn, as it is often taught in schools and bootcamps.

It's also what half the industry uses and, well, I happen to be part of that half of the industry. 

_And lastly, yeah- generally whoever starts coding something from scratch gets to pick..._

## Why choose version 7.72?
There was an infamous CipSoft breach back on 2007 in which all files and assets required to run the original game at the time were leaked, and those files were finally released to the public about 10 years later on OTLand.

Having access to the original game files and assets simplifies our goal to mimic and reverse engineer the original game, leaving us with an implementation that is as closest to the real game as it gets. 

On a more personal note, I hate aimbots (and hotkeys) since they ruin the game. Aimbots are fairly easy to catch with simple metric analysis and automation, and hotkeys were introduced on shortly after this version (7.8, or 8.0?) of the game.

I'm not however, opposed to develop any other versions of the game later on, but I'm super fond of 7.x versions as these were the ones in which I actually played the real game.

###### I'll add more FAQ as they come along...
