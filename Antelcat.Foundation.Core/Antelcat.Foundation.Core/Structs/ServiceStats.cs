﻿using Antelcat.Foundation.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Antelcat.Foundation.Core.Structs;

public class ServiceStats
{
    public ServiceStats(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 缓存的服务生命周期
    /// </summary>
    public Dictionary<Type, ServiceLifetime>? ServiceLifetimes { get; set; }
    
    /// <summary>
    /// 缓存的实现类的属性字段映射器
    /// </summary>
    public Dictionary<Type, ServiceStat> CachedMappers { get; private init; }= new();
    
    /// <summary>
    /// 解析过的单例
    /// </summary>
    public HashSet<Type> ResolvedSingletons { get; private init; } = new();
    
    /// <summary>
    /// 解析过的会话
    /// </summary>
    public HashSet<Type> ResolvedScopes { get; } = new();

    /// <summary>
    /// 是否需要被解析
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool NoNeedAutowired(Type type) => 
        ResolvedSingletons.Contains(type) 
        || ResolvedScopes.Contains(type)
        || CachedMappers.TryGetValue(type,out var r) && !r.NeedAutowired;
    
    /// <summary>
    /// 创建一个Scope
    /// </summary>
    /// <returns></returns>
    public ServiceStats CreateScope()
    {
        return new ServiceStats(ServiceProvider)
        {
            CachedMappers = CachedMappers,
            ResolvedSingletons = ResolvedSingletons,
            ServiceLifetimes = ServiceLifetimes
        };
    }
}

public struct ServiceStat
{
    public bool NeedAutowired { get; init; }
    public List<Tuple<Type, Setter<object, object>>>? Mappers;
}