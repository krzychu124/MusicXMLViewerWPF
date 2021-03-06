﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.ScoreLayout.PageLayouts.PageElements
{
    class HeaderContainer : AbstractPageElement
    {
        public HeaderContainer(double width, double height) : base(width, height)
        {
        }

        public void AddTitle(string text)
        {
            var simpleTextBlock = new SimpleTextBox(text)
            {
                FontWeight = FontWeights.Bold,
                FontSize = 40,
                TextAlignment = System.Windows.TextAlignment.Center,
                Width = base.Width
            };
            simpleTextBlock.SetWidth(Width);
            if (Height < simpleTextBlock.Height)
            {
                Height = simpleTextBlock.Height;
            }
            items.Add(simpleTextBlock);
            AddVisual(simpleTextBlock);
        }

        public override void Update()
        {
            Console.WriteLine("Header Updated");
        }

        public override void UpdateDimensions(double width, double height)
        {
            foreach (var item in items)
            {
                item.SetWidth(width);
            }
            Width = width;
        }
    }
}
