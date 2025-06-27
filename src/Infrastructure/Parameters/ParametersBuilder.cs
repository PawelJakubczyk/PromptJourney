using Utilities.ResultPattern;

namespace Infrastructure.Parameters;

public class ParametersBuilder
{
    private Guid _parametersId = Guid.NewGuid();
    private int? _aspectRatioX = 16;
    private int? _aspectRatioY = 9;
    private string? _version = "6.1";
    private int? _stylize = 750;
    private int? _quality = 1;
    private int? _chaos = 0;
    private long? _seed = null;
    private string? _styleReference = null;
    private string? _characterReference = null;
    private string? _no = null;
    private string? _profile = null;
    private int? _repeat = null;
    private int? _stop = null;
    private bool _raw = false;
    private bool _tile = false;
    private bool _draft = false;
    private int? _weird = null;
    private double? _imageWeight = null;
    private string? _mode = null;
    private string? _niji = null;
    private string? _visibilityMode = "stealth";

    private readonly List<string> _errors = [];

    public ParametersBuilder SetAspectRatio(int x, int y)
    {
        if (x <= 0 || y <= 0)
            _errors.Add("Aspect ratio values must be greater than 0.");
        else
        {
            _aspectRatioX = x;
            _aspectRatioY = y;
        }
        return this;
    }

    public ParametersBuilder SetStylize(int s)
    {
        if (s < 0 || s > 1000)
            _errors.Add("Stylize must be between 0 and 1000.");
        else _stylize = s;
        return this;
    }

    public ParametersBuilder SetQuality(int q)
    {
        var allowed = new HashSet<int> { 1, 2, 4 };
        if (!allowed.Contains(q))
            _errors.Add("Invalid quality value. Allowed values: 1, 2, 4.");
        else _quality = q;
        return this;
    }

    public ParametersBuilder SetChaos(int c)
    {
        if (c < 0 || c > 100)
            _errors.Add("Chaos must be between 0 and 100.");
        else _chaos = c;
        return this;
    }

    public ParametersBuilder SetSeed(long? s)
    {
        if (s.HasValue && (s < 0 || s > 4294967295))
            _errors.Add("Seed must be between 0 and 4294967295.");
        else _seed = s;
        return this;
    }

    public ParametersBuilder SetVersion(string? v)
    {
        var allowed = new HashSet<string> { "1", "2", "3", "4", "5", "5.1", "5.2", "6", "6.1", "7" };
        if (string.IsNullOrWhiteSpace(v))
            _errors.Add("Version cannot be empty.");
        else if (!allowed.Contains(v))
            _errors.Add("Invalid version value. Allowed values: 1, 2, 3, 4, 5, 5.1, 5.2, 6, 6.1, 7");
        else _version = v;
        return this;
    }

    public ParametersBuilder SetStyleReference(string? sr)
    {
        _styleReference = sr;
        return this;
    }

    public ParametersBuilder SetCharacterReference(string? cr)
    {
        _characterReference = cr;
        return this;
    }

    public ParametersBuilder SetNo(string? n)
    {
        _no = n;
        return this;
    }

    public ParametersBuilder SetProfile(string? p)
    {
        _profile = p;
        return this;
    }

    public ParametersBuilder SetRepeat(int? r)
    {
        if (r.HasValue && (r < 1 || r > 40))
            _errors.Add("Repeat must be between 1 and 40.");
        else _repeat = r;
        return this;
    }

    public ParametersBuilder SetStop(int? s)
    {
        if (s.HasValue && (s < 10 || s > 100))
            _errors.Add("Stop must be between 10 and 100.");
        else _stop = s;
        return this;
    }

    public ParametersBuilder SetRaw(bool r)
    {
        _raw = r;
        return this;
    }

    public ParametersBuilder SetTile(bool t)
    {
        _tile = t;
        return this;
    }

    public ParametersBuilder SetDraft(bool d)
    {
        _draft = d;
        return this;
    }

    public ParametersBuilder SetWeird(int? w)
    {
        if (w.HasValue && (w < 0 || w > 3000))
            _errors.Add("Weird must be between 0 and 3000.");
        else _weird = w;
        return this;
    }

    public ParametersBuilder SetImageWeight(double? iw)
    {
        if (iw.HasValue && (iw < 0.0 || iw > 2.0))
            _errors.Add("Image weight must be between 0.0 and 2.0.");
        else _imageWeight = iw;
        return this;
    }

    public ParametersBuilder SetMode(string? m)
    {
        var allowed = new HashSet<string> { null!, "fast", "relax", "turbo" };
        if (!allowed.Contains(m))
            _errors.Add("Mode must be one of: fast, relax, turbo.");
        else _mode = m;
        return this;
    }

    public ParametersBuilder SetNiji(string? n)
    {
        if (!string.IsNullOrWhiteSpace(n) && !n.StartsWith("niji"))
            _errors.Add("Niji must start with 'niji'.");
        else _niji = n;
        return this;
    }

    public ParametersBuilder SetVisibilityMode(string? v)
    {
        if (string.IsNullOrWhiteSpace(v) || (v != "stealth" && v != "public"))
            _errors.Add("VisibilityMode must be either 'stealth' or 'public'.");
        else _visibilityMode = v;
        return this;
    }

    public Result<Parameters> Build()
    {
        if (_errors.Any())
            return Result<Parameters>.Failure(_errors.ToArray());

        string aspectRatio = $"{_aspectRatioX}:{_aspectRatioY}";

        var parameters = new Parameters
        (
            _parametersId,
            aspectRatio,
            _version,
            _stylize,
            _quality,
            _chaos,
            _seed,
            _styleReference,
            _characterReference,
            _no,
            _profile,
            _repeat,
            _stop,
            _raw,
            _tile,
            _draft,
            _weird,
            _imageWeight,
            _mode,
            _niji,
            _visibilityMode
        );

        return Result<Parameters>.Success(parameters);
    }
}
