<img src="https://github.com/GabrielTK/DropMeter/raw/master/DropMeter/Resources/logo.png" width="100" height="100">

# DropMeter
Modern, easy way to create Widgets for your Desktop.

## What is this?

DropMeter is a software that allows for desktop widgets to be built with the well-known and loved Web Stack, making use of the latest Web Technologies.

Think Rainmeter, but with richer and easier to make widgets.

## Is this a replacement for Rainmeter?

I wouldn't say a _replacement_, per see. Rainmeter was initially releasen back in 2001, and it excels at rendering widgets with low resource usage. However, times have changed, and with the new design trends, many crave for more interactive, responsive and intelligent widgets. That's where DropMeter comes in.
I personally like to combine both, running clocks and audio visualizers on RainMeter and my fancy things on DropMeter.

## Is there any advantage of combining it with Rainmeter, then?

Apart from having a really fancy desktop? Yes, there actually is. We are working on a way to enable complete RainMeter compatibility! Which means that, when a plugin is not available, but you have it installed in RainMeter, it will be able to load it! (I am still working on that feature, so It's not published yet, but it soon will be)

## Can I do (insert fancy rainmeter widget feature here)?

Probably yes! But if you cannot, don't worry, we are paving the way to enable you to do it.
Like RainMeter, DropMeter is built on plugins that can send and receive data from widgets. However, there is an essential difference: All DropMeter Plugins are built with C#, and thus Garbage Collected, therefore making it extremely difficult to have memory leaks and such.
If you are a Developer, don't worry, we have built things so that using our fancy plugin system is as easy as requiring libraries available in NuGet (for plugins) / npm (for Widgets; of course with TypeScript definitions, we are decent people after all) and extending an interface / registering your callbacks.
We even have a built-in WebNowPlaying Plugin, so you don't need to make that extra effort to display your songs information.

## I hear the Web Stack is heavy... Will my memory go brrrr?

I wouldn't worry: Our runtime is pretty optimized, and seems to be able to handle things quite smoothly.
Testing on my machine with my word-by-word lyrics widget (quite heavy, uses React, Framer Motion animations and an interval-based tick system) tops usage at 1% CPU and 79MB on RAM.

## Cool... Where Can I get it?

Happy you are excited! [Downloads are available by clicking here](https://github.com/GabrielTK/DropMeter/releases/latest)

**If you have any doubts, feel free to contact @gab#8998 on Discord DMs, or join the [CGN Interactive Projects Discord Server](https://discord.gg/DBE44yU)**
