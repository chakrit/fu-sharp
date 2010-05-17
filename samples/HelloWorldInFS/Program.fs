
open Fu
open Fu.Steps

// Create the settings
let settings = new FuSettings()
settings.Hosts <- [| "localhost:80" |]

// Compose the pipeline
let pipeline = [| fu.Static.Text("Hello World!"); fu.Result.Render() |]

// Build the app
let app = new App(settings, null, pipeline)
app.Start()
