using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slicer
{
    internal class Robot
    {
        private double _overlap;
        private double _heightStep;

        /// <summary>
        /// Инициализация робота для работы с STL деталью
        /// </summary>
        /// <param name="overlap"> Перекрытие для горизонтального хода робота [метр]</param>
        /// <param name="heightStep"> Шаг вертикального хода [метр]</param> asd
        public Robot(double overlap, double heightStep)
        {
            this._overlap = overlap;
            this._heightStep = heightStep;
        }

        /// <summary>
        /// Перекрытие горизонтального хода [метр]
        /// </summary>
        public double Overlap
        {
            get { return _overlap; }
            set { _overlap = value; }
        }
        /// <summary>
        /// Шаг вертикального хода [метр]
        /// </summary>
        public double HeightStep
        {
            get { return _heightStep; }
            set { _heightStep = value; }
        }
    }
}
