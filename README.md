Fu# is a new take at an embedded web server library implementation that gives you full control over the request processing pipeline and doesn't depend on ASP.NET or IIS.

You can find it on GitHub: http://github.com/chakrit/fu-sharp

This was my hobby project inspired by various other open source technologies and born from accumulated frustrations of trying to bend the ASP.NET pipeline / configure IIS7 so it behaves exactly as I wanted.

So I built this thing in a way that all of pieces in the pipeline are easily replaceable, transparent, portable, embeddable and depend as little as possible on IIS/ASP.NET

You can build this with VS2008 and/or MSBuild. For the test, you'll need a test runner that can run MbUnit tests. But, being a hobby project there are only a few basic "it works" tests right now.

==A Quick HOWTO==

Here's the simplest hello world app built with Fu:

	new SimpleApp(fu.Static.Text("Hello World!")).Start();

A fu app is composed up from Steps and Services. Steps are actually just a `Func<IFuContext, IFuContext>` delegate (i.e. a piece of the pipeline, something to process the current context) and Services are there to provide ambient services that's not suited to being implemented as a step such as parsers and sessions.

For example, here's another more elaborate version of a hello world app in fu:

	var steps = new Step[] {
		fu.Static.Text("Hello World!"),
		fu.Result.Render()
	};

	var app = new App(null, null, steps);
	app.Start();

The static `fu` classes house various standard Steps and a few helper methods to work with steps. In the above code, I've used the `fu.Static.Text` method to produce a constant string as a result disregarding whatever is inside the context. And then the `fu.Result.Render()` to render the result from previous step.

I then proceeds to compose up an application by creating an `App` and supplying the steps (the pipelines) just made. and `Start()` it.

Try hitting http://localhost/ now and you'll be greeted with a Hello World! in fu!

Note that you don't even have to deal with URLs! Hit any URL and it'll still return "Hello World!" because that's the only thing in the entire pipeline. This can make for a very efficient web server if needed (for example, a HTTP-based message queue)

When you are working with the most basic `App` class, you are starting on a blank slate adding things you need as you go, one piece of the pipeline at a time. Nothing is done in the background without you knowing.

Fu, however, does provide you with a few basic Steps to work on URLs and it's very easy to roll your own. Here's a Hello World! with URL mapping:

	var app = new SimpleApp(fu.Map.Urls(new[] {
		new UrlMap("^/hello$", fu.Static.Text("Hello World!")),
		new UrlMap("^/number$", fu.Static.Text("42""))
	}));

    app.Start();
    
Try hitting /hello and you'll then be greeted yet again. But now, try http://localhost/ (root url) you'll then be presented with a `NotFound` text because it isn't properly mapped, yet this, too, can be replaced if you provide a 404 step to `fu.Map.Urls`.

----

For more info, please look at the source code and the sample applications for now, until I have more time to write a proper tutorial.

Fu# is developed by [Chakrit Wichian][3] of [2nitedesign! Co.,Ltd.][0] and is licensed under [The New BSD License][1].

  [0]: http://2nitedesign.com
  [1]: http://en.wikipedia.org/wiki/BSD_licenses#3-clause_license_.28.22New_BSD_License.22.29
  [2]: http://twitter.com/chakrit