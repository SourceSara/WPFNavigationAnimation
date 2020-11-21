using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfNavigationAnimation.Controls
{
    [TemplatePart(Name = ContainerPanelName, Type = typeof(StackPanel))]
    [TemplatePart(Name = AmRoLogoCanvasName, Type = typeof(Canvas))]
    public class MadeByAmRo : Control
    {
        #region Fields

        private const string ContainerPanelName = "PART_ContainerPanel";
        private const string AmRoLogoCanvasName = "PART_AmRoLogoCanvas";

        private Canvas _amRoLogoCanvas;

        #endregion

        #region Constructors

        static MadeByAmRo()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MadeByAmRo),
                new FrameworkPropertyMetadata(typeof(MadeByAmRo)));
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            _amRoLogoCanvas = GetTemplateChild(AmRoLogoCanvasName) as Canvas;
            _amRoLogoCanvas.PreviewMouseLeftButtonDown += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(AmRoUrl))
                    return;

                Process.Start(new ProcessStartInfo(AmRoUrl) { UseShellExecute = true });
            };
        }

        #endregion

        #region DependencyProperty : AmRoUrl

        public static readonly DependencyProperty AmRoUrlProperty =
            DependencyProperty.Register(nameof(AmRoUrl),
                typeof(string), typeof(MadeByAmRo),
                new PropertyMetadata(default));

        public string AmRoUrl
        {
            get => (string)GetValue(AmRoUrlProperty);
            set => SetValue(AmRoUrlProperty, value);
        }

        #endregion

        #region DependencyProperty : AmRoLogoBrush

        public static readonly DependencyProperty AmRoLogoBrushProperty =
            DependencyProperty.Register(nameof(AmRoLogoBrush),
                typeof(SolidColorBrush), typeof(MadeByAmRo),
                new PropertyMetadata(default));

        public SolidColorBrush AmRoLogoBrush
        {
            get => (SolidColorBrush)GetValue(AmRoLogoBrushProperty);
            set => SetValue(AmRoLogoBrushProperty, value);
        }

        #endregion

        #region DependencyProperty : HeartBrush

        public static readonly DependencyProperty HeartBrushProperty =
            DependencyProperty.Register(nameof(HeartBrush),
                typeof(SolidColorBrush), typeof(MadeByAmRo),
                new PropertyMetadata(default));

        public SolidColorBrush HeartBrush
        {
            get => (SolidColorBrush)GetValue(HeartBrushProperty);
            set => SetValue(HeartBrushProperty, value);
        }

        #endregion
    }
}
