using System;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ViewModelTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void BoundsTest()
        {
            var vm = new ViewModel.ModelDataCollectionVM(new ViewModel.ObservableModelData());
            Assert.AreEqual(vm.HasErrors, false);
            vm.X1 = -0.1;
            Assert.AreEqual(vm.HasErrors, true);
            vm.X1 = 0.6;
            vm.X2 = 0.4;
            Assert.AreEqual(vm.HasErrors, true);
        }

        [TestMethod]
        public void InputParamsTest()
        {
            var vm = new ViewModel.ModelDataInputVM();
            Assert.AreEqual(vm.HasErrors, false);
            vm.NodeCount = 2;
            Assert.AreNotEqual(vm["NodeCount"], string.Empty);
            vm.Parameter = Model.ModelData.PMin - 1;
            Assert.AreNotEqual(vm["Parameter"], string.Empty);
            vm.Parameter = Model.ModelData.PMax + 1;
            Assert.AreNotEqual(vm["Parameter"], string.Empty);
        }

        [TestMethod]
        public void NewCollectionTest()
        {
            var vm = new ViewModel.MainWindowVM(new UI());
            Assert.AreEqual(vm.DataView.ModelDatas.Count, 0);
            vm.AddDefaultsCommand.Execute(null);
            vm.NewCommand.Execute(null);
            Assert.AreEqual(vm.DataView.ModelDatas.Count, 0);
        }
    }
    public class UI : ViewModel.IUIService
    {
        public Chart UIChart { get; private set; } = null;

        public bool ConfirmAction(string text, string title)
        {
            return true;
        }

        public string OpenFileDGName()
        {
            return string.Empty;
        }

        public string SaveFileDGName()
        {
            return string.Empty;
        }

        public void ShowErrorMessage(string text)
        {        }
    }
}
