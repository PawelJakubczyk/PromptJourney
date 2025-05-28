## How to use program

1. Build the command by chaining methods on **Command.Begin()**
2. Run program (F5)
3. Use the prompt that is copied to the clipboard

NOTE:

To stop the console from opening (so its easier to use)
1. click right to project
2. select "properties"
3. in "general" change output type to "windows application"

## Example command build

```
var command = Command
    .Begin()
    .Imagine("beauty landscape", 2)
    .Imagine("majestic", 2)
    .Imagine("mysterious", 1)
    .Imagine("dangerous", 1)
    .InStyle(Style.Dark_Fantasy)
    .InStyle(Style.Fantasy)
    .InTime(Time.Noon)
    .InSeason(Season.Spring)
    .InWeather(Weather.Rain)
    .InColor(Color.Dark_Purple)
    .InLight(Light.None)
    .PaintedBy(Artist.None)
    .WithQuality(Quality.VeryHigh)
    .WithRatio(1, 1)
    .Build();
```

1. Each method has second input for "weight" that is responsible for how important the phrase is
2. We can use multiple styles
3. We can use Imagine to add anything we want (event styles and other, but it is preferred to use conrete methods)
