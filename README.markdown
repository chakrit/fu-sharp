Fu#
---

Bends the pipeline to your will with Fu#. An embedded web server implementation in pure functional-style C# with a focus on maximum compositionality and modifiability.

Not everyone will understands what this is, but if you like LINQ or functional programming in general... or you use F#, you'll like Fu#. Especially so if you have had nightmares configuring and figuring out the unusable behemoth that is IIS7.

Some of the benefits:

* **Replacable services** -- sessions, model binders, parsers can be mocked and replaced etc.
  * Scalable sessions? -- Just implement your own ISessionService.
  * Wacky model binders needed? -- Just implement a `Bind(T yourObj)` method and add it to the model binders.
  * Custom protocol? -- Implement an IService that parses the protocol.
* **Easily bendable pipeline** -- entirely in C# no XML no configuration needed.
  * Canonical URLs? -- Just add a string replace method.
  * Selective Compression? -- Just add compression methods where you need them and only where you need them.
  * Like ruby? -- Try the RestStyleController inspired by Sinatra.
  * Wildcard subdomains? -- `app.Settings.Host = "*.domain.com:80";` done.
* **Code in any style you want.** -- No IIS worries, no complex unmovable stuff in ASP.NET to workaround.
  * HttpModules, HttpHandlers, ISAPI modules you *don't* need any of that, Fu# does all they can and much more with even less code.
  * No style is ever imposed on you that you cannot change. -- This is due to the functional nature of Fu#. Your app can be as simple as a single method or as complex as any enterprise apps.
* **Deployment heaven** -- Since it's embedded, you can deploy it whatever way you want.
  * EXE? -- Make an EXE app an references `fu.dll`.
  * Windows Service? -- Make a windows services app and references `fu.dll`.
  * WPF-class management interface? -- Just make a WPF app and references `fu.dll`.
  * Shrinkwrap product? -- Well... just make an EXE and shrinkwrap that.
  * Security? -- Well... you do know how to secure a simple service or EXE right?
* **Easier and more performant asynchronous code.** -- Using [Continuation-Passing Style][5].
  * Comets support -- Using CPS you can leave the connection open as long as you want.
  * Streaming. -- Throw in some buffers and `File.BeginRead` then `Stream.BeginWrite`, 'nuff said.

**See the [wiki][3] for an overview of how to use Fu#. For a more detailed use, checkout the samples folder.**

This was my hobby project inspired by various other open source technologies and born from accumulated frustrations of trying to bend the ASP.NET pipeline / configure IIS7 so it behaves exactly as I wanted. So I built this thing in a way that all of pieces in the pipeline are easily replaceable, transparent and reusable and depend as little as possible on IIS/ASP.NET.

You can be confident that this will be kept maintained and bugs fixed as I'm using this to run a few production sites right now and also I will be using Fu# to do all my future web projects.

----

TODO
----

If you like it, please help! I'm using this in a few production sites right now so I'm not going anywhere but there're still much work to be done, right now some of the most important things are:

* Tutorials on common basic tasks
* Lots of real-world tests

Or if you prefer more interesting stuffs:

* Create a lightweight wrapper around HttpListener classes
* Constants fine-tuning (threads number, buffer size etc.)
* HTTP Cache-Control support
* HTTP specs verification
* Lots of null-checks/invariant checks and TODOs in code
* Simplification or various parts which are sometimes hard to understand.

All feedbacks and contributions greatly appreciated!

For any questions, you can ping me on twitter [@chakrit][4] or post it to the temporary [fu-sharp Google group][6]

---

Fu# is developed and maintained by [@chakrit][4]/[website][2] of [2nitedesign!][0] and is licensed under [The New BSD License][1].

  [0]: http://2nitedesign.com/
  [1]: http://en.wikipedia.org/wiki/BSD_licenses#3-clause_license_.28.22New_BSD_License.22.29
  [2]: http://chakrit.net/
  [3]: http://wiki.github.com/chakrit/fu-sharp/
  [4]: http://twitter.com/chakrit/
  [5]: http://en.wikipedia.org/wiki/Continuation-passing_style
  [6]: http://groups.google.com/group/fu-sharp