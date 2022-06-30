using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Nevron.Nov.Barcode;
using Nevron.Nov.Chart;
using Nevron.Nov.Diagram;
using Nevron.Nov.Dom;
using Nevron.Nov.Graphics;
using Nevron.Nov.Grid;
using Nevron.Nov.IO;
using Nevron.Nov.Schedule;
using Nevron.Nov.Text;
using Nevron.Nov.UI;
using Nevron.Nov.Windows.Forms;

namespace Nevron.Nov.Examples.WinForm
{
	static class Program
	{
		/// <summary>
		/// The main item point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            try
            {
                // In .NET core apps high dpi awareness is controlled from code
                Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

				// Install Nevron Open Vision for Windows Forms
				NNovApplicationInstaller.Install(
					NTextModule.Instance,
					NChartModule.Instance,
					NDiagramModule.Instance,
					NScheduleModule.Instance,
					NGridModule.Instance,
                    NBarcodeModule.Instance);

                // use this to Enable/Disable GPU rendering of all NOV Content
                NApplication.EnableGPURendering = true;

#if !DEBUG
				// Change the Resources folder to the one where the NOV installer places the resources, which is by default:
				// C:\Program Files (x86)\Nevron Software\Nevron Open Vision 2021.1\Resources
                string resourcesPath = NPath.Current.Normalize(NPath.Current.Combine(NApplication.ResourcesFolder.Path, @"..\..\..\..\..\Resources"));
                NApplication.ResourcesFolder = NFileSystem.Current.GetFolder(resourcesPath);

                if (!Directory.Exists(resourcesPath))
                {
                    Console.Write("Failed to locate resources path ["  + resourcesPath + "]");
                }
#endif

                // show the main form
                bool startWithNovWindow = false;
                if (startWithNovWindow)
                {
                    // create a NOV top level window
                    NTopLevelWindow window = NApplication.CreateTopLevelWindow();
					window.BackgroundFill = new NColorFill(NColor.White);
                    window.Content = new NExamplesContent();
                    window.Closed += OnWindowClosed;
                    window.Title = "Nevron Open Vision Examples for Windows Forms";
                    window.AllowXResize = true;
                    window.AllowYResize = true;
                    window.ShowInTaskbar = true;
                    window.Modal = true;
                    window.PreferredSize = new NSize(500, 500);
                    window.StartPosition = ENWindowStartPosition.CenterScreen;
                    window.Open();
                    
                    // run the application
                    ApplicationContext context = new ApplicationContext();
                    Application.Run(context);
                }
                else
                {
                    // create a WinForms form  
                    Form form = new Form();

                    var resourceUris = Assembly.GetEntryAssembly().GetCustomAttributes();

                    // set form icon
                    try
                    {
                        using (Stream stream = typeof(Program).Assembly.GetManifestResourceStream("Nevron.Nov.Examples.WinForm.Resources.NevronOpenVision.ico"))
                        {
                            if (stream != null)
                            {
                                Icon icon = new Icon(stream);
                                form.Icon = icon;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        NTrace.WriteLine("Failed to load application icon.");
                    }

                    // set form title and state
                    form.Text = "Nevron Open Vision Examples for Windows Forms";
                    form.WindowState = FormWindowState.Maximized;

                    // place a NOV WinForms Control that contains an NExampleContent widget
                    NNovWidgetHost<NExamplesContent> host = new NNovWidgetHost<NExamplesContent>();
                    host.Dock = DockStyle.Fill;
                    form.Controls.Add(host);

                    // run the form
                    Application.Run(form);
                }
            }
            catch (Exception ex)
            {
                NTrace.WriteException("Exception in Main", ex);
            }
		}

        private static void OnWindowClosed(NEventArgs arg)
        {
            if (arg.EventPhase != ENEventPhase.AtTarget)
                return;

            Application.Exit(); 
        }
	}
}