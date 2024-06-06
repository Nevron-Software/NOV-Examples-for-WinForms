using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Nevron.Nov.Barcode;
using Nevron.Nov.Chart;
using Nevron.Nov.Compiler;
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
		/// <param name="args">Command line argumnets. Used to launch a specific example.
		/// For example: "nov-winforms://NDiagramDesignerExample".</param>
		[STAThread]
		static void Main(string[] args)
		{
            try
            {
                // In .NET core apps high dpi awareness is controlled via code
                Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Use this to Enable/Disable GPU rendering of all NOV Content
                NApplication.EnableGPURendering = true;

                // Install Nevron Open Vision for Windows Forms
                NNovApplicationInstaller.Install(
					NTextModule.Instance,
					NChartModule.Instance,
					NDiagramModule.Instance,
					NScheduleModule.Instance,
					NGridModule.Instance,
                    NBarcodeModule.Instance);

                // Optional: If you intend to use NCodeAssembly (for example for Family Tree Shapes in in NOV Diagram for .NET), 
                // you need to specify the compiler service used to compile them.
                NApplication.CompilerService = new NRoslynCompilerService();

#if DEBUG
				string resourcesPath = NPath.Current.Normalize(NPath.Current.Combine(NApplication.ResourcesFolder.Path, @"..\..\..\NOV\Resources"));
#else
				// Change the Resources folder to the one where the NOV installer places the resources, which is by default:
				// C:\Program Files (x86)\Nevron Software\Nevron Open Vision [Version]\Resources
				string resourcesPath = NPath.Current.Normalize(NPath.Current.Combine(NApplication.ResourcesFolder.Path, @"..\..\..\..\Resources"));
#endif

                NApplication.ResourcesFolder = NFileSystem.Current.GetFolder(resourcesPath);
				if (!Directory.Exists(resourcesPath))
                {
                    Console.Write("Failed to locate resources path ["  + resourcesPath + "]");
                }

                // Create the examples content
                NExamplesContent examplesContent = new NExamplesContent();
                examplesContent.LinkProcessor = new NWfExampleLinkProcessor();

                // Show the main form
                bool startWithNovWindow = false;
                if (startWithNovWindow)
                {
                    // Create a NOV top level window
                    NTopLevelWindow window = NApplication.CreateTopLevelWindow();
					window.BackgroundFill = new NColorFill(NColor.White);
                    window.Content = examplesContent;
                    window.Closed += OnWindowClosed;
                    window.Title = "Nevron Open Vision Examples for Windows Forms";
                    window.AllowXResize = true;
                    window.AllowYResize = true;
                    window.ShowInTaskbar = true;
                    window.Modal = true;
                    window.PreferredSize = new NSize(1360, 740);
                    window.StartPosition = ENWindowStartPosition.CenterScreen;

					if (args.Length == 1)
					{
						NDiagramModule.Instance.PredefinedLibrariesLoaded += (sender, e) =>
						{
							// Navigate to the URI after the predefined libraries of NOV Diagram has been loaded
							examplesContent.NavigateToExampleUri(args[0]);
						};
					}

					window.Open();

                    // Run the application
                    ApplicationContext context = new ApplicationContext();
                    Application.Run(context);
                }
                else
                {
                    // Create a WinForms form  
                    Form form = new Form();

                    // Set form icon
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
                        NTrace.WriteException("Failed to load application icon.", ex);
                    }

					double scaleFactor = form.DeviceDpi / 96.0;

                    // Set form title and state
                    form.Text = "Nevron Open Vision Examples for Windows Forms";
					form.Size = new Size((int)Math.Round(1360 * scaleFactor), (int)Math.Round(740 * scaleFactor));
                    form.WindowState = FormWindowState.Maximized;
					form.StartPosition = FormStartPosition.CenterScreen;

                    // Place a NOV WinForms Control that contains an NExampleContent widget
                    NNovWidgetHost<NExamplesContent> host = new NNovWidgetHost<NExamplesContent>(examplesContent);
                    host.Dock = DockStyle.Fill;
                    form.Controls.Add(host);

                    if (args.Length == 1)
                    {
						NDiagramModule.Instance.PredefinedLibrariesLoaded += (sender, e) =>
						{
							// Navigate to the URI after the predefined libraries of NOV Diagram has been loaded
							examplesContent.NavigateToExampleUri(args[0]);
						};
                    }

                    // Run the form
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