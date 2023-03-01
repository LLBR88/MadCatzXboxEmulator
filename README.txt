XBox 360 Controller Emulator for Mad Catz CTRLS 32263 bluetooth controller

Background:
The Mad Catz controller is not properly recognised in Windows when connected via bluetooth. Firstly, it is not recognised as an XBox style pad and so is not natively supported in games using the XInput API. Secondly, and worse, the triggers are not recognised at all.

Solution:
This script utilises the lower-level RawInput API (thanks to the HidSharp plugin) which does recognise the triggers on the Mad Catz controller. These and all other inputs are passed to an emulated XBox 360 controller created by the ViGEm Bus. The Mad Catz controller can now be used in all games through the DirectInput or XInput API.

Known issues:
This script adds a new emulated XBox controller but does not hide/remove the actual Mad Catz one. I have not encountered any issues so far but the HidHide utility might be useful if you have clashes between the two.
The Home and Back buttons can't be remapped and I have not been able to do anything about them. The Home button launches your browser and the Back button navigates back. These cannot be used in games.
This will only create one emulated pad for one connected Mad Catz controller. In my experience, it's not possible to connect two at the same time so I haven't included the ability to create a second one.

Thanks:
HidSharp
www.zer7.com/software/hidsharp
ViGEm by Nafarius
https://vigem.org/

Installation:
Download and install prerequisites (see below)
Download latest release exe
Run exe file

Prerequisites:
Microsoft .NET runtime 6.0 (download either the Hosting Bundle or the .NET Desktop Runtime)
https://dotnet.microsoft.com/en-us/download/dotnet/6.0
ViGEm driver 
https://github.com/ViGEm/ViGEmBus/releases
