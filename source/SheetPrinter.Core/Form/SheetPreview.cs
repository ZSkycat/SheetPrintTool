﻿using SheetPrinter.Core.Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using static SheetPrinter.Core.UnitConvert;

namespace SheetPrinter.Core.Form
{
    /// <summary>
    /// 具备延迟功能的预览控件
    /// </summary>
    public partial class SheetPreview : UserControl
    {
        private float dpiX;
        private float dpiY;

        private TemplateModel data;
        private Image background = null;
        private Panel content;

        // 延迟功能
        private DateTime drawTime = new DateTime(0);
        private Timer timer;

        public SheetPreview()
        {
            InitializeComponent();

            Graphics g = CreateGraphics();
            dpiX = g.DpiX;
            dpiY = g.DpiY;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data">模板数据</param>
        public void Initialize(TemplateModel data)
        {
            this.data = data;

            if (data.BackgroundFileName != "")
            {
                try
                {
                    background = Image.FromFile($@"{Program.TemplatePath}\{data.BackgroundFileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"背景图加载失败\r\n{ex}");
                }
            }

            content = new Panel()
            {
                Width = CmToPx_int(data.Width, dpiX),
                Height = CmToPx_int(data.Height, dpiY),
            };
            content.Paint += (sender, e) => { RefreshPreview(e.Graphics); };
            Controls.Add(content);

            timer = new Timer();
            timer.Interval = 500;
            timer.Tick += (sender, e) => { RefreshPreview(); };

            Disposed += (sender, e) => { timer.Stop(); };
        }

        /// <summary>
        /// 刷新预览图
        /// </summary>
        public void RefreshPreview()
        {
            if ((DateTime.Now - drawTime) > new TimeSpan(5000000))
            {
                RefreshPreview(null);
                drawTime = DateTime.Now;
                timer.Stop();
            }
            else if (!timer.Enabled)
            {
                timer.Start();
            }
        }

        /// <summary>
        /// 刷新预览图
        /// </summary>
        private void RefreshPreview(Graphics g)
        {
            DrawPreview(g ?? content.CreateGraphics());
        }

        /// <summary>
        /// 绘制预览图
        /// </summary>
        private void DrawPreview(Graphics g)
        {
            if (background != null)
            {
                g.DrawImage(background, new RectangleF(0f, 0f, CmToPx(data.Width, dpiX), CmToPx(data.Height, dpiY)), new RectangleF(0, 0, background.Width, background.Height), GraphicsUnit.Pixel);
            }
            foreach (var i in data.ElementList)
            {
                var font = PrintController.CalcFont(Program.Config.Font, i.FontSize);
                g.DrawString(i.Value, font, Brushes.Black, new RectangleF(CmToPx(i.X, dpiX), CmToPx(i.Y, dpiY), CmToPx(i.Width, dpiX), CmToPx(i.Height, dpiY)));
            }
        }
    }
}
