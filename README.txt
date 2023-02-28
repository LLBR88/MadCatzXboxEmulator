XBox 360 Controller Emulator for Mad Catz 32263 bluetooth controller

Issue:
Mad Catz controller is not properly recognised in Windows when connected via bluetooth. Firstly, it is not recognised as an XBox style pad and so is not natively supported in games using the XInput API. Secondly, and worse, the triggers are not recognised at all in DirectInput.

Solution:
This basic script utilises the lower-level RawInput API (thanks to the HidSharp plugin) which does recognise the triggers on the Mad Catz controller. These and all other inputs are passed to an emulated XBox 360 controller created by the ViGEm Bus. The Mad Catz controller can now be used in all games through the DirectInput or XInput API.
Windows will still see two controllers (the original Mad Catz and the emulated XBox), but the Mad Catz can be hidden using HidHide if there are any clashes. I haven't had any issues so far.

Prerequisites:
Microsoft .NET runtime 6.0
ViGEm driver
