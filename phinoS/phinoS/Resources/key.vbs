Set WShell = WScript.CreateObject("WScript.Shell")
Wshell.run "cmd.exe /C " & chr(34) & chr(34) & Replace(Wscript.ScriptFullName, "key.vbs", "") & "\run.bat" & chr(34) & chr(34) ,0,false
WShell.Sendkeys "{down}"

