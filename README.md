Bindery
=======
Bindery aims to support fluent MVVM binding definition for WinForms applications.

Projects
--------
* **Bindery:** 
  * Contains the static `Binder` factory class 
  * Dependent on the [System.Reactive](https://www.nuget.org/packages/System.Reactive/) package
  * Please note that it may be necessary to install the [System.Reactive.Compatibility](https://www.nuget.org/packages/System.Reactive.Compatibility/) package in the main project of 
    a consuming application in order to resolve conflicts between the [System.Reactive](https://www.nuget.org/packages/System.Reactive/) package 
    and older versions of `System.Reactive`-related libraries (such as `Sytem.Reactive.Linq`).

Assumptions
-----------
* A **View Model** is a binding source, an object of any type. Full binding functionality requires a View Model to properly implement
 [INotifyPropertyChanged](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged).
* A **Control** is a binding target, an object that implements [IBindableComponent](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.ibindablecomponent). 
A Control supports the full range of binding functionality.
* A **Target** is a binding target, an object of any type. A Target only supports a limited set of binding functionality.
-------------
# Code Examples
## Binding
##### Create a root binder for the view model
```C#
var binder = Binder.Source(viewModel);
```
##### Create a root binder and set the default subscription scheduler to schedule actions on the form's thread
```C#
var binder = Binder.Source(viewModel, new ControlScheduler(form));
```
##### Dispose of a binder
Diposing of a binder removes all bindings and disposes of all subcriptions created by the binder.
```C#
binder.Dispose();
```
##### Register external disposables with the binder
This can be used to tie the lifetime of other objects to the binder's lifetime.
```C#
binder.RegisterDisposable(disposableViewModel, disposableSubscription);
```
##### Bind a view model property to a control property
```C#
// Two-way binding
binder.Control(textBox).Property(c => c.Text).Bind(vm => vm.Name); 
// One-way binding from source to control
binder.Control(form).Property(c => c.UseWaitCursor).Get(vm => vm.IsBusy); 
// One-way binding from control to source
binder.Control(textBox).Property(c => c.Text).Set(vm => vm.Name); 
```
##### Bind an integer view model property to a string control property
```C#
binder.Control(textBox).Property(c => c.Text).Get(vm => vm.Age, Convert.ToString);
```
##### Subscribe to a button's click event
```C#
binder.Control(button).OnClick().Subscribe(_ => DoSomething());
```
##### Subscribe to a form's `MouseMove` event
```C#
binder.Control(form).Event<MouseEventArgs>(nameof(form.MouseMove))
  .Transform(o => o.Where(e => e.Args.Button==MouseButtons.Left).Select(e => new {e.Args.X, e.Args.Y})) 
  // Mouse coords are passed
  .Subscribe(a => DoSomething(a.X, a.Y));
```
##### Bind a form's `MouseMove` event arguments to a view model property
```C#
binder.Control(form).Event<MouseEventArgs>(nameof(form.MouseMove))
  .Transform(o => o.Select(e => new MyCoord{X = e.Args.X, Y = e.Args.Y}))
  .Set(vm => vm.CurrentMouseCoords);
```
##### Bind to a non-control target object
Non-control targets only support one-way binding from source to target.
```C#
binder.Target(target).Property(t => t.Status).Get(vm => vm.Status);
```
### Observable subscriptions

##### Trigger an action when a view model property changes
```C#
binder.OnPropertyChanged(vm => vm.ErrorMessage).Subscribe(msg => DisplayErrorDialog(msg));
```
##### Subscribe to a button's `Click` event to close the form
```C#
binder.Control(cancelButton).OnClick().Subscribe(_ => form.Close());
```
##### Subscribe to a form's `Closed` event to dispose of the binder
```C#
binder.Control(form).Event(nameof(form.Closed)).Subscribe(_ => binder.Dispose());
```
##### Create an observable subscription
```C#
binder.Observe(viewModel.Observable).Subscribe(_ => DoSomething());
// Override the default scheduler
binder.Observe(viewModel.Observable).ObserveOn(Scheduler.Immediate).Subscribe(_ => DoSomethingImmediately());
```
##### Subscribe to an observable with full subscription syntax support
```C#
binder.Observe(viewModel.Observable).Subscribe(
  ctx => ctx.OnNext(oVal => OnNextAction(oVal))
       .OnError(ex => HandleException(ex))
       .OnComplete(() => OnCompleteAction()));
```
##### Create an observable subscription to call an async method
```C#
binder.Observe(viewModel.Observable).SubscribeAsync(msg => command.ExecuteAsync(msg.Value));
```
## Event to observable conversion
```C#
IObservable<string> mouseMoveButtons =
  Binder.Observe(form).Event<MouseEventArgs>(nameof(form.MouseMove))
       .Select(e => Convert.ToString(e.Args.Button));
```
## Send value to `IObservable`
A view model will often want to send messages to the UI through an `IObservable` property. 
It can be useful to have the underlying value of that property also implement `IObserver`, e.g. by using `System.Reactive.Subjects.Subject`.
```C#
// Send on current thread
observable.Send(value);
// Send synchronously to synchronization context
observable.Send(message, syncContext);
// Send asynchronously to synchronization context
observable.Post(request, syncContext);
```