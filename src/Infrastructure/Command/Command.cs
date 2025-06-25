//using System.Text;
//using System.Globalization;
//using mjCommandGenerator.Enums;

//namespace mjCommandGenerator;

//public class Command
//{
//    private readonly StringBuilder _stringBuilder = new("/imagine prompt:");
//    private readonly List<Input> _imagine = new();
//    private readonly List<Input> _styles = new();
//    private readonly List<Input> _colors = new();
//    private Input? _light;
//    private Input? _artist;
//    private Input? _time;
//    private Input? _season;
//    private Input? _weather;
//    private decimal _quality = 1;
//    private int _stylize = 2500;
//    private int _width = 1;
//    private int _height = 1;

//    private readonly NumberFormatInfo numberFormatInfo = new() { NumberDecimalSeparator = "." };

//    private Command()
//    {
        
//    }

//    public static Command Begin()
//    {
//        return new Command();
//    }

//    public Command Imagine(string imagine, double weight = 1)
//    {
//        _imagine.Add(new(imagine, weight));
//        return this;
//    }

//    public Command PaintedBy(Artist artist, double weight = 1)
//    {
//        if (artist is not Artist.None)
//            _artist = new Input($"Painting By {artist.ToString().Replace('_', ' ')}", weight);

//        return this;
//    }

//    public Command InStyle(Style style, double weight = 1)
//    {
//        if (style is not Style.None)
//            _styles.Add(new Input($"{style.ToString().Replace('_', ' ')}", weight));

//        return this;
//    }

//    public Command InTime(Time time, double weight = 1)
//    {
//        if (time is not Time.None)
//            _time = new Input($"{time}", weight);

//        return this;
//    }

//    public Command InSeason(Season season, double weight = 1)
//    {
//        if (season is not Season.None)
//            _season = new Input($"{season}", weight);

//        return this;
//    }

//    public Command InWeather(Weather weather, double weight = 1)
//    {
//        if (weather is not Weather.None)
//            _weather = new Input($"{weather}", weight);

//        return this;
//    }

//    public Command InColor(Color color, double weight = 1)
//    {
//        if (color is not Color.None)
//            _colors.Add(new Input($"{color.ToString().Replace('_', '-')}", weight));

//        return this;
//    }

//    public Command InLight(Light light, double weight = 1)
//    {
//        if (light is not Light.None)
//            _light = new Input($"{light.ToString().Replace('_', ' ')}", weight);

//        return this;
//    }

//    /// <summary>
//    /// Allowed qualities: 0.25, 0.5, 1, 2, 5
//    /// </summary>
//    /// <param name="quality"></param>
//    /// <returns></returns>
//    public Command WithQuality(Quality quality)
//    {
//        _quality = quality switch
//        {
//            Quality.VeryLow => 0.25m,
//            Quality.Low => 0.5m,
//            Quality.Normal => 1m,
//            Quality.High => 2m,
//            Quality.VeryHigh => 5m,
//            _ => throw new NotSupportedException()
//        };

//        return this;
//    }

//    /// <summary>
//    /// Value from 0 by 100 to 1000
//    /// </summary>
//    /// <param name="stylize"></param>
//    /// <returns></returns>
//    public Command WithStylize(int stylize = 500)
//    {
//        _stylize = stylize;
//        return this;
//    }

//    public Command WithRatio(int height = 1, int wight = 1)
//    {
//        _width = wight;
//        _height = height;
//        return this;
//    }

//    public string Build()
//    {
//        _imagine.ForEach(input => AppendCommand(input));
//        _styles.ForEach(input => AppendCommand(input));

//        AppendCommand(_time);
//        AppendCommand(_weather);
//        AppendCommand(_season);

//        _colors.ForEach(input => AppendCommand(input));

//        AppendCommand(_light);
//        AppendCommand(_artist);

//        //Remove last comma
//        _stringBuilder.Remove(_stringBuilder.Length - 2, 1);

//        AppendParameter("quality", _quality);
//        AppendParameter("stylize", _stylize);
//        AppendRatio();

//        string command = _stringBuilder.ToString();
//        return command;
//    }

//    private void AppendCommand(Input? input)
//    {
//        if (input is null)
//        {
//            return;
//        }

//        if (input.Weight is not 1)
//        {
//            _stringBuilder.Append($"{input.Value}::{input.Weight.ToString(numberFormatInfo)}, ");
//            return;
//        }

//        _stringBuilder.Append($"{input.Value}, ");
//    }

//    private void AppendParameter(string name, int value)
//    {
//        if (value is not 1 and not 2500)
//            _stringBuilder.Append($"--{name} {value} ");
//    }
    
//    private void AppendParameter(string name, decimal value)
//    {
//        if (value is not 1 and not 2500)
//            _stringBuilder.Append($"--{name} {value.ToString().Replace(",", ".")} ");
//    }

//    private void AppendRatio()
//    {
//        if (_height is 1 && _width is 1)
//            return;

//        _stringBuilder.Append($"--aspect {_height}:{_width}");
//    }
//}