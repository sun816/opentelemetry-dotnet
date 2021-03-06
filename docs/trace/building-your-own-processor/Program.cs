﻿// <copyright file="Program.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

public class Program
{
    private static readonly ActivitySource MyActivitySource = new ActivitySource(
        "MyCompany.MyProduct.MyLibrary");

    public static void Main()
    {
        using var otel = Sdk.CreateTracerProvider(b => b
            .AddActivitySource("MyCompany.MyProduct.MyLibrary")

            // TODO: seems buggy as ShutdownAsync is called 6 times
            // TODO: need to discuss the expectation, currently FlushAsync is not called by default
            // TODO: should the dispose order be C, B, A or A, B C?
            .AddProcessorPipeline(p => p.AddProcessor(current => new MyActivityProcessor("A")))
            .AddProcessorPipeline(p => p.AddProcessor(current => new MyActivityProcessor("B")))
            .AddProcessorPipeline(p => p.AddProcessor(current => new MyActivityProcessor("C"))));

        using (var activity = MyActivitySource.StartActivity("SayHello"))
        {
            activity?.SetTag("foo", 1);
            activity?.SetTag("bar", "Hello, World!");
        }
    }
}
