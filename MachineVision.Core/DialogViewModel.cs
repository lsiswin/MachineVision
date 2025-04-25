using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;
using System;

namespace MachineVision.Core
{
    /// <summary>
    /// 弹窗基类
    /// </summary>
    public class DialogViewModel : BindableBase, IDialogAware
    {
        public DialogViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        public DelegateCommand SaveCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public string Title { get; set; }

        public DialogCloseListener RequestClose { get; private set; }


        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        { }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        { }


        private void Cancel()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public virtual void Save()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }
    }
}
