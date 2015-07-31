//**************************************************************************
// FIREWALL CLASS FOR SECURITY CENTRE PROGRAM
// By William Willis (10/03/2014)
//**************************************************************************

using System;
using System.Diagnostics;
using System.IO;

namespace LinuxSecurityCenter
{
	public class firewallMethods
	{
		//**************************************************************************
		// DECLARE VARIABLES BLOCK
		//**************************************************************************
		#region variableDeclarationFirewall

		// SYSTEM SHELL LIST
		const string BASH = "/bin/bash"; // DEFAULT SHELL
		const string SUDO = "/usr/bin/sudo"; // SUPER-USER SHELL
		const string GKSU = "/usr/bin/gksudo"; // GRAPHICAL SUPER-USER SHELL

		// * SYSTEM TERMINAL LIST: UNCOMMENT ONE
		// * LATER VERSIONS WILL DETECT DISTRIBUTION
		const string TERM = "gnome-terminal"; // GNOME TERMINAL (GNOME, UNITY, CINNAMON)
		//const string TERM = "konsole"; // KONSOLE (KDE)
		//const string TERM = "lxterminal"; // LXTERMINAL (LXDE)
		//const string TERM = "xfce4-terminal"; // XFCE4-TERMINAL (XFCE V4)

		// ALTER DEPENDING ON FIREWALL - DEFAULT: 'UFW'
		const string FIRE = "ufw "; // FIREWALL PROGRAM
		const string ON = "enable "; // ACTIVATE FIREWALL COMMAND
		const string OFF = "disable"; // DEAVTIVATE FIREWALL COMMAND
		const string DENY = " default deny"; // DEFAULT RULES SET TO 'DENY'
		const string ALLOW = " default allow"; // DEFAULT RULES SET TO 'ALLOW'
		const string LIMIT = " limit ssh"; // LIMIT TRAFFIC

		#endregion

		//**************************************************************************
		// MAIN METHODS CODE BLOCK
		//**************************************************************************
		#region mainMethodsFirewall

		public static void ufwEnableGksu()
		{
			// ENABLE 'UFW' FIREWALL
			Process enableFirewall = new Process();

			enableFirewall.StartInfo.FileName = GKSU;
			enableFirewall.StartInfo.Arguments =
				"gksudo -m 'Please input your password to enable <b>ufw</b> firewall.' " +
				"\"" + TERM +" -x bash -c 'ufw enable; ufw default deny; ufw limit ssh'\"";


			enableFirewall.Start();
			enableFirewall.WaitForExit();
		}

		public static void ufwDisableGksu()
		{
			// DISABLE 'UFW' FIREWALL
			Process disableFirewall = new Process();

			disableFirewall.StartInfo.FileName = GKSU;
			disableFirewall.StartInfo.Arguments =
				"gksudo -m 'Please input your password to disable <b>ufw</b> firewall.' " +
				"\"" + TERM +" -x bash -c 'ufw disable'\"";


			disableFirewall.Start();
			disableFirewall.WaitForExit();
		}

		#endregion
	}
}

// End of File.