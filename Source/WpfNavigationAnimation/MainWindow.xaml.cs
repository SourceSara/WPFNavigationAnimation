using System.Windows;
using WpfNavigationAnimation.Controls;

namespace WpfNavigationAnimation
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnNoneButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new NonePage(),
                OldPageAnimation.None, NewPageAnimation.None);
        }

        private void OnFadeButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new FadeOutFadeInPage(),
                OldPageAnimation.FadeOut, NewPageAnimation.FadeIn);
        }

        private void OnScaleToOutScaleFromInButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new ScaleToOutScaleFromInPage(), 
                OldPageAnimation.ScaleToOut, NewPageAnimation.ScaleFromIn);
        }

        private void OnScaleToInScaleFromInButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new ScaleToInScaleFromInPage(),
                OldPageAnimation.ScaleToIn, NewPageAnimation.ScaleFromIn);
        }

        private void OnScaleToInScaleFromOutButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new ScaleToInScaleFromOutPage(), 
                OldPageAnimation.ScaleToIn, NewPageAnimation.ScaleFromOut);
        }

        private void OnScaleToOutScaleFromOutButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new ScaleToOutScaleFromOutPage(),
                OldPageAnimation.ScaleToOut, NewPageAnimation.ScaleFromOut);
        }

        private void OnSlideLeftRightButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new SlideLeftRightPage(), 
                OldPageAnimation.SlideOutToLeft, NewPageAnimation.SlideInFromRight);
        }

        private void OnSlideRightLeftButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new SlideRightLeftPage(), 
                OldPageAnimation.SlideOutToRight, NewPageAnimation.SlideInFromLeft);
        }
    }
}
