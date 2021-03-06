using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using View;
using System.Collections.Generic;
using XleGenerator;
using System.Windows.Forms;

//using Microsoft.VisualStudio.Shell.Interop;
//using DevExpress.DXCore.Interop;

namespace XnaLevelEditor
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
        private Dictionary<Window, MainUserControl> mainUserControls;
        private WindowVisibilityEvents windowVisibilityEvents;
        private DTE2 applicationObject;
        private AddIn _addInInstance;
        private string guidString;
        private Windows2 windows2;
        private Type userControlType;
        private Assembly asm;

		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;

			if(connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
				object []contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)applicationObject.Commands;
				string toolsMenuName = "Tools";

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
				Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)applicationObject.CommandBars)["MenuBar"];

				//Find the Tools command bar on the MenuBar command bar:
				CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
				CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				try
				{
					//Add a command to the Commands collection:
					Command command = commands.AddNamedCommand2(_addInInstance, "XnaLevelEditor", "XnaLevelEditor", "Executes the command for XnaLevelEditor", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					//Add a control for the command to the tools menu:
					if((command != null) && (toolsPopup != null))
					{
						command.AddControl(toolsPopup.CommandBar, 1);
					}
				}
				catch(System.ArgumentException)
				{
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
				}
			}
            else if (connectMode == ext_ConnectMode.ext_cm_AfterStartup)
            {
                windows2 = (Windows2)applicationObject.Windows;
                userControlType = typeof(MainUserControl);
                asm = Assembly.GetAssembly(userControlType);

                mainUserControls = new Dictionary<Window, MainUserControl>();

                Events2 events = (Events2)applicationObject.Events;
                windowVisibilityEvents = events.get_WindowVisibilityEvents();
                windowVisibilityEvents.WindowShowing += new _dispWindowVisibilityEvents_WindowShowingEventHandler(windowVisibilityEvents_WindowShowing);
                windowVisibilityEvents.WindowHiding += new _dispWindowVisibilityEvents_WindowHidingEventHandler(windowVisibilityEvents_WindowHiding);
            }
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "XnaLevelEditor.Connect.XnaLevelEditor")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if(commandName == "XnaLevelEditor.Connect.XnaLevelEditor")
                {
                    handled = true;

                    try
                    {
                        #region Load Tool Window
                        ClassManager.applicationObject = applicationObject;
                        ChooseClassForm form = new ChooseClassForm();
                        if (form.ShowDialog() != DialogResult.OK)
                            return;
                        ClassManager classManager = new XleGenerator.ClassManager(form.ProjectName, form.ClassName, form.IsOpen);
                    
                        object programmableObject = null;
                        guidString = "{A5431877-4E06-44FC-A59C-5BDE534A8F4F}";//{9FFC9D9B-1F39-4763-A2AF-66AED06C799E}";
                        //guidString = Guid.NewGuid().ToString("B");
                        Window window = windows2.CreateToolWindow2(_addInInstance, asm.Location,
                            userControlType.FullName,
                            "Level Editor", guidString, ref programmableObject);
                        MainUserControl mainUserControl = (MainUserControl)programmableObject;
                        window.IsFloating = false;
                        window.Linkable = false;
                        window.WindowState = vsWindowState.vsWindowStateMaximize;
                        #endregion

                        mainUserControl.ApplicationObject = applicationObject;
                        mainUserControl.Window = window;
                        mainUserControl.IsOpen = form.IsOpen;
                        mainUserControl._ClassManager = classManager;
                        window.Visible = true;

                        mainUserControls.Add(window, mainUserControl);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                    }

					return;
				}
			}
		}

        private void windowVisibilityEvents_WindowShowing(Window window)
        {
            int x = 1;
        }

        private void windowVisibilityEvents_WindowHiding(Window window)
        {
            if (mainUserControls.ContainsKey(window))
            {
                MainUserControl mainUserControl = mainUserControls[window];
                mainUserControls.Remove(window);
                mainUserControl.Dispose();
            }
        }

        //private static IVsWindowFrame GetWindowFrameFromGuid(Guid guid)
        //{
        //    Guid slotGuid = guid;
        //    IVsWindowFrame wndFrame;
        //    VisualStudioServices.VsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFrameOnly, ref slotGuid, out wndFrame);
        //    return wndFrame;
        //}

        //private static IVsWindowFrame GetWindowFrameFromWindow(EnvDTE.Window window)
        //{
        //    if (window == null)
        //        return null;
        //    if (window.ObjectKind == null || window.ObjectKind == String.Empty)
        //        return null;
        //    return GetWindowFrameFromGuid(new Guid(window.ObjectKind));
        //}

        //public static void ShowWindowDocked()
        //{
        //    EnvDTE.Window window = CodeRush.ToolWindows.Show(typeof(YourToolWindowClassName));
        //    if (window != null)
        //    {
        //        IVsWindowFrame wndFrame = GetWindowFrameFromWindow(window);
        //        if (wndFrame != null)
        //            wndFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_MdiChild);
        //    }
        //}
	}
}