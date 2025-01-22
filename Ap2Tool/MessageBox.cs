using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Ap2Tool
{

    internal static class MessageBox
    {

        // 메시지박스를 표시한다.
        // 주의) Avalonia Framework가 초기화되기 전에 호출하면 에러가 발생한다.

        public static async Task<DialogResult> ShowAsync(string message, 
            string title = null, 
            MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            ButtonEnum buttonEnum = ToButtonEnum(buttons);
            var box = MessageBoxManager.GetMessageBoxStandard(title, message, buttonEnum);
            ButtonResult res = await box.ShowAsync();
            return ToDialogResult(res);
        }


        static ButtonEnum ToButtonEnum(MessageBoxButtons buttons)
        {
            ButtonEnum buttonEnum;

            if (buttons == MessageBoxButtons.YesNo)
                buttonEnum = ButtonEnum.YesNo;
            else if (buttons == MessageBoxButtons.OKCancel)
                buttonEnum = ButtonEnum.OkCancel;
            else if (buttons == MessageBoxButtons.YesNoCancel)
                buttonEnum = ButtonEnum.YesNoCancel;
            else
                buttonEnum = ButtonEnum.Ok;

            return buttonEnum;
        }


        static DialogResult ToDialogResult(ButtonResult result)
        {
            DialogResult dr;

            if (result == ButtonResult.Ok)
                dr = DialogResult.OK;
            else if (result == ButtonResult.Yes)
                dr = DialogResult.Yes;
            else if (result == ButtonResult.No)
                dr = DialogResult.No;
            else if (result == ButtonResult.Abort)
                dr = DialogResult.Abort;
            else if (result == ButtonResult.Cancel)
                dr = DialogResult.Cancel;
            else
                dr = DialogResult.None;

            return dr;
        }

    }

    public enum DialogResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Abort = 3,
        Retry = 4,
        Ignore = 5,
        Yes = 6,
        No = 7,
        TryAgain = 10,
        Continue = 11
    };


    public enum MessageBoxButtons
    {
        OK = 0,
        OKCancel = 1,
        //AbortRetryIgnore = 2,
        YesNoCancel = 3,
        YesNo = 4,
        //RetryCancel = 5,
        //CancelTryContinue = 6,
    }



}
