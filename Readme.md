Granular - WPF for JavaScript!
==============================

Granular is a re-implementation of WPF which is designed to run in the browser, it aims to bring to web applications the same development experience as WPF brings to desktop applications (with xamls, code-behind, _real_ MVVM).

Using [Saltarelle C# to JavaScript compiler](http://www.saltarelle-compiler.com), Granular fills the gap between a simple JavaScript "Console Application" and a full "WPF Application".


See it in action
----------------

[GranularPad](http://yuvaltz.github.io/granularpad) is a good showcase of the current Granular framework capabilities (don't forget to inspect the visual tree in the DOM :)


How does it work?
-----------------

Granular can be separated into two parts:
* **Presentation** - Maintains a visual tree (with dependency properties, routed events, animations and so on) which is completely separated from its host, exactly the same way as WPF does.
* **Host** - Contains a minimal rendering engine and a raw events source, the Presentation part interacts with the Host the same way as WPF interacts with MilCore (DirectX adapter) and WndProc messages.


Creating a new Granular application
-----------------------------------

Granular aims to seamlessly replace WPF so that Granular applications could be created **in addition** to existing WPF applications, share the same source code, and deploy to a different environment.

In addition to a browser, Granular can also be hosted in a desktop application, so that verifying compatibility with WPF could be easily done with Visual Studio.

More details on Granular applications configuration [can be found here](https://github.com/yuvaltz/Granular/wiki/Creating-a-new-Granular-application)


Supported features
------------------
A list of the currently supported features [can be found here](https://github.com/yuvaltz/Granular/wiki/Supported-features)
