//**************************************************************************
// MAIN FORM CLASS FOR SECURITY CENTRE PROGRAM
// By William Willis (10/03/2014)
//**************************************************************************

using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Gtk;

// PLUG IN EXTERNAL CLASSES
// ...OR WRAP IN A 'NAMESPACE'
using LinuxSecurityCenter;

public partial class frmMain: Gtk.Window
{
	//**************************************************************************
	// DECLARE VARIABLES BLOCK
	//**************************************************************************
	#region variableDeclaration

	// THREAD LOCKS
	object deletionThreadLock = new object();
	object updateThreadLock = new object();
	object upgradeThreadLock = new object();
	object clamscanThreadLock = new object();
	object clamscanInstallLock = new object();
	object freshclamThreadLock = new object();
	object enableFirewallThreadLock = new object();
	object disableFirewallThreadLock = new object();

	// * NUMBER OF PASSES TO 'SHRED'
	// * VALUE IS SET BY RADIO BUTTON LIST
	int shredIterations = 0;

	// PROGRAM IMPORT LIST
	const string ZEN = "zenity"; // ZENITY WIDGETS (NON-ROOT)

	// PROGRESS BAR VARIABLES
	string progBarText = "";
	const string PROGTEXT_DEFAULT = "Ready when you are ^_^b";
	const int TASK_DELAY = 80; // TASK DELAY TO AID PROGRESS BAR
	bool taskLocked = true; // PULSE PROGRESS BAR UNTIL 'FALSE'

	// NOTIFICATION VARIABLES
	string notificationHeader = "Linux Security Center: ";
	string notificationText = "";

	// CLAMAV BINARY LOCATION CHECK
	const string CLAMSCAN_BIN = "/usr/bin/clamscan";

	// ABOUT THE AUTHOR TEXT
	const string AUTHOR_INFO =
		"<b>About the author:</b>\n\n" +
		"Written by William Willis Whinn\n" +
		"<i>bg10sa@student.sunderland.ac.uk</i>\n\n" +
		"<b>Technologies used:</b>\n\n" +
		"MonoDevelop (C#, GTK#)\n" +
		"Zenity Widgets (BASH)\n" +
		"BASH (MATE Terminal, GNOME Terminal)\n" +
		"Linux Mint 13 (i386/amd64)\n" +
		"Oracle VirtualBox (amd64)";

	#endregion

	//**************************************************************************
	// FORM LOAD EVENTS BLOCK
	//**************************************************************************
	#region formLoadEvents

	public frmMain (): base (Gtk.WindowType.Toplevel)
	{
		Build();

		// CLEAR CONSOLE AND RESET PROGRESS BAR
		clearConsole();
		progBar.Text = PROGTEXT_DEFAULT;

		// ACTIVATE DEFAULT RADIO BUTTONS
		rdoStandardShred.Activate();
		rdoClamavHomeFolder.Activate();

		// ACTIVATE DEFAULT CHECKBOXES
		chkThumbnails.Activate();
		chkMru.Activate();
		chkBashHist.Activate();
	}

	#endregion

	//**************************************************************************
	// MESSAGE OUTPUT BLOCK
	//**************************************************************************
	#region messageOutput

	public void taskComplete()
	{
		// DISPLAY SYSTEM NOTIFICATION USING 'ZENITY'
		var notificationTaskComplete = Process.Start(ZEN,
			"--notification --timeout=1 --text='" +
			notificationHeader +
			notificationText + "'");
		notificationTaskComplete.WaitForExit();
	}

	public void noInternetConnection()
	{
		// INTERNET CONNECTION DOES NOT EXIST
		MessageDialog mBoxError = new MessageDialog (this,
			DialogFlags.DestroyWithParent,
			MessageType.Error,
			ButtonsType.Close,
			"An active Internet connection was not detected. Please connect to the Internet and try again.");
		mBoxError.Run();
		mBoxError.Destroy();
	}

	protected void about_Activated (object sender, System.EventArgs e)
	{
		var aboutTheAuthor = Process.Start(ZEN,
			"--info --title='About' --text='" + AUTHOR_INFO + "'");
		aboutTheAuthor.WaitForExit();
	}

	#endregion

	//**************************************************************************
	// BUTTON CODE BLOCK
	//**************************************************************************
	#region buttonCode

	protected void btnClear_Click (object sender, System.EventArgs e)
	{
		// CLEAR THE CONSOLE WINDOW AND RESET PROGRESS BAR
		progBar.Text = PROGTEXT_DEFAULT;
		clearConsole();
	}

	protected void btnDelete_Click (object sender, System.EventArgs e)
	{
		clearConsole();

		// * SELECT DELETION ALGORITHM BASED ON RADIO BUTTON SELECTED
		// * DELETION PASSES EQUAL TO 'SHREDITERATIONS'
		if(rdoMinimalShred.Active == true)
		{
			shredIterations = 1;
		}
		else if(rdoStandardShred.Active == true)
		{
			shredIterations = 3;
		}
		else if(rdoExtremeShred.Active == true)
		{
			shredIterations = 7;
		}

		if(chkTrash.Active == false && chkThumbnails.Active == false &&
		chkMru.Active == false && chkBashHist.Active == false)
		{
			// DOES NOT CATCH BASH ERROR BUT WILL CATCH INCOMPATIBLE SHELL
			MessageDialog mBoxError = new MessageDialog (this,
				DialogFlags.DestroyWithParent,
				MessageType.Error,
				ButtonsType.Close,
				"No items are selected. Please try again.");
			mBoxError.Run();
			mBoxError.Destroy();
		}
		else
		{
			// CREATE AND LAUNCH 'SECURE DELETION' THREAD
			Thread secureDeletionThread = new Thread(deletionThread);
			secureDeletionThread.Start();

			// BEGIN PULSING THE PROGRESS BAR
			progressBarPulse();
		}
	}

	protected void btnRefreshPackageCatalog_Click (object sender, System.EventArgs e)
	{
		clearConsole();

		// DECLARE OBJECTS AND VARIABLES
		Thread aptGetUpdateThread = new Thread(updateThread);

		// ADDRESS REPRESENTS UBUNTU UPDATE MIRROR, FAILING TO CONNECT
		// WILL HALT THE UPDATE PROCESS TO PROTECT THE SYSTEM
		string host = "archive.ubuntu.com";
		Ping p = new Ping();

		try
		{
			PingReply reply = p.Send(host, 1000);
		    if (reply.Status == IPStatus.Success)
			{
				// INTERNET CONENCTION EXISTS
				// INSTALL SOFTWARE
				aptGetUpdateThread.Start();

				progressBarPulse();
			}
		}
		catch
		{
			// DISPLAY ERROR WINDOW
			noInternetConnection();
		}
	}

	protected void btnInstallUpdates_Click (object sender, System.EventArgs e)
	{
		clearConsole();

		// DECLARE OBJECTS AND VARIABLES
		Thread aptGetUpgradeThread = new Thread(upgradeThread);

		// ADDRESS REPRESENTS UBUNTU UPDATE MIRROR, FAILING TO CONNECT
		// WILL HALT THE UPDATE PROCESS TO PROTECT THE SYSTEM
		string host = "archive.ubuntu.com";
		Ping p = new Ping();

		try
		{
			PingReply reply = p.Send(host, 1000);
		    if (reply.Status == IPStatus.Success)
			{
				// INTERNET CONENCTION EXISTS
				// INSTALL SOFTWARE
				aptGetUpgradeThread.Start();

				progressBarPulse();
			}
		}
		catch
		{
			// DISPLAY ERROR WINDOW
			noInternetConnection();
		}
	}

	protected void btnActivateFirewall_Click (object sender, System.EventArgs e)
	{
		clearConsole();

		// CREATE AND LAUNCH 'ENABLE FIREWALL' THREAD
		Thread enableUfwThread = new Thread(enableFirewallThread);
		enableUfwThread.Start();

		// BEGIN PULSING THE PROGRESS BAR
		progressBarPulse();
	}

	protected void btnDeavtivateFirewall_Click (object sender, System.EventArgs e)
	{
		clearConsole();

		// CREATE AND LAUNCH 'DISABLE FIREWALL' THREAD
		Thread disableUfwthread = new Thread(disableFirewallThread);
		disableUfwthread.Start();

		// BEGIN PULSING THE PROGRESS BAR
		progressBarPulse();
	}

	protected void btnVirusScan_Click (object sender, System.EventArgs e)
	{
		clearConsole();

		// DECLARE OBJECTS AND VARIABLES
		Thread clamavThread = new Thread(clamScanThread);
		Thread clamavInstallationThread = new Thread(clamavInstallThread);

		// ADDRESS REPRESENTS UBUNTU UPDATE MIRROR, FAILING TO CONNECT
		// WILL HALT THE UPDATE PROCESS TO PROTECT THE SYSTEM
		string host = "archive.ubuntu.com";
		Ping p = new Ping();

		if (File.Exists(CLAMSCAN_BIN))
		{
			// CLAMAV IS INSTALLED, BEGIN VIRUS SCAN
			clamavThread.Start();

			progressBarPulse();
		}
		else
		{
			// CLAMAV IS NOT INSTALLED
			// DETECT NETWORK CONNECTION BEFORE ATTEMPTING INSTALLATION
			try
			{
				PingReply reply = p.Send(host, 1000);
			    if (reply.Status == IPStatus.Success)
				{
					// INTERNET CONENCTION EXISTS
					// INSTALL SOFTWARE
					clamavInstallationThread.Start();

					progressBarPulse();
				}
			}
			catch
			{
				// INTERNET CONNECTION DOES NOT EXIST
				MessageDialog mBoxError = new MessageDialog (this,
					DialogFlags.DestroyWithParent,
					MessageType.Error,
					ButtonsType.Close,
					"ClamAV is not installed. An active Internet connection was not detected. Please connect to the Internet to install ClamAV.");
				mBoxError.Run();
				mBoxError.Destroy();
			}
		}
	}

	protected void btnFreshclam_Click (object sender, System.EventArgs e)
	{
		clearConsole();

		// UPDATE VIRUS DEFINITIONS
		Thread updateClamavThread = new Thread(freshclamThread);

		// ADDRESS REPRESENTS CLAMAV UPDATE MIRROR, FAILING TO CONNECT
		// WILL HALT THE UPDATE PROCESS TO PROTECT THE SYSTEM
		string host = "clamav.net";
		Ping p = new Ping();

		try
		{
			PingReply reply = p.Send(host, 1000);
		    if (reply.Status == IPStatus.Success)
			{
				// INTERNET CONNECTION EXISTS
				updateClamavThread.Start();

				// BEGIN PULSING THE PROGRESS BAR
				progressBarPulse();
			}
		}
		catch
		{
			// DISPLAY ERROR WINDOW
			noInternetConnection();
		}
	}

	#endregion

	//**************************************************************************
	// MAIN METHODS CODE BLOCK
	//**************************************************************************
	#region mainMethods

	public void clearConsole()
	{
		// CALL FOR EACH BUTTON TO ERASE PREVIOUS OUTPUT
		txtOutput.Buffer.Text = "";
	}

	public void progressBarPulse ()
	{
		// PULSE THE PROGRESS BAR UNTIL TASK FINISHES
		// AND THE LOCK IS RELEASED
		while (taskLocked == true)
		{
			progBar.Pulse ();
			progBar.Text = progBarText;
			Thread.Sleep (TASK_DELAY);
			Main.IterationDo (false);
			Thread.Sleep (TASK_DELAY);
			Main.IterationDo (false);
		}

		progBar.Fraction = 0;
		taskLocked = true;
	}

	public void disableAllTheThings()
	{
		// DISABLE ALL THE THINGS!
		// http://hyperboleandahalf.blogspot.co.uk/

		btnClear.Sensitive = false;
		btnDelete.Sensitive = false;
		btnDeavtivateFirewall.Sensitive = false;
		btnActivateFirewall.Sensitive = false;
		btnVirusScan.Sensitive = false;
		btnFreshclam.Sensitive = false;
	}

	public void enableAllTheThings()
	{
		// ENABLE ALL THE THINGS!
		// http://hyperboleandahalf.blogspot.co.uk/

		btnClear.Sensitive = true;
		btnDelete.Sensitive = true;
		btnDeavtivateFirewall.Sensitive = true;
		btnActivateFirewall.Sensitive = true;
		btnVirusScan.Sensitive = true;
		btnFreshclam.Sensitive = true;
	}

	public void deletionThread()
	{
		lock(deletionThreadLock)
		{
			disableAllTheThings();

			// * ERASE ITEMS ACCORDING TO CHECKBOXES ACTIVATED
			// * SCREEN MAY FLASH AS SYSTEM TERMINAL OPENS AND CLOSES
			if(chkTrash.Active == true)
			{
				progBarText = "Erasing Trash / Wastebasket. Please wait...";
				txtOutput.Buffer.InsertAtCursor("Now securely shredding Trash / Wastebasket at " +
					shredIterations + " pass(es). Please wait...\n");
				deletionMethods.shredTrash(shredIterations);
				txtOutput.Buffer.InsertAtCursor("Trash / Wastebasket now securely deleted.\n\n");
			}

			if (chkThumbnails.Active == true)
			{
				progBarText = "Erasing image cache. Please wait...";
				txtOutput.Buffer.InsertAtCursor("Now securely shredding the image cache at " +
					shredIterations + " pass(es). Please wait...\n");
				deletionMethods.shredThumbnails(shredIterations);
				txtOutput.Buffer.InsertAtCursor("Image cache now securely deleted.\n\n");
			}

			if (chkMru.Active == true)
			{
				progBarText = "Erasing most recently used list. Please wait...";
				txtOutput.Buffer.InsertAtCursor("Now securely shredding the most recently used list at " +
					shredIterations + " pass(es). Please wait...\n");
				deletionMethods.shredRecentlyUsedFiles(shredIterations);
				txtOutput.Buffer.InsertAtCursor("Most recently used list now securely deleted.\n\n");
			}

			if (chkBashHist.Active == true)
			{
				progBarText = "Erasing command-line history. Please wait...";
				txtOutput.Buffer.InsertAtCursor("Now securely shredding the command-line history at " +
					shredIterations + " pass(es). Please wait...\n");
				deletionMethods.shredBashHistory(shredIterations);
				txtOutput.Buffer.InsertAtCursor("Command-line history now securely deleted.\n\n");
			}

			taskLocked = false;

			progBar.Text = "Selected item(s) are now securely erased.";
			notificationText = "Secure deletion complete";

			taskComplete();
			enableAllTheThings();
		}
	}

	public void updateThread()
	{
		// APT-GET: UPDATE PACKAGE CATALOG
		lock(updateThreadLock)
		{
			disableAllTheThings();

			progBarText = "Refreshing package catalog. Please wait...";
			txtOutput.Buffer.InsertAtCursor("Asking for permission to refresh package catalog.\n\n");
			txtOutput.Buffer.InsertAtCursor("Now refreshing package catalog...\n\n");

			updateMethods.aptGetUpdateGksu();

			taskLocked = false;

			txtOutput.Buffer.InsertAtCursor("Package catalog refresh complete.");
			progBar.Text = "Package catalog has been updated.";
			notificationText = "Package catalog updated";

			taskComplete();

			enableAllTheThings();
		}
	}

	public void upgradeThread()
	{
		// APT-GET: UPGRADE DISTRIBUTION
		lock(upgradeThreadLock)
		{
			disableAllTheThings();

			progBarText = "Updating system. Please wait...";
			txtOutput.Buffer.InsertAtCursor("Asking for permission to perform system update.\n\n");
			txtOutput.Buffer.InsertAtCursor("Now performing system update...\n\n");

			updateMethods.aptGetUpgradeGksu();

			taskLocked = false;

			txtOutput.Buffer.InsertAtCursor("System update complete.");
			progBar.Text = "System has been updated.";
			notificationText = "System update complete";

			taskComplete();

			enableAllTheThings();
		}
	}

	public void enableFirewallThread()
	{
		// UFW: ENABLE FIREWALL
		lock(enableFirewallThreadLock)
		{
			disableAllTheThings();

			progBarText = "Activating firewall. Please wait...";
			txtOutput.Buffer.InsertAtCursor("Asking for permission to enable 'ufw' firewall.\n\n");
			txtOutput.Buffer.InsertAtCursor("Now activating 'ufw' firewall...\n\n");

			firewallMethods.ufwEnableGksu();

			txtOutput.Buffer.InsertAtCursor("System firewall 'ufw' is now enabled and will activate on system startup.");

			taskLocked = false;

			progBar.Text = "Firewall is activated and will launch on startup.";
			notificationText = "Firewall now enabled";

			taskComplete();

			enableAllTheThings();
		}
	}

	public void disableFirewallThread()
	{
		// UFW: DISABLE FIREWALL
		lock(disableFirewallThreadLock)
		{
			disableAllTheThings();

			progBarText = "Dectivating firewall. Please wait...";
			txtOutput.Buffer.InsertAtCursor("Asking for permission to disable 'ufw' firewall.\n\n");
			txtOutput.Buffer.InsertAtCursor("Now deactivating 'ufw' firewall...\n\n");

			firewallMethods.ufwDisableGksu();

			txtOutput.Buffer.InsertAtCursor("System firewall 'ufw' is now disabled and will not activate on system startup.");

			taskLocked = false;

			progBar.Text = "Firewall is deactivated and will not launch on startup.";
			notificationText = "Firewall now disabled";

			taskComplete();

			enableAllTheThings();
		}
	}

	public void freshclamThread()
	{
		// CLAMAV: UPDATE VIRUS DATABASE
		lock(freshclamThreadLock)
		{
			disableAllTheThings();

			progBarText = "Updating ClamAV virus database...";
			txtOutput.Buffer.InsertAtCursor("Asking for permission to update ClamAV virus database.\n\n");
			txtOutput.Buffer.InsertAtCursor("Now updating ClamAV virus database...\n\n");

			antivirusMethods.freshclamGksu();

			txtOutput.Buffer.InsertAtCursor("ClamAV virus database update complete.");

			taskLocked = false;

			progBar.Text = "ClamAV virus database now fully updated.";
			notificationText = "ClamAV update complete";

			taskComplete();

			enableAllTheThings();
		}
	}

	public void clamScanThread()
	{
		// CLAMAV: LAUNCH SELECTED SCAN
		lock(clamscanThreadLock)
		{
			disableAllTheThings();

			clamscanGtk();

			taskLocked = false;

			taskComplete();

			enableAllTheThings();
		}
	}

	public void clamavInstallThread()
	{
		// INSTALL 'CLAMAV'
		lock(clamscanInstallLock)
		{
			disableAllTheThings();

			clamAvNotInstalled();
			clamscanGtk();

			taskLocked = false;

			taskComplete();

			enableAllTheThings();
		}
	}

	public void clamscanGtk()
	{
		// CLEAR TEXT OUTPUT
		txtOutput.Buffer.Text = "";

		// OUTPUT TEXT TO PROGRESS BAR AND TEXT WINDOW
		progBarText = "Performing ClamAV Virus Scan. Please Wait...";
		txtOutput.Buffer.InsertAtCursor
			("Erasing old log and launching external Terminal window to perform virus scan.\n\n" +
			 "Close Terminal window to abort the process or end if scan is complete.");

		// BEGIN SCAN DEPENDING ON WHICH RADIO BUTTON IS ACTIVE
		if(rdoClamavHomeFolder.Active == true)
		{
			antivirusMethods.clamScanHome();
		}
		else if(rdoClamavExternal.Active == true)
		{
			antivirusMethods.clamScanExternal();
		}
		else if(rdoClamavFull.Active == true)
		{
			antivirusMethods.clamScanFull();
		}

		// OUTPUT TEXT TO NOTIFICATION, PROGRESS BAR AND TEXT WINDOW
		txtOutput.Buffer.InsertAtCursor
			("\n\nVirus scan is complete. Log recorded in your home directory.");
		notificationText = "Virus Scan Complete";
		progBar.Text = "ClamAV scan complete. Check output below.";
	}

	public void clamAvNotInstalled()
	{
		//CLAMAV IS NOT INSTALLED INFORM USER AND INSTALL

		txtOutput.Buffer.Text = "";

		progBarText = "ClamAV is not installed. Beginning installation...";
		antivirusMethods.clamavNotInstalledMsg();
		txtOutput.Buffer.InsertAtCursor("ClamAV is not installed.");


		txtOutput.Buffer.InsertAtCursor("\nClamAV is now installing...");
		progBarText = "Installing ClamAV...";
		antivirusMethods.installClamavGksu();
		txtOutput.Buffer.InsertAtCursor("\nClamAV is now installed.");


		txtOutput.Buffer.InsertAtCursor("\nClamAv is now updating virus definitions...");
		progBarText = "Updating ClamAV virus database...";
		antivirusMethods.freshclamGksu();
		txtOutput.Buffer.InsertAtCursor("\nClamAV virus definitions are now updated.");


		txtOutput.Buffer.InsertAtCursor("\n\nClamAV is now fully installed and configured.");
		progBar.Text = "ClamAV is now fully installed and configured.";
		notificationText = "ClamAV is now installed";
	}

	#endregion

	//**************************************************************************
	// PROGRAM EXIT EVENT BLOCK
	//**************************************************************************
	#region programExitEvent

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	#endregion
}

// End of File.