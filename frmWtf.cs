using LOIC.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LOIC
{
  public class frmWtf : Form
  {
    private IContainer components;

    public frmWtf()
    {
      this.InitializeComponent();
    }

    private void frmWtf_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void frmWtf_KeyDown(object sender, KeyEventArgs e)
    {
      this.Close();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackgroundImage = (Image) Resources.WTF;
      this.ClientSize = new Size(416, 300);
      this.ControlBox = false;
      this.Name = "frmWtf";
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Click += new EventHandler(this.frmWtf_Click);
      this.KeyDown += new KeyEventHandler(this.frmWtf_KeyDown);
      this.ResumeLayout(false);
    }
  }
}
