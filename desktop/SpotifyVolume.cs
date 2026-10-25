using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class VolumeState {public bool Available,Muted;public int Percent;}

// Core Audio per-application sessions. Never changes the endpoint/master volume.
public sealed class SpotifyVolume : IDisposable {
    readonly object gate=new object();readonly List<object> sessions=new List<object>();DateTime scanned=DateTime.MinValue;
    static readonly Guid managerId=new Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F");
    public static int Clamp(int value){return Math.Max(0,Math.Min(100,value));}
    public static bool IsSpotifyProcess(string name){return String.Equals(name,"Spotify",StringComparison.OrdinalIgnoreCase);}
    static void Release(object value){if(value!=null&&Marshal.IsComObject(value))Marshal.ReleaseComObject(value);}
    void Clear(){foreach(var value in sessions)Release(value);sessions.Clear();}
    void Discover(){
        if((DateTime.UtcNow-scanned).TotalSeconds<(sessions.Count>0?15:3))return;Clear();scanned=DateTime.UtcNow;object enumerator=null;IMMDeviceCollection devices=null;
        try{enumerator=new DeviceEnumerator();var api=(IMMDeviceEnumerator)enumerator;if(api.EnumAudioEndpoints(0,1,out devices)!=0)return;uint count;devices.GetCount(out count);
            for(uint d=0;d<count;d++){IMMDevice device=null;object manager=null;IAudioSessionEnumerator list=null;try{if(devices.Item(d,out device)!=0)continue;Guid id=managerId;if(device.Activate(ref id,23,IntPtr.Zero,out manager)!=0)continue;if(((IAudioSessionManager2)manager).GetSessionEnumerator(out list)!=0)continue;int n;list.GetCount(out n);for(int i=0;i<n;i++){object session=null;bool keep=false;try{if(list.GetSession(i,out session)!=0)continue;var control=(IAudioSessionControl2)session;uint pid;if(control.GetProcessId(out pid)!=0||pid==0)continue;using(var p=Process.GetProcessById((int)pid))if(IsSpotifyProcess(p.ProcessName)){var volume=(ISimpleAudioVolume)session;float level;if(volume.GetMasterVolume(out level)==0){sessions.Add(session);keep=true;}}}catch{}finally{if(!keep)Release(session);}}}finally{Release(list);Release(manager);Release(device);}}
        }finally{Release(devices);Release(enumerator);}
    }
    public VolumeState Read(){lock(gate){try{Discover();foreach(var session in sessions){float level;bool muted;var volume=(ISimpleAudioVolume)session;if(volume.GetMasterVolume(out level)==0&&volume.GetMute(out muted)==0)return new VolumeState {Available=true,Percent=Clamp((int)Math.Round(level*100)),Muted=muted};}}catch{Clear();scanned=DateTime.MinValue;}return new VolumeState();}}
    public bool SetPercent(int value){lock(gate){try{Discover();bool done=false;Guid context=Guid.Empty;foreach(var session in sessions)if(((ISimpleAudioVolume)session).SetMasterVolume(Clamp(value)/100f,ref context)==0)done=true;return done;}catch{Clear();scanned=DateTime.MinValue;return false;}}}
    public bool SetMuted(bool muted){lock(gate){try{Discover();bool done=false;Guid context=Guid.Empty;foreach(var session in sessions)if(((ISimpleAudioVolume)session).SetMute(muted,ref context)==0)done=true;return done;}catch{Clear();scanned=DateTime.MinValue;return false;}}}
    public void Dispose(){lock(gate){Clear();scanned=DateTime.MinValue;}}

    [ComImport,Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]class DeviceEnumerator{}
    [ComImport,Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]interface IMMDeviceEnumerator {[PreserveSig]int EnumAudioEndpoints(int flow,uint mask,out IMMDeviceCollection devices);}
    [ComImport,Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]interface IMMDeviceCollection {[PreserveSig]int GetCount(out uint count);[PreserveSig]int Item(uint index,out IMMDevice device);}
    [ComImport,Guid("D666063F-1587-4E43-81F1-B948E807363F"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]interface IMMDevice {[PreserveSig]int Activate(ref Guid id,uint context,IntPtr parameters,[MarshalAs(UnmanagedType.IUnknown)]out object value);}
    [ComImport,Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]interface IAudioSessionManager2 {[PreserveSig]int GetAudioSessionControl(ref Guid id,uint flags,out IntPtr control);[PreserveSig]int GetSimpleAudioVolume(ref Guid id,uint flags,out IntPtr volume);[PreserveSig]int GetSessionEnumerator(out IAudioSessionEnumerator sessions);}
    [ComImport,Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]interface IAudioSessionEnumerator {[PreserveSig]int GetCount(out int count);[PreserveSig]int GetSession(int index,[MarshalAs(UnmanagedType.IUnknown)]out object session);}
    [ComImport,Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]interface IAudioSessionControl2 {
        [PreserveSig]int GetState(out int state);[PreserveSig]int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)]out string value);[PreserveSig]int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)]string value,ref Guid context);[PreserveSig]int GetIconPath([MarshalAs(UnmanagedType.LPWStr)]out string value);[PreserveSig]int SetIconPath([MarshalAs(UnmanagedType.LPWStr)]string value,ref Guid context);[PreserveSig]int GetGroupingParam(out Guid group);[PreserveSig]int SetGroupingParam(ref Guid group,ref Guid context);[PreserveSig]int RegisterAudioSessionNotification(IntPtr callback);[PreserveSig]int UnregisterAudioSessionNotification(IntPtr callback);
        [PreserveSig]int GetSessionIdentifier([MarshalAs(UnmanagedType.LPWStr)]out string value);[PreserveSig]int GetSessionInstanceIdentifier([MarshalAs(UnmanagedType.LPWStr)]out string value);[PreserveSig]int GetProcessId(out uint pid);
    }
    [ComImport,Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]interface ISimpleAudioVolume {[PreserveSig]int SetMasterVolume(float value,ref Guid context);[PreserveSig]int GetMasterVolume(out float value);[PreserveSig]int SetMute([MarshalAs(UnmanagedType.Bool)]bool muted,ref Guid context);[PreserveSig]int GetMute([MarshalAs(UnmanagedType.Bool)]out bool muted);}
}
