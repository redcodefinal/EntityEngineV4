﻿using System;
using System.Linq;
using System.Xml.Linq;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Rendering
{
    public class SourceAnimation : ImageRender
    {
        //TODO: Write frame class, to use with offset/rotation origin for each frame. VERY IMPORTANT!
        private Rectangle[] _sourcerectangles;

        public Rectangle[] SourceRectangles
        {
            get { return _sourcerectangles; }
        }

        public int FramesPerSecond;

        public int CurrentFrame { get; set; }

        public event Timer.TimerEvent LastFrameEvent;

        public Timer FrameTimer;

        public int TimelineSize
        {
            get { return _sourcerectangles.GetUpperBound(0); }
        }

        public bool HitLastFrame
        {
            get { return (CurrentFrame >= _sourcerectangles.GetUpperBound(0)); }
        }

        public int MillisecondsPerFrame
        {
            get { return 1000 / FramesPerSecond; }
        }

        public override Rectangle DrawRect
        {
            get
            {
                Vector2 position = GetDependency<Body>(DEPENDENCY_BODY).Position;
                return new Rectangle(
                    (int)(position.X + GetDependency<Body>(DEPENDENCY_BODY).Origin.X),
                    (int)(position.Y + GetDependency<Body>(DEPENDENCY_BODY).Origin.Y),
                    (int)(SourceRect.Width * Scale.X),
                    (int)(SourceRect.Height * Scale.Y));
            }
        }

        public SourceAnimation(Node e, string name)
            : base(e, name)
        {
            FrameTimer = new Timer(e, Name + ".FrameTimer");
            FrameTimer.LastEvent += AdvanceNextFrame;

            _sourcerectangles = new Rectangle[1];
        }

        public SourceAnimation(Node e, string name, Texture2D texture, Vector2 tileSize, int framesPerSecond)
            : base(e, name, texture)
        {
            FramesPerSecond = framesPerSecond;
            FramesPerSecond = framesPerSecond;

            FrameTimer = new Timer(e, Name + ".FrameTimer") { Milliseconds = MillisecondsPerFrame };
            FrameTimer.LastEvent += AdvanceNextFrame;

            _sourcerectangles = new Rectangle[1];
        }

        public override void Update(GameTime gt)
        {
            FrameTimer.Update(gt);
            if (HitLastFrame)
            {
                if (LastFrameEvent != null)
                    LastFrameEvent();
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            Rectangle sourcerect = _sourcerectangles[CurrentFrame];
            sb.Draw(Texture, DrawRect, sourcerect, Color * Alpha, GetDependency<Body>(DEPENDENCY_BODY).Angle,
                    GetDependency<Body>(DEPENDENCY_BODY).Origin, Flip, Layer);
        }

        public void AdvanceNextFrame()
        {
            CurrentFrame++;
            if (CurrentFrame >= TimelineSize)
                CurrentFrame = 0;
            SourceRect = _sourcerectangles[CurrentFrame];
        }

        public void AdvanceLastFrame()
        {
            CurrentFrame--;
            if (CurrentFrame < 0)
                CurrentFrame = TimelineSize;
            SourceRect = _sourcerectangles[CurrentFrame];
        }

        public void Start()
        {
            FrameTimer.Start();
        }

        public void Stop()
        {
            FrameTimer.Stop();
        }

        /// <summary>
        /// Adds a new source rectagle to the back of the timeline
        /// </summary>
        /// <param name="r"></param>
        public void AddSourceRectangle(Rectangle r)
        {
            //Check if out array has yet to be filled
            if (_sourcerectangles[0] == new Rectangle(0, 0, 0, 0) && _sourcerectangles.Count() == 1)
            {
                _sourcerectangles[0] = r;
                return;
            }

            int upper = _sourcerectangles.GetUpperBound(0);
            var copy = new Rectangle[upper + 2];

            for (int i = 0; i <= upper; i++)
            {
                copy[i] = _sourcerectangles[i];
            }
            copy[copy.GetUpperBound(0)] = r;

            //Copy over the new array
            _sourcerectangles = (Rectangle[])copy.Clone();
        }

        /// <summary>
        /// Add's a source rectangle into the timeline at a specified position.
        /// </summary>
        /// <param name="r">The source rectangle</param>
        /// <param name="position"> the postion</param>
        /// <param name="insert">Whether or not to insert or replace.</param>
        public void AddSourceRectangle(Rectangle r, int position, bool insert)
        {
            if (insert)
            {
                //resize it
                var copy = new Rectangle[_sourcerectangles.GetUpperBound(0) + 1];
                for (int i = 0; i < _sourcerectangles.GetUpperBound(0); i++)
                {
                    if (i >= position) i++;
                    copy[i] = _sourcerectangles[i];
                }

                //Copy over the new array
                _sourcerectangles = (Rectangle[])copy.Clone();

                //copy the rectangle
                _sourcerectangles[position] = r;
            }
            else
            {
                //if the position is greater than the array size
                if (position > _sourcerectangles.GetUpperBound(0))
                {
                    //resize it
                    var copy = new Rectangle[position];
                    for (int i = 0; i < _sourcerectangles.GetUpperBound(0); i++)
                    {
                        copy[i] = _sourcerectangles[i];
                    }

                    //Copy over the new array
                    _sourcerectangles = (Rectangle[])copy.Clone();
                }

                //copy the rectangle
                _sourcerectangles[position] = r;
            }
        }

        public void ReadXml(string path)
        {
            //Load our XML
            var xml = XDocument.Load(path);

            //Get the root element
            var rootelement = xml.Root;
            if (rootelement == null)
            {
                EntityGame.Log.Write("XML Path was null!", this, Alert.Error);
                throw new Exception("XML Path was null!");
            }

            var rectangles = rootelement.Elements();
            foreach (var rectangle in rectangles)
            {
                Rectangle r = new Rectangle();
                var values = rectangle.Elements();
                foreach (var value in values)
                {
                    if (value.Name == "X")
                    {
                        r.X = (int)value;
                    }
                    else if (value.Name == "Y")
                    {
                        r.Y = (int)value;
                    }
                    else if (value.Name == "Width")
                    {
                        r.Width = (int)value;
                    }
                    else if (value.Name == "Height")
                    {
                        r.Height = (int)value;
                    }
                }

                AddSourceRectangle(r);
            }
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
    }
}