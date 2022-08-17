using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cynteract.CGlove
{

    [Serializable]
    public class Scan
    {
        public Task<ScanResult> ScanDevices()
        {
            var taskCompletionSource = new TaskCompletionSource<ScanResult>();
            Task.Factory.StartNew(() =>
            {
                var ports = USB.DetectComPorts();
                taskCompletionSource.SetResult(new ScanResult() { ids = ports });
            });
            return taskCompletionSource.Task;
        }
    }
    public class ScanResult
    {
        public string[] ids;
    }

}
