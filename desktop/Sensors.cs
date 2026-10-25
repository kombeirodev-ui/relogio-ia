using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

public class HardwareSample {
    public Dictionary<string,double?> Values=new Dictionary<string,double?>();
    public string DiskName="Disco",GpuNote="Aguardando leitura",Error="";
    public DateTime At=DateTime.UtcNow;
}
public class Sensors : IDisposable {
    readonly object gate=new object();
    [StructLayout(LayoutKind.Sequential)] struct Times {public uint Low,High;public ulong Value {get{return ((ulong)High<<32)|Low;}}}
    [StructLayout(LayoutKind.Sequential)] class Memory {public uint Length=(uint)Marshal.SizeOf(typeof(Memory));public uint Load;public ulong Total,Available,PageTotal,PageAvailable,VirtualTotal,VirtualAvailable,Extended;}
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool GetSystemTimes(out Times idle,out Times kernel,out Times user);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool GlobalMemoryStatusEx([In,Out]Memory memory);
    ulong lastIdle,lastKernel,lastUser;bool hasCpu;
    readonly GpuReader gpu=new GpuReader();
    public HardwareSample Sample(string[] selected) {lock(gate)return SampleInternal(selected);}
    HardwareSample SampleInternal(string[] selected) {
        var sample=new HardwareSample();
        foreach(var key in selected)sample.Values[key]=null;
        if(selected.Contains("cpu"))try{Times i,k,u;if(GetSystemTimes(out i,out k,out u)){if(hasCpu && i.Value>=lastIdle && k.Value>=lastKernel && u.Value>=lastUser)sample.Values["cpu"]=SensorMath.Cpu(i.Value-lastIdle,k.Value-lastKernel,u.Value-lastUser);lastIdle=i.Value;lastKernel=k.Value;lastUser=u.Value;hasCpu=true;}}catch{}
        if(selected.Contains("ram"))try{var m=new Memory();if(GlobalMemoryStatusEx(m)&&m.Total>0)sample.Values["ram"]=100d*(m.Total-m.Available)/m.Total;}catch{}
        if(selected.Contains("disk"))try{var drive=new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory));sample.DiskName="Disco "+drive.Name.TrimEnd('\\');if(drive.IsReady&&drive.TotalSize>0)sample.Values["disk"]=100d*(drive.TotalSize-drive.TotalFreeSpace)/drive.TotalSize;}catch{}
        if(selected.Contains("gpu")){sample.Values["gpu"]=gpu.Read();sample.GpuNote=gpu.Note;}else gpu.Dispose();
        return sample;
    }
    public void Dispose(){lock(gate)gpu.Dispose();}
}
