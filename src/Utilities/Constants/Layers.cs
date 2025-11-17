namespace Utilities.Constants;

public sealed class DomainLayer : ILayer { }

public sealed class ApplicationLayer : ILayer { }

public sealed class PersistenceLayer : ILayer { }

public sealed class InfrastructureLayer : ILayer { }

public sealed class PresentationLayer : ILayer { }

public sealed class AppLayer : ILayer { }

public sealed class UtilitiesLayer : ILayer { }

public sealed class UnknownLayer : ILayer { }

public interface ILayer { }