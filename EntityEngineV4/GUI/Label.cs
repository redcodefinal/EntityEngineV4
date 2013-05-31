﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    public class Label : Control
    {
        public Color Color = Color.Black;

        public string Text
        {
            get { return TextRender.Text; }
            set { TextRender.Text = value; }
        }

        //Components
        protected TextRender TextRender;

        public Label(EntityState stateref, string name) : base(stateref, name)
        {
            TextRender = new TextRender(this, "TextRender", Assets.Font, name, Body);
            TextRender.Color = Color.Black;
            Selectable = false;
        }
    }
}
