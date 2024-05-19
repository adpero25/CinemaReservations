﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;

public class AsyncRelayCommand : ICommand
{
	private readonly Func<object, Task> _execute;
	private readonly Func<object, bool> _canExecute;

	public AsyncRelayCommand(Func<object, Task> execute, Func<object, bool>? canExecute = null)
	{
		_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		if (canExecute != null)
		{
			_canExecute = canExecute;
		}
		else
		{
			_canExecute = x => { return true; };
		}
	}

	public event EventHandler CanExecuteChanged
	{
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}

	public bool CanExecute(object parameter)
	{
		return _canExecute(parameter);
	}

	public async void Execute(object parameter)
	{
		await _execute(parameter);
	}
}
