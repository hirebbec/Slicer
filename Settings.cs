using slicer.stl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slicer
{
    public class Settings
    {
        private double _overlap;
        private double _heightStep;
        private double _delay;
        private double _feedSpeed;

        public Settings(double overlap, double heightStep, double delay, double feedSpeed)
        {
            this._overlap = overlap;
            this._heightStep = heightStep;
            this._delay = delay;
            this._feedSpeed = feedSpeed;
        }
        public double Overlap
        {
            get { return _overlap; }
            set { _overlap = value; }
        }
        public double HeightStep
        {
            get { return _heightStep; }
            set { _heightStep = value; }
        }
        public double Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }
        public double FeedSpeed
        {
            get { return _feedSpeed; }
            set { _feedSpeed = value; }
        }
    }
}
