
Set WShell = WScript.CreateObject("WScript.Shell")
Wshell.run "run.bat",0,false
WShell.Sendkeys "{down}"
WShell.Sendkeys "{enter}"