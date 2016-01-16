namespace LOIC
{
  internal interface IFlooder
  {
    int Delay { get; set; }

    bool IsFlooding { get; set; }

    void Start();

    void Stop();
  }
}
