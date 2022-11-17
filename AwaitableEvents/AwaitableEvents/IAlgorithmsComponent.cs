﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AwaitableEvents;

public interface IAlgorithmsComponent
{
    void RunAsync(InputData input);

    Guid LatestRequestId { get; }

    event EventHandler<AlgorithmFinishedEventArgs> AlgorithmFinished;
}

public static class AlgorithmsComponentExtensions
{
    public static TaskAwaiter<AlgorithmResult> GetAwaiter(this IAlgorithmsComponent self)
    {
        var requestId = self.LatestRequestId;

        var tcs = new TaskCompletionSource<AlgorithmResult>();

        void OnAlgorithmFinished(object _, AlgorithmFinishedEventArgs e)
        {
            if (requestId != e.RequestId)
            {
                return;
            }

            self.AlgorithmFinished -= OnAlgorithmFinished;
            tcs.SetResult(e.Result);
        }

        self.AlgorithmFinished += OnAlgorithmFinished;

        return tcs.Task.GetAwaiter();
    }
}

