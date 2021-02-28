# SingleInstance-WPF-Sample
Sample single instance WPF project based on working code found in [NETworkManager](https://github.com/BornToBeRoot/NETworkManager)

## Use Mutex to check for single instance
```csharp
/// <summary>
/// This identifier must be unique for each application.
/// </summary>
private const string ApplicationSingleInstanceGuid = "{910e8c27-ab31-4043-9c5d-1382707e6c93}";
private static System.Threading.Mutex _singleInstanceMutex;

private void Application_Startup(object sender, StartupEventArgs e)
{
    // Create SingleInstance Mutex
    _singleInstanceMutex = new System.Threading.Mutex(true, ApplicationSingleInstanceGuid);
    var isOnlyInstance = _singleInstanceMutex.WaitOne(TimeSpan.Zero, true);

    // TODO: Read from settings if you want your users to be able to opt-in to multiple instance or not
    var settings_AllowMultipleInstances = false;
    if (settings_AllowMultipleInstances || isOnlyInstance)
    {
        // Show main window
        StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);

        // Release SingleInstance Mutex
        _singleInstanceMutex.ReleaseMutex();
    }
    else
    {
        // Bring the already running application into the foreground
        SingleInstance.PostMessage((IntPtr)SingleInstance.HWND_BROADCAST, SingleInstance.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
        Shutdown();
    }
}
```

## Bring to front already running application
```csharp
private System.Windows.Interop.HwndSource _hwndSoure;

protected override void OnSourceInitialized(EventArgs e)
{
    base.OnSourceInitialized(e);

    _hwndSoure = System.Windows.Interop.HwndSource.FromHwnd(new System.Windows.Interop.WindowInteropHelper(this).Handle);
    _hwndSoure?.AddHook(HwndHook);
}

[DebuggerStepThrough]
private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
{
    // Single instance --> Show window
    if (msg == SingleInstance.WM_SHOWME)
    {
        BringWindowToFront();
        handled = true;
    }

    return IntPtr.Zero;
}

private void BringWindowToFront()
{
    if (WindowState == WindowState.Minimized)
    {
        WindowState = WindowState.Normal;
    }

    Activate();
}
```

## Notes
* Don't forget to use an unique identifier for every application
* You can optionally add settings for the user to opt-in for single instance or multiple instance application
