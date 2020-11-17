﻿// ***********************************************************************
// Copyright (c) 2015 NUnit Project
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.Runner.Extensions;
using Xamarin.Forms;

namespace NUnit.Runner.ViewModel
{
    //class TestViewModel : BaseViewModel
    //{
    //    public TestViewModel(ITestResult result)
    //    {
    //        TestResult = result;
    //        Message = StringOrNone(result.Message);
    //        Output = StringOrNone(result.Output);
    //        StackTrace = StringOrNone(result.StackTrace);

    //        var builder = new StringBuilder();
    //        IPropertyBag props = result.Test.Properties;
    //        foreach (string key in props.Keys)
    //        {
    //            foreach (var value in props[key])
    //            {
    //                builder.AppendFormat("{0} = {1}{2}", key, value, Environment.NewLine);
    //            }
    //        }
    //        Properties = StringOrNone(builder.ToString());
    //    }

    //    public ITestResult TestResult { get; private set; }
    //    public string Message { get; private set; }
    //    public string Output { get; private set; }
    //    public string StackTrace { get; private set; }
    //    public string Properties { get; private set; }

    //    /// <summary>
    //    /// Gets the color for this result.
    //    /// </summary>
    //    public Color Color
    //    {
    //        get { return TestResult.ResultState.Color(); }
    //    }

    //    private string StringOrNone(string str)
    //    {
    //        if (string.IsNullOrWhiteSpace(str))
    //            return "<none>";
    //        return str;
    //    }
    //}
    internal class TestViewModel : BaseViewModel
    {
        private ITestResult _result;

        public string Name { get; private set; }

        public ITest Test { get; }

        public ITestResult Result
        {
            get
            {
                return _result;
            }
            set
            {
                if (Set(ref _result, value))
                {
                    NotifyStatusUpdated();
                }
            }
        }

        public Color Color
        {
            get
            {
                if (Result == null)
                {
                    return Color.Transparent;
                }

                return Result.OverallResultState().Color();
            }
        }

        public string TestStatus
        {
            get
            {
                if (Result == null)
                {
                    if (Test.HasChildren)
                    {
                        return $"{Test.TestCaseCount} test case(s), {Test.RunState}";
                    }

                    return $"Not Executed, {Test.RunState}";
                }

                if (Result.OverallResultState().Status == Framework.Interfaces.TestStatus.Passed)
                {
                    if (Result.HasChildren)
                    {
                        return $"Success! {(int)(Result.Duration * 1000)} ms for {Result.PassCount} test(s); {Result.SkipCount} skipped";
                    }

                    return $"Success! {(int)(Result.Duration * 1000)} ms for {Result.AssertCount} assertion(s)";
                }

                if (Result.HasChildren)
                {
                    return $"Failure! {Result.Test.TestCaseCount} test(s) - {Result.FailCount} failed, {Result.PassCount} passed";
                }

                return $"Failure! {Result.Message}";
            }
        }

        public TestViewModel(ITest test)
        {
            Name = test.Name;
            Test = test;
        }

        public TestViewModel(ITestResult result) : this(result.Test)
        {
            Result = result;
        }

        protected void NotifyStatusUpdated()
        {
            OnPropertyChanged(nameof(Color));
            OnPropertyChanged(nameof(TestStatus));
        }
    }
}
