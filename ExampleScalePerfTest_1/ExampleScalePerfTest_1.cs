/*
****************************************************************************
*  Copyright (c) 2024,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

01/07/2024	1.0.0.1		MSA, Skyline	Initial version
****************************************************************************
*/

namespace ExampleScalePerfTest_1
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using QAPortalAPI.Enums;


	using QAPortalAPI.Models.ReportingModels;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private IEngine engine;
		private IDms thisDms;
		private IEnumerable<IDmsView> views;

		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			this.engine = engine;
			thisDms = engine.GetDms();
			List<int> projectIds = new List<int> { 1648 }; // Change this to your M&S support project ID to get the results in collaboration
			TestReport testReport = new TestReport(
			new TestInfo("Retrieve views", "Phoenix", projectIds, "This test will validate the perf of DataMiner."),
			new TestSystemInfo("Unknown")); // This will be updated by the Skyline QAPortal element to your machine name

			// Adding test cases
			testReport.PerformanceTestCases.Add(GetViewsTest());
			testReport.PerformanceTestCases.Add(RenameViewTest());

			// Setting the result for the Skyline QAPortal element
			engine.AddScriptOutput("report", testReport.ToJson());
		}

		private PerformanceTestCaseReport GetViewsTest()
		{
			return RunPerformanceTest("Retrieving views", () =>
			{
				views = thisDms.GetViews();
			});
		}

		private PerformanceTestCaseReport RenameViewTest()
		{
			return RunPerformanceTest("Rename view", () =>
			{
				var view = views.First(v => v.Name.StartsWith("Scalability Test View "));
				string newName = view.Name;
				int indexOfRename = newName.LastIndexOf("RENAME ");
				if (indexOfRename > 0)
				{
					int number = Convert.ToInt32(newName.Substring(indexOfRename + 7)) + 1;
					newName = newName.Substring(0, indexOfRename) + "RENAME " + number;
				}
				else
				{
					newName = newName + " RENAME 1";
				}

				engine.GenerateInformation($"Renaming {view.Name} to {newName}");
				view.Name = newName;
				view.Update();
			});
		}

		private PerformanceTestCaseReport RunPerformanceTest(string testName, Action testAction)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			try
			{
				testAction();
			}
			catch (Exception ex)
			{
				engine.GenerateInformation($"{testName} Exception: {ex}");
				return new PerformanceTestCaseReport(testName, Result.Failure, $"Exception '{ex.Message}' caught, see information events", ResultUnit.Millisecond, -1);
			}

			sw.Stop();

			return new PerformanceTestCaseReport(testName, Result.Success, string.Empty, ResultUnit.Millisecond, sw.Elapsed.TotalMilliseconds);
		}
	}
}