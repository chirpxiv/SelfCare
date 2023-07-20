using System;

namespace SelfCare.Core; 

public abstract class ServiceBase : IDisposable {
	public abstract void Init(SelfCare plugin);
	public abstract void Dispose();
}