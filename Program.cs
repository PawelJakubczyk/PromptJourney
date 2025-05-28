using TextCopy;
using mjCommandGenerator;
using mjCommandGenerator.Enums;

//This will copy the command to the memory (Clipboard)
var command = Command
    .Begin()
    //.Imagine("simple symbol emblem", 30)
    .Imagine("beauty landscape", 2)
    .Imagine("majestic", 2)
    .Imagine("mysterious", 1)
    .Imagine("dangerous", 1)
    // .Imagine("god of death", 2)
    //.Imagine("calligraphy", 2)
    //.Imagine("ink", 2)
    //.Imagine("Medieval Parchment", 2)
    //.Imagine("Basic", 1)
    //.Imagine("Simple", 1)
    //.Imagine("Simplicity", 1)
    .InStyle(Style.Dark_Fantasy)
    .InStyle(Style.Fantasy)
    .InTime(Time.Noon)
    .InSeason(Season.Spring)
    .InWeather(Weather.Rain)
    .InColor(Color.Dark_Purple)
    .InLight(Light.None)
    .PaintedBy(Artist.None)
    .WithQuality(Quality.VeryHigh)
    //.WithStylize(2500)
    .WithRatio(1, 1)
    .Build();

//var command = Command
//    .Begin()
//    .Imagine("three towers", 2)
//    .Imagine("castle on the hill", 2)
//    .Imagine("majestic", 1)
//    .Imagine("Painted By Alyssa Monks")
//    .Imagine("Painting By Albert Bierstadt")
//    .Imagine("Painting By Marc Simonetti")
//    .Imagine("Charcoal Art")
//    .Imagine("Fable 2 Style")
//    .Imagine("Graphite")
//    .InStyle(Style.Realistic)
//    .InStyle(Style.Dreamlike)
//    .InStyle(Style.Nostalgiacore)
//    .InTime(Time.Afternoon)
//    .InSeason(Season.None)
//    .InWeather(Weather.None)
//    .InColor(Color.None)
//    .InLight(Light.None)
//    .PaintedBy(Artist.None)
//    .WithQuality(2)
//    .WithStylize(2500)
//    .WithRatio(1, 1)
//    .Build();

ClipboardService.SetText($"{command}");