Granular - WPF for JavaScript!
==============================

Granular is a re-implementation of WPF that was designed to run in the browser, it aims to bring to web applications the same development experience as WPF brings to desktop applications (with xamls, code-behind, _real_ MVVM and everything that is good).

Using [Saltarelle C# to JavaScript compiler](http://www.saltarelle-compiler.com), Granular fills the gap between a simple JavaScript "Console Application" and a full "WPF Application".


See it in action
----------------

[GranularPad](http://yuvaltz.github.io/granularpad) is a good showcase of the current Granular framework capabilities (don't forget to inspect the visual tree in the DOM :)


How does it work?
-----------------

Granular can be separated into two parts:
* **Presentation** - Maintains a visual tree (with dependency properties, routed events, animations and so on) which is completely separated from its host, exactly the same way as WPF do.
* **Host** - Contains a minimal rendering engine and a raw events source, the Presentation part interacts with the Host the same way as WPF interacts with MilCore (DirectX adapter) and WndProc messages.


Creating a new Granular application
-----------------------------------

Granular is portable and can be hosted in many environments, at the moment it can be hosted in a Windows desktop application and in the browser.

Creating a Granular application is as easy as creating any other WPF application - you can fully run, test and debug it with Visual Studio (and even inspect it to some extent with [Snoop](http://snoopwpf.codeplex.com)), while you can simultaneously compile it to JavaScript and run it in the browser (and inspect the DOM).

[See a complete guide here](https://github.com/yuvaltz/Granular/wiki/Creating-a-new-Granular-application)
