#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuanLySan.ViewModels.Base
{
    // Lớp cơ sở hỗ trợ cập nhật giao diện tự động
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Thông báo cho giao diện biết thuộc tính đã thay đổi
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}