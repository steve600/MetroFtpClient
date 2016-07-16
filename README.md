# MetroFtpClient

FTP-Client based on the [PrismMahAppsSample](https://github.com/steve600/PrismMahAppsSample) and the standard .NET classes [FtpWebRequest](https://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest(v=vs.100).aspx)/[FtpWebRespone](https://msdn.microsoft.com/en-us/library/system.net.ftpwebresponse(v=vs.100).aspx). Again some open source components are used:

* [Dragablz](https://github.com/ButchersBoy/Dragablz) - Dragable and tearable tab control for WPF
* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro) - "Metro" or "Modern UI" for WPF applications
* [MaterialDesignInXamlToolkit](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit) - Material Design templates and styles for WPF
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) - JSON framework for .NET
* [OxyPlot](https://github.com/oxyplot/oxyplot) - Plotting library for .NET
* [Prism](https://github.com/PrismLibrary/Prism) - Application framework which provides an implementation of a collection of design patterns (MVVM, EventAggregator, ...) that are helpful in writing well structured and maintainable applications
* [WpfLocalizeExtension](https://github.com/SeriousM/WPFLocalizationExtension) - Library for the localization

Here are some screenshots:

Within the overview page it is possible to create new connections. If a connection is opened the following screen appears:

![Connection view](http://csharp-blog.de/wp-content/uploads/2016/07/MetroFtpClient_02.png)

Within the connection view there is a local and remote file explorer, the queue for downloads/uploads, a log window (for the FTP messages) and a diagram with the connection speed for the selected queue entry. It's possible to open multiple connections simultaneously.

This is a very early version so there are still some open points:

* finish queue handling
* error handling
* upload functionality
* Drag&Drop for downloads/uploads
* timeout handling
* ...


