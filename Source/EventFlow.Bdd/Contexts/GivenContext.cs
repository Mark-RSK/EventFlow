﻿// The MIT License (MIT)
//
// Copyright (c) 2015 Rasmus Mikkelsen
// https://github.com/rasmus/EventFlow
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Bdd.Steps.GivenSteps;
using EventFlow.Configuration;
using EventFlow.Core;

namespace EventFlow.Bdd.Contexts
{
    public class GivenContext : IGivenContext
    {
        private readonly IResolver _resolver;
        private IScenarioRunnerContext _scenarioRunnerContext;

        public GivenContext(
            IResolver resolver)
        {
            _resolver = resolver;
        }

        public IGiven Event<TAggregate, TIdentity, TAggregateEvent>(
            TIdentity identity,
            TAggregateEvent aggregateEvent)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            _scenarioRunnerContext.AddGiven(new EventScenarioStep<TAggregate, TIdentity, TAggregateEvent>(_resolver, identity, aggregateEvent));
            return this;
        }

        public IGiven That(string name, Action<IResolver> action)
        {
            return That(
                name,
                (r, c) =>
                {
                    action(r);
                    return Task.FromResult(0);
                });
        }

        public IGiven That(string name, Func<IResolver, CancellationToken, Task> action)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            _scenarioRunnerContext.AddGiven(new ThatScenarioStep(name, _resolver, action));
            return this;
        }

        public void Setup(IScenarioRunnerContext scenarioRunnerContext)
        {
            _scenarioRunnerContext = scenarioRunnerContext;
        }

        public void Dispose()
        {
        }
    }
}