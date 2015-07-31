//**************************************************************************
// ANTIVIRUS CLASS FOR SECURITY CENTRE PROGRAM
// By William Willis (10/03/2014)
//**************************************************************************

using System;
using System.Diagnostics;
using System.IO;

namespace LinuxSecurityCenter
{
	public class antivirusMethods
	{
		//**************************************************************************
		// DECLARE VARIABLES BLOCK
		//**************************************************************************
		#region variableDeclarationAntivirus

		// SYSTEM SHELL LIST
		const string BASH = "/bin/bash"; // DEFAULT SHELL
		const string SUDO = "/usr/bin/sudo"; // SUPER-USER SHELL
		const string GKSU = "/usr/bin/gksudo"; // GRAPHICAL SUPER-USER SHELL

		// PROGRAM IMPORT LIST
		const string FIND = "find"; // FIND PROGRAM (NON-ROOT)
		const string ZEN = "zenity"; // ZENITY WIDGETS (NON-ROOT)
		const string CLAM = "clamscan"; // CLAMAV VIRUS SCANNER (NON-ROOT)
		const string FRESH = "freshclam"; // CLAMAV VIRUS DEFINITIONS UPDATER (ROOT)

		// * SYSTEM TERMINAL LIST: UNCOMMENT ONE
		// * LATER VERSIONS WILL DETECT DISTRIBUTION
		const string TERM = "gnome-terminal"; // GNOME TERMINAL (GNOME, UNITY, CINNAMON)
		//const string TERM = "konsole"; // KONSOLE (KDE)
		//const string TERM = "lxterminal"; // LXTERMINAL (LXDE)
		//const string TERM = "xfce4-terminal"; // XFCE4-TERMINAL (XFCE V4)

		// SYSTEM SOFTWARE INSTALLER LIST
		const string APT_INSTALL = "apt-get install "; // DEBIAN, UBUNTU, LINUX MINT
		const string YUM_INSTALL = "yum install "; // FEDORA, RED HAT, CENTOS
		const string ZYP_INSTALL = "zypper install "; // OPENSUSE

		const string UPDATE = "freshclam";
		const string SCAN = "clamscan ";

		public const string CLAMAV_NOT_INSTALLED =
			"Dependency <b>clamav</b> is not installed on your system.\n\n" +
			"Click <b>OK</b> to install.";

		#endregion

		//**************************************************************************
		// MAIN METHODS CODE BLOCK
		//**************************************************************************
		#region variableDeclarationAntivirus

		public static void setDirHome()
		{
			// SET STARTING DIRECTORY TO BASH EQUIVENT: '$HOME'
			string dirHome = Environment.GetEnvironmentVariable("HOME");
			Environment.CurrentDirectory = dirHome;
		}

		public static void clamScanHome()
		{
			// CLAMAV: SCAN HOME DIRECTORY
			setDirHome();

			Process clamscan = new Process();
			clamscan.StartInfo.FileName = TERM;
			clamscan.StartInfo.Arguments =
				"-x bash -c 'rm ~/clamscan-home.log > /dev/null 2>&1; cd; " +
				"clamscan -r -l ~/clamscan-home.log; echo; " +
				"echo Process Complete. Close the window to return; echo; cd; " +
				"exec bash'";

			clamscan.Start();
			clamscan.WaitForExit();
		}

		public static void clamScanExternal()
		{
			// CLAMAV: SCAN EXTERNAL MEDIA (USB, DVD, BD, ETC...)
			Process clamscan = new Process();
			clamscan.StartInfo.FileName = TERM;
			clamscan.StartInfo.Arguments =
				"-x bash -c 'rm ~/clamscan-external-disks.log > /dev/null 2>&1; cd /media; " +
				"clamscan -r -l ~/clamscan-external-disks.log; echo; " +
				"echo Process Complete. Close the window to return; echo; cd; " +
				"exec bash'";

			clamscan.Start();
			clamscan.WaitForExit();
		}

		public static void clamScanFull()
		{
			// CLAMAV: SCAN ENTIRE COMPUTER (INCLUDING ROOT DIRECTORY AND EXTERNAL MEDIA)
			Process clamscan = new Process();
			clamscan.StartInfo.FileName = TERM;
			clamscan.StartInfo.Arguments =
				"-x bash -c 'rm ~/clamscan-full-computer.log > /dev/null 2>&1; cd /; " +
				"clamscan -r -l ~/clamscan-full-computer.log; echo; " +
				"echo Process Complete. Close the window to return; echo; cd; " +
				"exec bash'";

			clamscan.Start();
			clamscan.WaitForExit();
		}

		public static void clamavNotInstalledMsg()
		{
			// DISPLAY NOTIFICATION THAT 'CLAMAV' IS NOT INSTALLED
			Process displayClamAvError = new Process();
			displayClamAvError.StartInfo.FileName = ZEN;
			displayClamAvError.StartInfo.Arguments = "--error " +
				"--title='Linux Security Center: Error' --text='" +
				CLAMAV_NOT_INSTALLED + "'";

			displayClamAvError.Start();
			displayClamAvError.WaitForExit();
		}

		public static void installClamavGksu()
		{
			// INSTALL 'CLAMAV' ONTO THE SYSTEM
			Process installClamAv = new Process();
			installClamAv.StartInfo.FileName = GKSU;
			installClamAv.StartInfo.Arguments =
				@"-m 'Please input your password to install <b>clamav</b>.' " +
				"'" + TERM + " -x apt-get install -y clamav'";

			installClamAv.Start();
			installClamAv.WaitForExit();
		}

		public static void freshclamGksu()
		{
			// UPDATE 'CLAMAV' ANTIVIRUS DATABASE
			Process updateClamAvDatabase = new Process();
			updateClamAvDatabase.StartInfo.FileName = GKSU;
			updateClamAvDatabase.StartInfo.Arguments =
				@"-m 'Please input your password to update <b>clamav</b> virus definitions.' " +
				"'" + TERM + " -x freshclam'";

			updateClamAvDatabase.Start();
			updateClamAvDatabase.WaitForExit();
		}

		#endregion
	}
}

// End of File.