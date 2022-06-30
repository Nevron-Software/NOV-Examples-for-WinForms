﻿using System;

using Nevron.Nov.Chart;
using Nevron.Nov.Chart.Tools;
using Nevron.Nov.Dom;
using Nevron.Nov.Graphics;
using Nevron.Nov.UI;

namespace Nevron.Nov.Examples.Chart
{
	/// <summary>
	/// Daily Schdule Work Calendar Example
	/// </summary>
	public class NDailyScheduleWorkCalendarExample : NExampleBase
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public NDailyScheduleWorkCalendarExample()
		{
			
		}
		/// <summary>
		/// Static constructor
		/// </summary>
		static NDailyScheduleWorkCalendarExample()
		{
			NDailyScheduleWorkCalendarExampleSchema = NSchema.Create(typeof(NDailyScheduleWorkCalendarExample), NExampleBaseSchema);
		}

		#endregion

		#region Example

		protected override NWidget CreateExampleContent()
		{
			NChartView chartView = new NChartView();
			chartView.Surface.CreatePredefinedChart(ENPredefinedChartType.Cartesian);

			// configure title
			chartView.Surface.Titles[0].Text = "Daily Schedule Work Calendar";

			// configure chart
			m_Chart = (NCartesianChart)chartView.Surface.Charts[0];
			m_Chart.SetPredefinedCartesianAxes(ENPredefinedCartesianAxis.XOrdinalYLinear);

			NRangeSeries ranges = new NRangeSeries();
			m_Chart.Series.Add(ranges);

			ranges.DataLabelStyle = new NDataLabelStyle(false);
			ranges.UseXValues = true;

			DateTime dt = new DateTime(2014, 4, 14);
			Random rand = new Random();

			NRangeTimelineScale rangeTimeline = new NRangeTimelineScale();
			rangeTimeline.EnableCalendar = true;
			rangeTimeline.InflateViewRangeEnd = false;
			rangeTimeline.InflateViewRangeBegin = false;
			m_Chart.Axes[ENCartesianAxis.PrimaryX].Scale = rangeTimeline;

			NLinearScale yScale = (NLinearScale)m_Chart.Axes[ENCartesianAxis.PrimaryY].Scale;
			yScale.MajorGridLines.Visible = true;

			// add interlaced strip
			NScaleStrip strip = new NScaleStrip(new NColorFill(NColor.Beige), null, true, 0, 0, 1, 1);
			strip.Interlaced = true;
			yScale.Strips.Add(strip);

			yScale.Title.Text = "Daily Workload in %";
			
			NWorkCalendar workCalendar = rangeTimeline.Calendar;
			NDateTimeRangeRule dateTimeRangeRule = null;

			for (int i = 0; i < 120; i++)
			{
				int hourOfTheDay = i % 24;

				if (hourOfTheDay < 8 || hourOfTheDay > 18)
				{
					DateTime curDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

					if (dateTimeRangeRule != null)
					{
						if (dateTimeRangeRule.Range.Begin != curDate)
						{
							dateTimeRangeRule = null;
						}
					}

					if (dateTimeRangeRule == null)
					{
						dateTimeRangeRule = new NDateTimeRangeRule(new NDateTimeRange(curDate, curDate + new TimeSpan(24, 0, 0)), true);
						workCalendar.Rules.Add(dateTimeRangeRule);
					}

					dateTimeRangeRule.Schedule.SetHourRange(dt.Hour, dt.Hour + 1, true);
				}
				else
				{
					ranges.DataPoints.Add(new NRangeDataPoint(NDateTimeHelpers.ToOADate(dt), 0, NDateTimeHelpers.ToOADate(dt + new TimeSpan(1, 0, 0)), rand.NextDouble() * 70 + 30.0d));
				}

				dt += new TimeSpan(1, 0, 0);
			}

			ConfigureInteractivity(m_Chart);

			chartView.Document.StyleSheets.ApplyTheme(new NChartTheme(ENChartPalette.Bright, false));

			return chartView;
		}
		protected override NWidget CreateExampleControls()
		{
			NStackPanel stack = new NStackPanel();
			NUniSizeBoxGroup boxGroup = new NUniSizeBoxGroup(stack);

			NCheckBox enableWorkCalendarCheckBox = new NCheckBox("Enable Work Calendar");
			enableWorkCalendarCheckBox.CheckedChanged += new Function<NValueChangeEventArgs>(OnEnableWorkCalendarCheckBoxCheckedChanged);
			enableWorkCalendarCheckBox.Checked = true;
			stack.Add(enableWorkCalendarCheckBox);

			return boxGroup;
		}
		protected override string GetExampleDescription()
		{
			return @"<p>This example demonstrates how to use the daily schedule of the work calendar in order to skip hourly ranges for which there is no data.</p>";
		}

		#endregion

		#region Event Handlers

		void OnEnableWorkCalendarCheckBoxCheckedChanged(NValueChangeEventArgs arg)
		{
			((NRangeTimelineScale)m_Chart.Axes[ENCartesianAxis.PrimaryX].Scale).EnableCalendar = (arg.TargetNode as NCheckBox).Checked;
		}

		#endregion

		#region Implementation

		private void ConfigureInteractivity(NChart chart)
		{
			NInteractor interactor = new NInteractor();

			NRectangleZoomTool rectangleZoomTool = new NRectangleZoomTool();
			rectangleZoomTool.Enabled = true;
			rectangleZoomTool.VerticalValueSnapper = new NAxisRulerMinMaxSnapper();
			interactor.Add(rectangleZoomTool);

			NDataPanTool dataPanTool = new NDataPanTool();
			dataPanTool.StartMouseButtonEvent = ENMouseButtonEvent.RightButtonDown;
			dataPanTool.EndMouseButtonEvent = ENMouseButtonEvent.RightButtonUp;
			dataPanTool.Enabled = true;
			interactor.Add(dataPanTool);

			chart.Interactor = interactor;
		}

		#endregion

		#region Fields

		NCartesianChart m_Chart;

		#endregion

		#region Static

		public static readonly NSchema NDailyScheduleWorkCalendarExampleSchema;

		#endregion
	}
}
