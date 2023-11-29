namespace ExampleTest_1
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

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
			new TestInfo("DataMiner Perf Validation", "Phoenix", projectIds, "This test will validate the perf of DataMiner."),
			new TestSystemInfo("Unknown")); // This will be updated by the Skyline QAPortal element to your machine name

			// Adding test cases
			testReport.PerformanceTestCases.Add(GetElementsTest());
			testReport.PerformanceTestCases.Add(GetServicesTest());
			testReport.TryAddTestCase(GetRootViewTest());

			// Setting the result for the Skyline QAPortal element
			engine.AddScriptOutput("report", testReport.ToJson());
		}

		private PerformanceTestCaseReport GetElementsTest()
		{
			return RunPerformanceTest("Retrieving elements", () =>
			{
				var elements = thisDms.GetElements();
			});
		}

		private PerformanceTestCaseReport GetServicesTest()
		{
			return RunPerformanceTest("Retrieving services", () =>
			{
				var services = thisDms.GetServices();
			});
		}

		private TestCaseReport GetRootViewTest()
		{
			return RunTest("Get Root View", () =>
			{
				var rootView = thisDms.GetView(-1);
				if (rootView == null)
				{
					throw new Exception("Root View is null");
				}
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
				return new PerformanceTestCaseReport(testName, Result.Failure, $"Exception '{ex.Message}' caught, see information events", ResultUnit.Millisecond, sw.Elapsed.TotalMilliseconds);
			}

			sw.Stop();
			if (sw.Elapsed.TotalMilliseconds > 2000d)
			{
				return new PerformanceTestCaseReport(testName, Result.Failure, "Test took longer than 2s", ResultUnit.Millisecond, sw.Elapsed.TotalMilliseconds);
			}

			return new PerformanceTestCaseReport(testName, Result.Success, string.Empty, ResultUnit.Millisecond, sw.Elapsed.TotalMilliseconds);
		}

		private TestCaseReport RunTest(string testName, Action testAction)
		{
			try
			{
				testAction();
			}
			catch (Exception ex)
			{
				engine.GenerateInformation($"{testName} Exception: {ex}");
				return new TestCaseReport(testName, Result.Failure, $"Exception '{ex.Message}' caught, see information events");
			}

			return new TestCaseReport(testName, Result.Success, string.Empty);
		}
	}
}