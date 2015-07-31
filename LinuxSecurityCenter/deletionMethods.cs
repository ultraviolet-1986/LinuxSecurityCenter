//**************************************************************************
// SECURE DELETION CLASS FOR SECURITY CENTRE PROGRAM
// By William Willis (10/03/2014)
//**************************************************************************

using System;
using System.Diagnostics;
using System.IO;

namespace LinuxSecurityCenter
{
	public class deletionMethods
	{
		//**************************************************************************
		// DECLARE VARIABLES BLOCK
		//**************************************************************************
		#region variableDeclarationDeletion

		// SYSTEM APPLICATION LIST
		const string FIND = "find"; // 'FIND' PROGRAM (NON-ROOT)
		const string SHRED = "shred"; // 'SHRED' PROGRAM (NON-ROOT)

		// SYSTEM TERMINAL LIST: UNCOMMENT *ONE*
		// NOTE: LATER VERSIONS WILL DETECT DISTRIBUTION
		const string TERM = "gnome-terminal"; // GNOME TERMINAL (GNOME, UNITY, CINNAMON)
		//const string TERM = "konsole"; // KONSOLE (KDE)
		//const string TERM = "lxterminal"; // LXTERMINAL (LXDE)
		//const string TERM = "xfce4-terminal"; // XFCE4-TERMINAL (XFCE V4)

		#endregion

		//**************************************************************************
		// MAIN METHODS CODE BLOCK
		//**************************************************************************
		#region mainMethodsDeletion

		public static void setDirHome()
		{
			string dirHome = Environment.GetEnvironmentVariable("HOME");
			Environment.CurrentDirectory = dirHome;
		}

		public static int shredThumbnails(int passes)
		{
			setDirHome();

			// ERASE THUMBNAILS (CINNAMON/GNOME3 DESKTOPS)
			Process eraseThumbsCinnamon = new Process();
			eraseThumbsCinnamon.StartInfo.FileName = TERM;
			eraseThumbsCinnamon.StartInfo.Arguments =
				@"-t 'shred: Image Thumbnails' -e " +
				"'find .cache/thumbnails -exec shred -fuzv -n " +
				passes + @" {} \;'";

			eraseThumbsCinnamon.Start();
			eraseThumbsCinnamon.WaitForExit();

			// ERASE THUMBNAILS (KDE/MATE/XFCE DESKTOPS)
			Process eraseThumbsMate = new Process();
			eraseThumbsMate.StartInfo.FileName = TERM;
			eraseThumbsMate.StartInfo.Arguments =
				@"-t 'shred: Image Thumbnails' -e " +
				"'find .thumbnails -exec shred -fuzv -n " +
				passes + @" {} \;'";

			eraseThumbsMate.Start();
			eraseThumbsMate.WaitForExit();

			return passes;
		}

		public static int shredTrash(int passes)
		{
			setDirHome();

			// ERASE TRASH ITEMS (ALL DESKTOPS)
			// **WARNING** THIS OPERATION CAN TAKE A VERY LONG TIME
			Process eraseTrashFiles = new Process();
			eraseTrashFiles.StartInfo.FileName = TERM;
			eraseTrashFiles.StartInfo.Arguments =
				@"-t 'shred: Trash / Wastebasket' -e " +
				"'find .local/share/Trash -exec shred -fuzv -n " +
				passes + @" {} \;'";

			eraseTrashFiles.Start();
			eraseTrashFiles.WaitForExit();

			// * ERASE ANY LEFTOVER DIRECTORIES
			// * NO 'SAFE' SECURE DELETION METHOD EXISTS FOR DIRECTORIES
			// * ALTERNATIVES ALSO ERASE THE FILES EXPLAINED IN THE PATH SELECTED
			Process eraseLeftovers = new Process();
			eraseLeftovers.StartInfo.FileName = "/bin/bash";
			eraseLeftovers.StartInfo.Arguments =
				@"-c 'rm -rfv .local/share/Trash/files/*'";
			eraseLeftovers.Start();
			eraseLeftovers.WaitForExit();

			return passes;
		}

		public static int shredBashHistory(int passes)
		{
			setDirHome();

			// ERASE BASH COMMAND INPUT HISTORY (ALL DESKTOPS)
			Process eraseBashHistory = new Process();
			eraseBashHistory.StartInfo.FileName = TERM;
			eraseBashHistory.StartInfo.Arguments =
				@"-t 'shred: Command-line History' -e " +
				"'shred -fuzv -n " +
				passes + @" .bash_history'";

			eraseBashHistory.Start();
			eraseBashHistory.WaitForExit();

			return passes;
		}

		public static int shredRecentlyUsedFiles(int passes)
		{
			setDirHome();

			// ERASE RECENTLY USED DOCUMENTS LIST (ALL DESKTOPS)
			Process eraseRecentlyUsedFiles = new Process();
			eraseRecentlyUsedFiles.StartInfo.FileName = TERM;
			eraseRecentlyUsedFiles.StartInfo.Arguments =
				@"-t 'shred: Most Recently Used List' -e " +
				"'shred -fuzv -n " +
				passes + @" .local/share/recently-used.xbel'";

			eraseRecentlyUsedFiles.Start();
			eraseRecentlyUsedFiles.WaitForExit();

			return passes;
		}

		#endregion
	}
}

// End of File.