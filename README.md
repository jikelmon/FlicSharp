# FlicSharp
A simple library to use your FlicButtons in combination with Mono on your RPi and the official Daemon which you can find here: https://github.com/50ButtonsEach/fliclib-linux-hci
A lot has to do, to make it fully functional but time was rare so i did only the minimum.

My programming skills are rudimental so you have to expect bugs in this initial release.

More functions added in the future.

#Before you start [Daemon]
Start the official Daemon on your machine by typing [limitied to localhost]:

sudo ./flicd -f flic.sqlite3

To expand the Daemon to listen on every network type:

sudo ./flicd -f flic.sqlite3 -s 0.0.0.0

#Usage of FlicSharp
First, include FlicSharp in you project and add it to your references.

Next, include the namespace by typing:

using FlicSharp;

//Create one single button and link event handler

FlicButton a = new FlicButton("Light", 12554, "80:E4:DA:71:08:0E");

a.SinglePressed += A_SinglePressed;

a.DoublePressed += A_DoublePressed;

a.Hold += A_Hold;

//Create a FlicClient connected to the official Daemon and link event handler

FlicClient f = new FlicClient("192.168.0.3");

f.Info += F_Info;

//Connect your Flic to the Daemon

f.ConnectFlic(a, LatencyMode.Normal, 511);

#Troubleshooting
Using an empty sqlite3 database works most of the time.

If your Flic does not connect, try turning it into public mode by holding the button more than 8 seconds and try the "ConnectFlic" method again.
