using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using TimeSeriesAnalyzer.Model;

namespace TimeSeriesAnalyzer.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IDataService, RandomDataService>();
            SimpleIoc.Default.Register<ITimeSeriesComparatorService, TimeSeriesComparatorService>();
            SimpleIoc.Default.Register<MainViewModel>();
            
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static void Cleanup()
        {
        }
    }
}