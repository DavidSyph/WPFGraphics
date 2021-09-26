using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFGraphics.Spinner
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class Spinner : UserControl
    {
        #region Properties
        #region CanvasSize
        /// <summary>
        /// Size of the canvas
        /// </summary>
        public double CanvasSize
        {
            get { return (double)GetValue(CanvasSizeProperty); }
            set { SetValue(CanvasSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanvasSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasSizeProperty =
            DependencyProperty.Register("CanvasSize", typeof(double), typeof(Spinner), new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender));


        #endregion

        #region PathData
        /// <summary>
        /// Path Data
        /// </summary>
        public string PathData
        {
            get { return (string)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathDataProperty =
            DependencyProperty.Register("PathData", typeof(string), typeof(Spinner), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(PathDataPropertyChanged)));

        protected static void PathDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner spinner = d as Spinner;
            spinner.Draw();
        }
        #endregion

        #region SliceCount

        /// <summary>
        /// Count of slices in the pie. An even number must be specified. An odd number will default to 36. A maximum value of 72.
        /// </summary>
        public int SliceCount
        {
            get { return (int)GetValue(SliceCountProperty); }
            set { SetValue(SliceCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SliceCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SliceCountProperty =
            DependencyProperty.Register("SliceCount", typeof(int), typeof(Spinner), new FrameworkPropertyMetadata(36, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(SliceCountPropertyChanged), new CoerceValueCallback(CoerceSliceCountProperty)));

        protected static void SliceCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner spinner = d as Spinner;
            spinner.Draw();
        }

        protected static object CoerceSliceCountProperty(DependencyObject d, object value)
        {
            int newValue = (int)value;
            //if value is invalid, just return default of 36
            if (newValue <= 0)
            {
                return 36;
            }
            //if value is odd, return default of 36
            if (newValue % 2 > 0)
            {
                return 36;
            }
            else
            {
                //if value is greater than maximu value of 72, return 72
                if (newValue > 72)
                {
                    return 72;
                }

                return value;
            }
        }
        #endregion

        #region LargeArcRadius
        /// <summary>
        /// Large arc radius
        /// </summary>
        public double LargeArcRadius
        {
            get { return (double)GetValue(LargeArcRadiusProperty); }
            set { SetValue(LargeArcRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LargeArcRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LargeArcRadiusProperty =
            DependencyProperty.Register("LargeArcRadius", typeof(double), typeof(Spinner), new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(LargeArcRadiusPropertyChanged)));

        protected static void LargeArcRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner spinner = d as Spinner;
            spinner.Draw();
        }
        #endregion

        #region SmallArcRadius

        /// <summary>
        /// Small arc radius
        /// </summary>
        public double SmallArcRadius
        {
            get { return (double)GetValue(SmallArcRadiusProperty); }
            set { SetValue(SmallArcRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallArcRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallArcRadiusProperty =
            DependencyProperty.Register("SmallArcRadius", typeof(double), typeof(Spinner), new FrameworkPropertyMetadata(30.0, FrameworkPropertyMetadataOptions.AffectsRender,new PropertyChangedCallback(SmallArcRadiusPropertyChanged)));

        protected static void SmallArcRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner spinner = d as Spinner;
            spinner.Draw();
        }
        #endregion

        #region PathFill
        /// <summary>
        /// Fill brush for the path
        /// </summary>
        public Brush PathFill
        {
            get { return (Brush)GetValue(PathFillProperty); }
            set { SetValue(PathFillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathFillBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathFillProperty =
            DependencyProperty.Register("PathFill", typeof(Brush), typeof(Spinner), new FrameworkPropertyMetadata(Brushes.CornflowerBlue, FrameworkPropertyMetadataOptions.AffectsRender));


        #endregion
        #endregion

        #region Constructor
        public Spinner()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Draw();
        }
        #endregion

        #region Methods

        protected void Draw()
        {
            StringBuilder data = new StringBuilder();

            //origin
            Point origin = new Point(LargeArcRadius, LargeArcRadius);

            //angle per slice in degrees
            double sliceAngle = 360.0 / SliceCount;

            //each slice starts with a large arc, a line that goes down, a small arc and is closed of by a line.
            //We will be using Path Markup Syntax
            //We draw slice from slice 1-2, 3-4 5-6 and so on

            //First point is 1. to get the angle we subtract 1 from the pointIndex and multiply by the sliceAngle
            for (int index = 1; index < SliceCount; index+=2)
            {
                //Angle in degrees for first point on large Arc
                double angle = (sliceAngle)*(index - 1);

                //Angle in radians for first point on large Arc
                double angleRadians = ((Math.PI / 180) * angle);

                // (Origin.X + Rcos(θ), Origin.Y – Rsin(θ)) 
                Point pointOneLargeArc = new Point(origin.X + (LargeArcRadius * Math.Cos(angleRadians)), origin.Y - (LargeArcRadius * Math.Sin(angleRadians)));

                // (Origin.X + rcos(θ), Origin.Y – rsin(θ)) 
                Point pointOneSmallArc = new Point(origin.X + (SmallArcRadius * Math.Cos(angleRadians)), origin.Y - (SmallArcRadius * Math.Sin(angleRadians)));


                //Angle in degrees for first point on large Arc. Notice 1 is added to index because we are on the second point on the arc
                angle = (sliceAngle) * ((index+1) - 1);

                //Angle in radians for first point on large Arc
                angleRadians = ((Math.PI / 180) * angle);

                // (Origin.X + Rcos(θ), Origin.Y – Rsin(θ)) 
                Point pointTwoLargeArc = new Point(origin.X + (LargeArcRadius * Math.Cos(angleRadians)), origin.Y - (LargeArcRadius * Math.Sin(angleRadians)));

                // (Origin.X + rcos(θ), Origin.Y – rsin(θ)) 
                Point pointTwoSmallArc = new Point(origin.X + (SmallArcRadius * Math.Cos(angleRadians)), origin.Y - (SmallArcRadius * Math.Sin(angleRadians)));

                //Start Path figure with pointOneLargeArc. M StartPoint
                data.Append("M ");
                data.Append(pointOneLargeArc.X);
                data.Append(",");
                data.Append(pointOneLargeArc.Y);
                data.Append(" ");

                //Draw large arc. A size rotationAngle isLargeArcFlag(0) sweepDirectionFlag(0 for counterclockwise) endPoint
                data.Append("A ");
                data.Append(this.LargeArcRadius);
                data.Append(",");
                data.Append(this.LargeArcRadius);
                data.Append(" ");
                data.Append(sliceAngle);
                data.Append(" ");
                data.Append("0");
                data.Append(" ");
                data.Append("0");
                data.Append(" ");
                data.Append(pointTwoLargeArc.X);
                data.Append(",");
                data.Append(pointTwoLargeArc.Y);
                data.Append(" ");

                //Draw line from S2 to s2. L endPoint
                data.Append("L ");
                data.Append(pointTwoSmallArc.X);
                data.Append(",");
                data.Append(pointTwoSmallArc.Y);
                data.Append(" ");

                //Draw small arc from s2 to s1. A size rotationAngle isLargeArcFlag(0) sweepDirectionFlag(1 for clockwise) endPoint
                data.Append("A ");
                data.Append(this.SmallArcRadius);
                data.Append(",");
                data.Append(this.SmallArcRadius);
                data.Append(" ");
                data.Append(sliceAngle);
                data.Append(" ");
                data.Append("0");
                data.Append(" ");
                data.Append("1");
                data.Append(" ");
                data.Append(pointOneSmallArc.X);
                data.Append(",");
                data.Append(pointOneSmallArc.Y);
                data.Append(" ");

                //close path figure. Draws a line from s1 to S1.  Z
                data.Append("Z ");
            }

            this.CanvasSize = LargeArcRadius * 2;
            this.PathData = data.ToString();
        }
        #endregion
    }
}
