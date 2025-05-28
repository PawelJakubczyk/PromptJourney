//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using Domain.ValueObjects;

//namespace Domain.Entities.MidjourneyVersions;

//public class MidjourneyVersion
//{
//    [Key]
//    [Required, MaxLength(7)]
//    public required MidjourneyVersion Version { get; set; }

//    [Required, MaxLength(10)]
//    public required string VersionAbbrev { get; set; }

//    [Column("aspect_ratio/height/width", TypeName = "jsonb")]
//    public ParameterDefinitionRange? AspectRatioHeightWidth { get; set; }

//    [Column("style", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Style { get; set; }

//    [Column("stylize", TypeName = "jsonb")]
//    public ParameterDefinitionRange? Stylize { get; set; }

//    [Column("quality/hd/hq", TypeName = "jsonb")]
//    public ParameterDefinitionRange? QualityHdHq { get; set; }

//    [Column("relax/fast/turbo", TypeName = "jsonb")]
//    public ParameterDefinitionModes? RelaxFastTurbo { get; set; }

//    [Column("chaos", TypeName = "jsonb")]
//    public ParameterDefinitionRange? Chaos { get; set; }

//    [Column("weird", TypeName = "jsonb")]
//    public ParameterDefinitionRange? Weird { get; set; }

//    [Column("raw", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Raw { get; set; }

//    [Column("tile", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Tile { get; set; }

//    [Column("no", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? No { get; set; }

//    [Column("vibe", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Vibe { get; set; }

//    [Column("test/testp", TypeName = "jsonb")]
//    public ParameterDefinitionTestTestp? TestTestp { get; set; }

//    [Column("creative", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Creative { get; set; }

//    [Column("newclip", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Newclip { get; set; }

//    [Column("nostretch", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Nostretch { get; set; }

//    [Column("prompt_weight", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? PromptWeight { get; set; }

//    [Column("sameseed", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Sameseed { get; set; }

//    [Column("permutation", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Permutation { get; set; }

//    [Column("repeat", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Repeat { get; set; }

//    [Column("seed", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Seed { get; set; }

//    [Column("personalization", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Personalization { get; set; }

//    [Column("cref", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Cref { get; set; }

//    [Column("sref", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Sref { get; set; }

//    [Column("oref", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Oref { get; set; }

//    [Column("image_prompt", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? ImagePrompt { get; set; }

//    [Column("image_weight", TypeName = "jsonb")]
//    public ParameterDefinitionRange? ImageWeight { get; set; }

//    [Column("stop", TypeName = "jsonb")]
//    public ParameterDefinitionRange? Stop { get; set; }

//    [Column("video", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Video { get; set; }

//    [Column("variations", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Variations { get; set; }

//    [Column("remix", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Remix { get; set; }

//    [Column("editor", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Editor { get; set; }

//    [Column("full_editor", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? FullEditor { get; set; }

//    [Column("upscalers", TypeName = "jsonb")]
//    public ParameterDefinitionOptions? Upscalers { get; set; }

//    [Column("pan", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Pan { get; set; }

//    [Column("zoom_out", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? ZoomOut { get; set; }

//    [Column("uplight", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Uplight { get; set; }

//    [Column("upbeta", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Upbeta { get; set; }

//    [Column("upanime", TypeName = "jsonb")]
//    public ParameterDefinitionFlag? Upanime { get; set; }

//    public MidjourneyVersion
//    (
//        string version,
//        string versionAbbrev,
//        ParameterDefinitionRange? aspectRatioHeightWidth,
//        ParameterDefinitionFlag? style = null,
//        ParameterDefinitionRange? stylize = null,
//        ParameterDefinitionRange? qualityHdHq = null,
//        ParameterDefinitionModes? relaxFastTurbo = null,
//        ParameterDefinitionRange? chaos = null,
//        ParameterDefinitionRange? weird = null,
//        ParameterDefinitionFlag? raw = null,
//        ParameterDefinitionFlag? tile = null,
//        ParameterDefinitionFlag? no = null,
//        ParameterDefinitionFlag? vibe = null,
//        ParameterDefinitionTestTestp? testTestp = null,
//        ParameterDefinitionFlag? creative = null,
//        ParameterDefinitionFlag? newclip = null,
//        ParameterDefinitionFlag? nostretch = null,
//        ParameterDefinitionFlag? promptWeight = null,
//        ParameterDefinitionFlag? permutation = null,
//        ParameterDefinitionFlag? repeat = null,
//        ParameterDefinitionFlag? seed = null,
//        ParameterDefinitionFlag? sameseed = null,
//        ParameterDefinitionFlag? personalization = null,
//        ParameterDefinitionFlag? cref = null,
//        ParameterDefinitionFlag? sref = null,
//        ParameterDefinitionFlag? oref = null,
//        ParameterDefinitionFlag? imagePrompt = null,
//        ParameterDefinitionRange? imageWeight = null,
//        ParameterDefinitionRange? stop = null,
//        ParameterDefinitionFlag? video = null,
//        ParameterDefinitionFlag? variations = null,
//        ParameterDefinitionFlag? remix = null,
//        ParameterDefinitionFlag? editor = null,
//        ParameterDefinitionFlag? fullEditor = null,
//        ParameterDefinitionOptions? upscalers = null,
//        ParameterDefinitionFlag? pan = null,
//        ParameterDefinitionFlag? zoomOut = null,
//        ParameterDefinitionFlag? uplight = null,
//        ParameterDefinitionFlag? upbeta = null,
//        ParameterDefinitionFlag? upanime = null
//    )
//    {
//        Version = version;
//        VersionAbbrev = versionAbbrev;
//        AspectRatioHeightWidth = aspectRatioHeightWidth;
//        Style = style;
//        Stylize = stylize;
//        QualityHdHq = qualityHdHq;
//        RelaxFastTurbo = relaxFastTurbo;
//        Chaos = chaos;
//        Weird = weird;
//        Raw = raw;
//        Tile = tile;
//        No = no;
//        Vibe = vibe;
//        TestTestp = testTestp;
//        Creative = creative;
//        Newclip = newclip;
//        Nostretch = nostretch;
//        PromptWeight = promptWeight;
//        Permutation = permutation;
//        Repeat = repeat;
//        Seed = seed;
//        Sameseed = sameseed;
//        Personalization = personalization;
//        Cref = cref;
//        Sref = sref;
//        Oref = oref;
//        ImagePrompt = imagePrompt;
//        ImageWeight = imageWeight;
//        Stop = stop;
//        Video = video;
//        Variations = variations;
//        Remix = remix;
//        Editor = editor;
//        FullEditor = fullEditor;
//        Upscalers = upscalers;
//        Pan = pan;
//        ZoomOut = zoomOut;
//        Uplight = uplight;
//        Upbeta = upbeta;
//        Upanime = upanime;
//    }

//    private MidjourneyVersion()
//    {

//    }

//    //public Result<MidjourneyVersion> SetAspectRatioValue(int xAxis, int yAxis)
//    //{
//    //    if (AspectRatioHeightWidth == null)
//    //        return Result<MidjourneyVersion>.Failure("AspectRatio definition is not available for this version.");

//    //    if (xAxis <= 0 || yAxis <= 0)
//    //        return Result<MidjourneyVersion>.Failure("Both xAxis and yAxis must be greater than 0.");

//    //    AspectRatioHeightWidthValue = $"{xAxis}:{yAxis}";
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetStylizeValue(int? stylize)
//    //{
//    //    if (stylize.HasValue)
//    //    {
//    //        if (StylizeDefinition == null)
//    //            return Result<MidjourneyVersion>.Failure("This version does not support the Stylize parameter.");

//    //        if (stylize.Value < StylizeDefinition.Min || stylize.Value > StylizeDefinition.Max)
//    //            return Result<MidjourneyVersion>.Failure($"Stylize must be between {StylizeDefinition.Min} and {StylizeDefinition.Max}.");
//    //    }

//    //    StylizeValue = stylize;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetQualityValue(int? quality)
//    //{
//    //    if (quality.HasValue)
//    //    {
//    //        if (QualityHdHqDefinition != null)
//    //        {
//    //            if (quality.Value < QualityHdHqDefinition.Min || quality.Value > QualityHdHqDefinition.Max)
//    //                return Result<MidjourneyVersion>.Failure($"Quality must be between {QualityHdHqDefinition.Min} and {QualityHdHqDefinition.Max}.");
//    //        }
//    //        else
//    //        {
//    //            var allowed = new HashSet<int> { 1, 2, 4 };
//    //            if (!allowed.Contains(quality.Value))
//    //                return Result<MidjourneyVersion>.Failure("Quality can only be 1, 2, or 4.");
//    //        }
//    //    }

//    //    QualityValue = quality;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetChaosValue(int? chaos)
//    //{
//    //    if (chaos.HasValue)
//    //    {
//    //        if (ChaosDefinition == null)
//    //            return Result<MidjourneyVersion>.Failure("This version does not support the Chaos parameter.");

//    //        if (chaos.Value < ChaosDefinition.Min || chaos.Value > ChaosDefinition.Max)
//    //            return Result<MidjourneyVersion>.Failure($"Chaos must be between {ChaosDefinition.Min} and {ChaosDefinition.Max}.");
//    //    }

//    //    ChaosValue = chaos;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetWeirdValue(int? weird)
//    //{
//    //    if (weird.HasValue)
//    //    {
//    //        if (WeirdDefinition == null)
//    //            return Result<MidjourneyVersion>.Failure("This version does not support the Weird parameter.");

//    //        if (weird.Value < WeirdDefinition.Min || weird.Value > WeirdDefinition.Max)
//    //            return Result<MidjourneyVersion>.Failure($"Weird must be between {WeirdDefinition.Min} and {WeirdDefinition.Max}.");
//    //    }

//    //    WeirdValue = weird;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetSeedValue(int? seed)
//    //{
//    //    if (seed.HasValue)
//    //    {
//    //        if (SeedDefinition == null)
//    //            return Result<MidjourneyVersion>.Failure("This version does not support the Seed parameter.");

//    //        if (seed.Value < SeedDefinition.Min || seed.Value > SeedDefinition.Max)
//    //            return Result<MidjourneyVersion>.Failure($"Seed must be between {SeedDefinition.Min} and {SeedDefinition.Max}.");
//    //    }

//    //    SeedValue = seed;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetImageWeightValue(int? imageWeight)
//    //{
//    //    if (imageWeight.HasValue)
//    //    {
//    //        if (ImageWeightDefinition == null)
//    //            return Result<MidjourneyVersion>.Failure("This version does not support the ImageWeight parameter.");

//    //        if (imageWeight.Value < ImageWeightDefinition.Min || imageWeight.Value > ImageWeightDefinition.Max)
//    //            return Result<MidjourneyVersion>.Failure($"ImageWeight must be between {ImageWeightDefinition.Min} and {ImageWeightDefinition.Max}.");
//    //    }

//    //    ImageWeightValue = imageWeight;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetStopValue(int? stop)
//    //{
//    //    if (stop.HasValue)
//    //    {
//    //        if (StopDefinition == null)
//    //            return Result<MidjourneyVersion>.Failure("This version does not support the Stop parameter.");

//    //        if (stop.Value < StopDefinition.Min || stop.Value > StopDefinition.Max)
//    //            return Result<MidjourneyVersion>.Failure($"Stop must be between {StopDefinition.Min} and {StopDefinition.Max}.");
//    //    }

//    //    StopValue = stop;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetRepeatValue(int? repeat)
//    //{
//    //    if (repeat.HasValue)
//    //    {
//    //        if (RepeatDefinition == null)
//    //            return Result<MidjourneyVersion>.Failure("This version does not support the Repeat parameter.");

//    //        if (repeat.Value < RepeatDefinition.Min || repeat.Value > RepeatDefinition.Max)
//    //            return Result<MidjourneyVersion>.Failure($"Repeat must be between {RepeatDefinition.Min} and {RepeatDefinition.Max}.");
//    //    }

//    //    RepeatValue = repeat;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetModeValue(string? mode)
//    //{
//    //    if (!string.IsNullOrWhiteSpace(mode))
//    //    {
//    //        if (ModeDefinition == null)
//    //            return Result<MidjourneyVersion>.Failure("This version does not support the Mode parameter.");

//    //        if (!ModeDefinition.Modes.ContainsKey(mode))
//    //            return Result<MidjourneyVersion>.Failure($"Invalid mode '{mode}'. Available modes: {string.Join(", ", ModeDefinition.Modes.Keys)}.");
//    //    }

//    //    ModeValue = mode;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetNijiValue(string? niji)
//    //{
//    //    if (!string.IsNullOrWhiteSpace(niji) && !niji.StartsWith("niji"))
//    //        return Result<MidjourneyVersion>.Failure("Niji must start with 'niji'.");

//    //    NijiValue = niji;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetVisibilityModeValue(string? visibilityMode)
//    //{
//    //    if (!string.IsNullOrWhiteSpace(visibilityMode) &&
//    //        visibilityMode is not ("stealth" or "public"))
//    //    {
//    //        return Result<MidjourneyVersion>.Failure("VisibilityMode must be either 'stealth' or 'public'.");
//    //    }

//    //    VisibilityModeValue = visibilityMode;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetVersionValue(string version)
//    //{
//    //    var allowedVersions = new HashSet<string>
//    //{
//    //    "1", "2", "3", "4", "5", "5.1", "5.2", "6", "6.1", "7", "niji 4", "niji 5", "niji 6"
//    //};

//    //    if (string.IsNullOrWhiteSpace(version))
//    //        return Result<MidjourneyVersion>.Failure("Version cannot be empty.");

//    //    if (!allowedVersions.Contains(version))
//    //        return Result<MidjourneyVersion>.Failure(
//    //            $"Invalid version value. Allowed values: {string.Join(", ", allowedVersions)}."
//    //        );

//    //    Version = version;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetStyleReferenceValue(string? styleReference)
//    //{
//    //    StyleReference = styleReference;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetCharacterReferenceValue(string? cref)
//    //{
//    //    CharacterReference = cref;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetNoValue(string? no)
//    //{
//    //    No = no;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}

//    //public Result<MidjourneyVersion> SetProfileValue(string? profile)
//    //{
//    //    Profile = profile;
//    //    return Result<MidjourneyVersion>.Success(this);
//    //}
//}