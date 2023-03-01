XBox 360 Controller Emulator for Mad Catz CTRLS 32263 bluetooth controller

Issue:
Mad Catz controller is not properly recognised in Windows when connected via bluetooth. Firstly, it is not recognised as an XBox style pad and so is not natively supported in games using the XInput API. Secondly, and worse, the triggers are not recognised at all in DirectInput.

Solution:
This script utilises the lower-level RawInput API (thanks to the HidSharp plugin) which does recognise the triggers on the Mad Catz controller. These and all other inputs are passed to an emulated XBox 360 controller created by the ViGEm Bus. The Mad Catz controller can now be used in all games through the DirectInput or XInput API.
Windows will still see two controllers (the original Mad Catz and the emulated XBox), but the Mad Catz can be hidden using HidHide if there are any clashes (I haven't tested this as so far I haven't found it to be necessary).
I have two Mad Catz controllers but I don't think it's possible to connect them both at the same time to the same pc so this script only works for one controller.
I have used this with other wired and wireless controllers for multiplayer split-screen play with no problems.

Prerequisites:
Microsoft .NET runtime 6.0 
https://dotnet.microsoft.com/en-us/download/dotnet/6.0
ViGEm driver 
https://github.com/ViGEm/ViGEmBus/releases
