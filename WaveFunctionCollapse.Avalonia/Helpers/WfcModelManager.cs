using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using WaveFunctionCollapse.Interfaces;

namespace WaveFunctionCollapse.Avalonia.Helpers;

public class WfcModelManager
{
    public int Speed { get; set; }

    private readonly IWaveFunctionCollapseModel _model;
    private readonly Action<Bitmap> updateImageCallback;
    private readonly Action finishedCallback;

    private Task? runningTask;
    private CancellationTokenSource? cts;

    public WfcModelManager(IWaveFunctionCollapseModel model, Action<Bitmap> updateImageCallback, Action finishedFunc)
    {
        _model = model;
        this.updateImageCallback = updateImageCallback;
        finishedCallback = finishedFunc;
        updateImageCallback(_model.Image);
    }

    public void Start()
    {
        if (runningTask != null && runningTask.Status == TaskStatus.Running)
            return;
        cts = new CancellationTokenSource();
        runningTask = Task.Run(() => Run(cts.Token));
    }

    public void Stop()
    {
        cts?.Cancel();
        runningTask?.Wait();
    }

    public void Reset()
    {
        cts?.Cancel();
        runningTask = null;
        _model.Reset();
        updateImageCallback(_model.Image);
    }

    private void Run(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _model.CollapsesLeft > 0)
        {
            var stopwatch = Stopwatch.StartNew();
            int count = 0;
            while (!cancellationToken.IsCancellationRequested && _model.CollapsesLeft > 0 && count < Speed)
            {
                count++;
                _model.Collapse();
            }
            updateImageCallback(_model.Image);
            stopwatch.Stop();
            var ellapsedMillis = (int)stopwatch.ElapsedMilliseconds;
            if (ellapsedMillis < 10)
                Thread.Sleep(10 - ellapsedMillis);
        }
        if (_model.CollapsesLeft == 0)
            finishedCallback();
    }
}
