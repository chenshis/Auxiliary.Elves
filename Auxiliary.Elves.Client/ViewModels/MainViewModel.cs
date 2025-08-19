using Prism.Mvvm;
using Prism.Regions;

namespace Auxiliary.Elves.Client.ViewModels
{
    /// <summary>
    /// 主体视图模型
    /// </summary>
    public class MainViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public MainViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        /// <summary>
        /// 初始化区域管理
        /// </summary>
        public void LoadRegionManager()
        {
        }
    }
}
