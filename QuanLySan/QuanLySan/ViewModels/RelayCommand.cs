#nullable enable
using System;
using System.Windows.Input;

namespace QuanLySan.ViewModels.Base
{
    // Lớp hỗ trợ binding sự kiện từ UI vào ViewModel
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Kiểm tra điều kiện để kích hoạt lệnh
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        // Thực hiện hành động của lệnh
        public void Execute(object? parameter) => _execute(parameter);

        // Tự động cập nhật trạng thái lệnh khi UI thay đổi
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}