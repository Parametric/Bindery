Bindery
=======
Bindery aims to support fluent MVVM binding definition for WinForms applications.

Projects
--------
* **Bindery.Core:** 
  * Contains the static Create factory class 
  * Contains CommandBase (an abstract base implementation of ICommand)
  * Dependent on the Rx-Linq package
* **Bindery.WinForms:** 
  * Contains an extension method for ISourceBinder that allows bindings to WinForms controls to be created
  * Dependent on Bindery.Core and System.Windows.Forms.

Assumptions
-----------
Full binding functionality requires view models to properly implement INotifyPropertyChanged.

Code Examples
-------------
### Binding
#### Create a root binder for the view model.
<pre><code>var binder = Create.Binder(viewModel);</code></pre>

#### Remove all bindings and dispose of all subcriptions created by a binder.
<pre><code>binder.Dispose();</code></pre>

#### Bind a view model property to a control property.
<pre><code>binder.Control(textBox).Property(c => c.Text).Bind(vm => vm.Name); // Two-way binding
binder.Control(form).Property(c => c.UseWaitCursor).Get(vm => vm.IsBusy); // One-way binding from source to control
binder.Control(textBox).Property(c => c.Text).Set(vm => vm.Name); // One-way binding from control to source
</code></pre>

#### Set up a conversion when the view model property's type is different than the control property's type.
<pre><code>binder.Control(textBox).Property(c => c.Text).Get(vm => vm.Age, Convert.ToString);
</code></pre>

#### Bind a control's Click event to a command. This also "binds" the control's Enabled property to the command's CanExecute method.
<pre><code>ICommand command = new CommandImplementation(viewModel);
binder.Control(textBox).OnClick(command);
</code></pre>

#### Bind a control's event to a command.
<pre><code>ICommand command = new CommandImplementation(viewModel);
binder.Control(form).OnEvent&lt;MouseEventArgs&gt;("MouseMove")
  .Transform(o => o.Where(e => e.Button==MouseButtons.Left).Select(e => new {e.X, e.Y})) 
  // Mouse coords are passed to command.Execute()
  .Execute(command);
</code></pre>

#### Bind a control's event arguments to a view model property.
<pre><code>ICommand command = new CommandImplementation(viewModel);
binder.Control(form).OnEvent&lt;MouseEventArgs&gt;("MouseMove")
  .Transform(o => o.Select(e => new MyCoord{X = e.X, Y = e.Y}))
  .Set(vm => vm.CurrentMouseCoords);
</code></pre>

#### Bind to a non-control target object. Non-control targets support a limited set of binding options. Two-way binding and one-way binding from target to source are not supported.
<pre><code>binder.Target(target).Property(t => t.Status).Get(vm => vm.Status);</code></pre>

#### Trigger an action when a view model property changes.
<pre><code>binder.OnPropertyChanged(vm => vm.ErrorMessage).Subscribe(msg => DisplayErrorDialog(msg));</code></pre>

#### Subscribe to an observable to execute a command.
<pre><code>binder.Observe(viewModel.Observable).Execute(command);</code></pre>

#### Subscribe to an observable with full subscription support.
<pre><code>binder.Observe(viewModel.Observable).Subscribe(
  ctx=>ctx.OnNext(oVal => OnNextAction(oVal))
       .OnError(ex => HandleException(ex))
       .OnComplete(() => OnCompleteAction()));
</code></pre>

### Event to observable conversion
<pre><code>IObservable&lt;string&gt; mouseMoveButtons =
  Create.ObservableFor(form).Event&lt;MouseEventArgs&gt;("MouseMove").Select(e => Convert.ToString(e.Button));
</code></pre>

### CommandBase ###
CommandBase is an abstract implementation of ICommand designed to be useful for MVVM command implementation.

* Execute() is abstract and must be implemented by an inheriting class.
* CanExecute() is implemented to return true but can be overridden by an inheriting class.
* The CanExecuteChanged event has a protected OnCanExecuteChanged() invocator method.
* A new ObserveCanExecuteChanges() method creates an IObservable<bool> that publishes the value of CanExecute() each time it changes, assuming the CanExecuteChanged event is triggered properly.
