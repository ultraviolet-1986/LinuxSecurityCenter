# LinuxSecurityCenter

**Build Environment:** Linux Mint Debian 2 (i586) Virtual Machine, MonoDevelop 5.9.4

**Notes:**
- Will currently function only on a GNOME-based desktop only unless modified.
- Program will freeze during Terminal task execution becuase of poor threading.

**To do:**
- Implement Desktop Environment detection and run accordingly.
- Implement native (C#) file handling and bring in a secure deletion library.
- Remove Zenity Widgets from code and use native notifications and prompts.
- Integrate console output into program window / solve buffer flooding.
- Rewrite threading for better stability and task-handling.