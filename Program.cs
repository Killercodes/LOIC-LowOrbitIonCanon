
using System;
using System.Windows.Forms;

namespace LOIC
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new frmMain());
    }
  }
}
