Fu# is a new take at an embedded web server library implementation that gives you full control over the request processing pipeline and doesn't depend on ASP.NET or IIS.

**See the [wiki][3] for an overview of how to use Fu#. For a more detailed use, try looking at the samples source code.**

This was my hobby project inspired by various other open source technologies and born from accumulated frustrations of trying to bend the ASP.NET pipeline / configure IIS7 so it behaves exactly as I wanted. So I built this thing in a way that all of pieces in the pipeline are easily replaceable, transparent, portable, embeddable and depend as little as possible on IIS/ASP.NET

You can build this with VS2008 and/or MSBuild. For the test, you'll need a test runner that can run MbUnit tests. But, being a hobby project there are only a few basic "it works" tests right now.

TODO
====

There're still much work to be done, right now some of the most important things are:

* HTTP Cache-Control support
* Threads fine-tuning/Change to an evented model
* Lots of null-checks/invariant checks and TODOs in code
* Lots of tests and optimizations

All feedbacks and contributions welcome!
Please post them to the google group

For any questions, you can ping me on twitter ([@chakrit][4] or post it to the temporary Google group: http://groups.google.com/group/fu-sharp)

Fu# is developed by [Chakrit Wichian][2] ([@chakrit][4] on Twitter) @ [2nitedesign! Co.,Ltd.][0] and is licensed under [The New BSD License][1].

  [0]: http://2nitedesign.com/
  [1]: http://en.wikipedia.org/wiki/BSD_licenses#3-clause_license_.28.22New_BSD_License.22.29
  [2]: http://chakrit.net/
  [3]: http://wiki.github.com/chakrit/fu-sharp/
  [4]: http://twitter.com/chakrit/