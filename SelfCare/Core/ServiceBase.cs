using System;

namespace SelfCare.Core; 

public abstract class ServiceBase : IDisposable {
	public abstract void Init();
	public abstract void Dispose();
}