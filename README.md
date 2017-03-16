MultiKeys for Windows
=======================

Wants to play multiplayer games like FIFA, EA Sports Cricket 07, WWE, Tekken 
etc.. on your PC without buying Joystick.

With this software you can connect 2 keyboards to one PC and play 
Multiplayer games without any conflicts between two keyboards.

#### Download

Download [latest version].
  *Stable version is not yet ready to downlaod.

#### How to use
1. Download the [latest version].
2. Install vjoy Device Driver from Requirements folder.
3. Install kernelhotkey.exe Device Driver from Requirements folder.
	few antiviruses delete the kernelhotkey.dll file, so please 
	stop your antivirus before installing kernelhotkey.exe.
4. Go to your c:\programfiles\vjoy\x86, open vjoyconf.exe and 
	add a new device with 8 buttons and x,y,z axis.
5. open multikeys.exe, click on Free. Then press any key on
	your 2nd keyboard to get keyboard id and put that id number in kbdid.
		Click on update and then click on start.
		Minimize the software and start any game you want to play.

		
		
 Requirements
	Windows xp, 7, 8, 8.1, 10
	.Net Framework 4.0 or above
	An aditional keyboard preferably with USB port.
	
#### Wants to Contribute to Project
 some things to read first.
 [SDK] for vjoy drivers by [Shaul Eizikovich].
[documentation] for  kernel hotkey by http://yorick.oblita.com.



#### Develop

[Visual Studio 2015] & [.NET Framework 4.6.2 Developer Pack] are required.

#### License

MIT


[latest release]: 		https://github.com/lalitsom/multikeys
[documentation]:    	http://yorick.oblita.com/hooking-part2
[SDK]:        			https://sourceforge.net/projects/vjoystick/files/Beta%202.x/2.1.7.7-260916/
[Shaul Eizikovich]: 	https://sourceforge.net/u/userid-1374741/profile/
[Visual Studio 2015]: 	https://www.visualstudio.com/downloads/
[.NET Framework 4.6.2 Developer Pack]: https://www.microsoft.com/download/details.aspx?id=53321