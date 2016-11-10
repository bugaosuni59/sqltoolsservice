﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.SqlTools.ServiceLayer.TestDriver.Utility
{
    /// <summary>
    /// Timer to calculate the test run time
    /// </summary>
    public class TestTimer
    {
        private static string ResultFolder = Environment.GetEnvironmentVariable("ResultFolder");

        public TestTimer()
        {
            Start();
        }

        public void Start()
        {
            StartDateTime = DateTime.UtcNow;
        }

        public void End()
        {
            EndDateTime = DateTime.UtcNow;
        }

        public void EndAndPrint(string testName)
        {
            End();
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Test Name: {0} Run time in milliSeconds: {1}", testName, TotalMilliSeconds));
            Console.ForegroundColor = currentColor;
            string resultContent = Newtonsoft.Json.JsonConvert.SerializeObject(new TestResult { ElapsedTime = TotalMilliSeconds.ToString() });
            string fileName = testName + ".json";
            string resultFilePath = string.IsNullOrEmpty(ResultFolder) ? fileName : Path.Combine(ResultFolder, fileName);
            File.WriteAllText(resultFilePath, resultContent);
        }

        public double TotalMilliSeconds
        {
            get
            {
                return (EndDateTime - StartDateTime).TotalMilliseconds;
            }
        }

        public double TotalMilliSecondsUntilNow
        {
            get
            {
                return (DateTime.UtcNow - StartDateTime).TotalMilliseconds;
            }
        }

        public DateTime StartDateTime { get; private set; }
        public DateTime EndDateTime { get; private set; }
    }
}
