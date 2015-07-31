//**************************************************************************
// UPDATE CLASS FOR SECURITY CENTRE PROGRAM
// By William Willis (10/03/2014)
//**************************************************************************

using System;
using System.Diagnostics;
using System.IO;

namespace LinuxSecurityCenter
{
	public class updateMethods
	{
		//**************************************************************************
		// DECLARE VARIABLES BLOCK
		//**************************************************************************
		#region variableDeclarationUpdate

		// SYSTEM SHELL LIST
		const string BASH = "/bin/bash"; // DEFAULT SHELL
		const string SUDO = "/usr/bin/sudo"; // SUPER-USER SHELL
		const string GKSU = "/usr/bin/gksudo"; // GRAPHICAL SUPER-USER SHELL

		// SYSTEM TERMINAL LIST: UNCOMMENT ONE
		// NOTE: LATER VERSIONS WILL DETECT DISTRIBUTION
		const string TERM = "gnome-terminal"; // GNOME TERMINAL (GNOME, UNITY, CINNAMON)
		//const string TERM = "konsole"; // KONSOLE (KDE)
		//const string TERM = "lxterminal"; // LXTERMINAL (LXDE)
		//const string TERM = "xfce4-terminal"; // XFCE4-TERMINAL (XFCE V4)

		#endregion

		//**************************************************************************
		// MAIN METHODS CODE BLOCK
		//**************************************************************************
		#region variableDeclarationUpdate

		public static void aptGetUpdateGksu()
		{
			// * RUN 'APT-GET UPDATE' COMMAND AS GRAPHICAL SUPER-USER
			// * THIS PROCESS DOES NOT INSTALL SOFTWARE, IT CHECKS
			//   ONLY FOR THE AVAILABILITY OF NEW PACKAGES
			Process updatePackageCatalog = new Process();
			updatePackageCatalog.StartInfo.FileName = GKSU;
			updatePackageCatalog.StartInfo.Arguments =
				@"-m 'Please input your password to refresh <b>package catalog</b>.' " +
				"'" + TERM + " -x apt-get update'";

			updatePackageCatalog.Start();
			updatePackageCatalog.WaitForExit();
		}

		public static void aptGetUpgradeGksu()
		{
			// * RUN 'APT-GET UPDATE', 'APT-GET UPGRADE' AND
			//   'APT-GET DSELECT-UPGRADE' COMMANDS AS GRAPHICAL SUPER-USER
			// * THIS PROCESS COMBINES ALL COMMANDS REQUIRED TO UPDATE THE SYSTEM,
			//   BY 'DAISY-CHAINING' COMMANDS, A USER MUST ONLY INPUT PASSWORD ONCE
			//   INSTEAD OF ONCE PER COMMAND AS IS DEFAULT
			Process upgradeSystem = new Process();
			upgradeSystem.StartInfo.FileName = GKSU;
			upgradeSystem.StartInfo.Arguments =
				@"-m 'Please input your password to begin <b>system update</b>.' " +
				"'" + TERM + " -x bash -c' " +
				"'apt-get update; echo; " +
				"apt-get -y upgrade; echo; " +
				"apt-get -y dselect-upgrade'";

			upgradeSystem.Start();
			upgradeSystem.WaitForExit();
		}

		#endregion
	}
}

// End of File.