# Matrix ScreenSaver: User's Guide



## Downloads & Runtimes

Download the latest release from the [Releases page](https://github.com/RemusRigo/MatrixScreenSaver-VB.NET/releases).

Each release contains one folder per framework and architecture:

|Folder|Target Framework|Architecture|Required Runtime|
|-|-|-|-|
|`net8.0 x64`|.NET 8.0|64-bit|[.NET 8.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0)|
|`net8.0 x86`|.NET 8.0|32-bit|[.NET 8.0 Desktop Runtime (x86)](https://dotnet.microsoft.com/download/dotnet/8.0)|
|`net10.0 x64`|.NET 10.0|64-bit|[.NET 10.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0)|
|`net10.0 x86`|.NET 10.0|32-bit|[.NET 10.0 Desktop Runtime (x86)](https://dotnet.microsoft.com/download/dotnet/10.0)|

> **Which one do I need?** On 64-bit Windows 10/11, use **x64**. Pick the .NET version whose
> **Desktop Runtime** you already have installed, or install one from the links above.
> The plain ".NET Runtime" is not enough; the screensaver needs the **Desktop** Runtime.

To check which runtimes are installed, open a command prompt and run:

```
dotnet --list-runtimes
```

Look for a `Microsoft.WindowsDesktop.App 8.x` or `10.x` line.



## Install

1. Download and extract the folder that matches your system (see the table above), e.g. `net8.0 x64`.
2. Copy the **whole folder** to a permanent location, for example:
   `D:\My Apps\Matrix ScreenSaver\`
   The folder must contain all 4 files; the screensaver won't start if any of them is missing:
   * `MatrixSS.scr`
   * `MatrixSS.dll`
   * `MatrixSS.deps.json`
   * `MatrixSS.runtimeconfig.json`
3. Right-click `MatrixSS.scr` and choose **Install**.
   The Windows **Screen Saver Settings** dialog opens with *MatrixSS* selected.
4. Set the **Wait** time and click **OK**.

> If Windows blocks the file ("Windows protected your PC"), right-click `MatrixSS.scr` →
> **Properties** → check **Unblock** → **OK**, then try again.



## Test it

* In **Screen Saver Settings**, click **Preview**, or
* double-click `MatrixSS.scr`.

Columns of characters rain down every monitor, each with a bright head and a fading trail.
The center column carries the message `© 2026 Remus Rigo`.
Press any key, click, or move the mouse to exit.

> Right-click `MatrixSS.scr` → **Configure** opens the **Settings** window.



## Settings

Open **Screen Saver Settings** (Start → type *screen saver*), select *MatrixSS*, and click **Settings...**

|Setting|Options|Default|Description|
|-|-|-|-|
|**Char set**|Katakana, A-Z, a-z, 0-9|Katakana|Characters used in the rain; any combination can be checked|
|**Color**|Red, Green, Blue|Green|Color of the rain; the head of each column is drawn near-white|

> If no character set is checked, Katakana is used.

Changes are saved as soon as you make them; click **Ok** to close the window.
The new settings are used the next time the screensaver (or the preview) starts.
Settings are stored per user in the registry under `HKEY_CURRENT_USER\Software\Remus RIGO\MatrixSS`.
The version is shown in the title bar; click the link in the status bar to open the project page.



## Multi-monitor

The rain fills every monitor; each monitor gets its own set of columns and its own message column in the center.



## Uninstall

1. In **Screen Saver Settings**, choose another screensaver (or *None*) and click **OK**.
2. Delete the folder you copied in step 2 of *Install*.
3. Optional: delete the registry key `HKEY_CURRENT_USER\Software\Remus RIGO\MatrixSS`.
