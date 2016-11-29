﻿using SheetPrintTool.DataModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static SheetPrintTool.Core.UnitlHelper;

namespace SheetPrintTool.Core
{
    public class PrintControl
    {
        List<TemplateData> dataList;
        TemplateData data;
        Image image;

        Panel previewContainer;
        Panel preview;

        /// <summary>
        /// 实例化打印控制器
        /// </summary>
        /// <param name="data">模版数据</param>
        public PrintControl(TemplateData data)
        {
            this.data = data;
            //!!!
            image = Image.FromFile($@"{Global.TemplatePath}\{data.BackgroundFileName}");
        }

        public PrintControl(List<TemplateData> datalist)
        {

        }

        #region 预览功能
        /// <summary>
        /// 初始化预览面板
        /// </summary>
        /// <param name="container">Panel 容器</param>
        public void InitPreview(Panel container)
        {
            container.AutoScroll = true;
            previewContainer = container;
            preview = new Panel()
            {
                Width = Convert.ToInt32(MmToPxAtDpi(data.Width, Global.DpiX)),
                Height = Convert.ToInt32(MmToPxAtDpi(data.Height, Global.DpiY))
            };
            previewContainer.Controls.Add(preview);
            preview.Paint += (sender, e) => { Preview(e.Graphics); };
        }

        /// <summary>
        /// 刷新预览图
        /// </summary>
        public void RefreshPreview()
        {
            Preview(preview.CreateGraphics());
        }

        /// <summary>
        /// 生成预览图
        /// </summary>
        /// <param name="g">要绘制的 Graphics 对象</param>
        private void Preview(Graphics g)
        {
            g.DrawImage(image, new RectangleF(0f, 0f, MmToPxAtDpi(data.Width, 96), MmToPxAtDpi(data.Height, 96)), new RectangleF(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            foreach (var i in data.ElementList)
            {
                g.DrawString(i.Value, Global.Config.Font, Brushes.Black, new RectangleF(MmToPxAtDpi(i.X, 96), MmToPxAtDpi(i.Y, 96), MmToPxAtDpi(i.Width, 96), MmToPxAtDpi(i.Height, 96)));
            }
        }
        #endregion

        #region 打印功能
        public void Print()
        {
            var document = new PrintDocument();
            document.PrintPage += Document_PrintPage;
            var dialog = new PrintDialog()
            {
                Document = document
            };

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                document.Print();
            }
        }

        private void Document_PrintPage(object sender, PrintPageEventArgs e)
        {
            // e.HasMorePages
            throw new NotImplementedException();
        }
        #endregion
    }
}