﻿using Nevron.Nov.Diagram;
using Nevron.Nov.Diagram.Export;
using Nevron.Nov.Diagram.Shapes;
using Nevron.Nov.Dom;
using Nevron.Nov.Graphics;
using Nevron.Nov.UI;

namespace Nevron.Nov.Examples.Diagram
{
    public class NRasterImageExportExample : NExampleBase
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NRasterImageExportExample()
        {
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static NRasterImageExportExample()
        {
            NRasterImageExportExampleSchema = NSchema.Create(typeof(NRasterImageExportExample), NExampleBaseSchema);
        }

        #endregion

        #region Example

        protected override NWidget CreateExampleContent()
        {
            // Create a simple drawing
            NDrawingViewWithRibbon drawingViewWithRibbon = new NDrawingViewWithRibbon();
            m_DrawingView = drawingViewWithRibbon.View;

            m_DrawingView.Document.HistoryService.Pause();
            try
            {
                InitDiagram(m_DrawingView.Document);
            }
            finally
            {
                m_DrawingView.Document.HistoryService.Resume();
            }

            return drawingViewWithRibbon;
        }
        protected override NWidget CreateExampleControls()
        {
            NStackPanel stackPanel = new NStackPanel();

            NButton showImageExportButton = new NButton("Show Raster Image Export Dialog...");
            showImageExportButton.Click += OnShowImageExportButtonClick;
            stackPanel.Add(showImageExportButton);

            NButton copyImageToClipboardButton = new NButton("Copy Raster Image to Clipboard");
            copyImageToClipboardButton.Click += OnCopyImageToClipboardButtonClick;
            stackPanel.Add(copyImageToClipboardButton);

            NButton saveAsRasterImageFileButton = new NButton("Save as Raster Image File...");
            saveAsRasterImageFileButton.Click += OnSaveAsRasterImageFileButtonClick;
            stackPanel.Add(saveAsRasterImageFileButton);

			NButton saveAsVectorImageFileButton = new NButton("Save as Vector Image File...");
			saveAsVectorImageFileButton.Click += OnSaveAsVectorImageFileButtonClick;
			stackPanel.Add(saveAsVectorImageFileButton);

            return stackPanel;
        }
        protected override string GetExampleDescription()
        {
            return @"
<p>
    Demonstrates the image exporters, with the help of which you can export the active page or any portion of it
	to a raster (PNG, JPG, BMP, GIF) or a vector (SVG) image.
</p>
            ";
        }

        private void InitDiagram(NDrawingDocument drawingDocument)
        {
            NDrawing drawing = drawingDocument.Content;
            NPage activePage = drawing.ActivePage;

            drawing.ScreenVisibility.ShowGrid = false;
            drawing.ScreenVisibility.ShowPorts = false;

            NBasicShapeFactory basisShapes = new NBasicShapeFactory();
			NFlowchartShapeFactory flowChartingShapes = new NFlowchartShapeFactory();
            NConnectorShapeFactory connectorShapes = new NConnectorShapeFactory();

            NShape nonPrintableShape = basisShapes.CreateShape(ENBasicShape.Rectangle);
            nonPrintableShape.Text = "Non printable shape";
            nonPrintableShape.AllowPrint = false;
            nonPrintableShape.Geometry.Fill = new NColorFill(NColor.Tomato);
            nonPrintableShape.SetBounds(50, 50, 150, 50);
            activePage.Items.Add(nonPrintableShape);

            NShape isLifeGood = flowChartingShapes.CreateShape(ENFlowchartingShape.Decision);
            isLifeGood.Text = "Is Life Good?";
            isLifeGood.SetBounds(300, 50, 150, 100);
            isLifeGood.Geometry.Fill = new NColorFill(NColor.LightSkyBlue);
            activePage.Items.Add(isLifeGood);

            NShape goodShape = flowChartingShapes.CreateShape(ENFlowchartingShape.Termination);
            goodShape.Text = "Good";
            goodShape.SetBounds(200, 200, 100, 100);
            goodShape.Geometry.Fill = new NColorFill(NColor.Gold);
            activePage.Items.Add(goodShape);

            NShape changeSomething = flowChartingShapes.CreateShape(ENFlowchartingShape.Process);
            changeSomething.Text = "Change Something";
            changeSomething.Geometry.Fill = new NColorFill(NColor.Thistle);
            changeSomething.SetBounds(450, 200, 100, 100);
            activePage.Items.Add(changeSomething);

            NShape yesConnector = connectorShapes.CreateShape(ENConnectorShape.RoutableConnector);
            yesConnector.Text = "Yes";
            yesConnector.GlueBeginToPort(isLifeGood.GetPortByName("Left"));
            yesConnector.GlueEndToPort(goodShape.GetPortByName("Top"));
            activePage.Items.Add(yesConnector);

            NShape noConnector = connectorShapes.CreateShape(ENConnectorShape.RoutableConnector);
            noConnector.Text = "No";
            noConnector.GlueBeginToPort(isLifeGood.GetPortByName("Right"));
            noConnector.GlueEndToPort(changeSomething.GetPortByName("Top"));
            activePage.Items.Add(noConnector);

            NShape gobackConnector = connectorShapes.CreateShape(ENConnectorShape.RoutableConnector);
            gobackConnector.GlueBeginToPort(changeSomething.GetPortByName("Right"));
            gobackConnector.GlueEndToPort(isLifeGood.GetPortByName("Top"));
            activePage.Items.Add(gobackConnector);
        }

        #endregion

        #region Event Handlers

        private void OnShowImageExportButtonClick(NEventArgs arg)
        {
			NDrawingRasterImageExporter imageExporter = new NDrawingRasterImageExporter(m_DrawingView.Drawing);
            imageExporter.ShowDialog(DisplayWindow, true);
        }
        private void OnCopyImageToClipboardButtonClick(NEventArgs arg)
        {
			NDrawingRasterImageExporter imageExporter = new NDrawingRasterImageExporter(m_DrawingView.Drawing);
            imageExporter.CopyToClipboard();
        }
        private void OnSaveAsRasterImageFileButtonClick(NEventArgs arg)
        {
			NDrawingRasterImageExporter imageExporter = new NDrawingRasterImageExporter(m_DrawingView.Drawing);
            imageExporter.SaveAsImage();
        }
		private void OnSaveAsVectorImageFileButtonClick(NEventArgs arg)
		{
			NDrawingVectorImageExporter imageExporter = new NDrawingVectorImageExporter(m_DrawingView.Drawing);
			imageExporter.SaveAsImage();
		}

        #endregion

        #region Fields

        private NDrawingView m_DrawingView;

        #endregion

        #region Schema

        /// <summary>
        /// Schema associated with NRasterImageExportExample.
        /// </summary>
        public static readonly NSchema NRasterImageExportExampleSchema;

        #endregion
    }
}
