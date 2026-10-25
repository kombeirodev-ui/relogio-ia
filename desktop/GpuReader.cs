using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// One native PDH query collects every GPU engine at once. No per-process managed
// PerformanceCounter objects, periodic discovery, or arbitrary instance limit.
public sealed class GpuReader : IDisposable {
    IntPtr query,counter;bool primed;
    [StructLayout(LayoutKind.Sequential)]struct Value {public uint Status;public double Number;}
    [StructLayout(LayoutKind.Sequential)]struct Item {public IntPtr Name;public Value Value;}
    [DllImport("pdh.dll",CharSet=CharSet.Unicode)]static extern uint PdhOpenQuery(string source,IntPtr user,out IntPtr query);
    [DllImport("pdh.dll",CharSet=CharSet.Unicode)]static extern uint PdhAddEnglishCounter(IntPtr query,string path,IntPtr user,out IntPtr counter);
    [DllImport("pdh.dll")]static extern uint PdhCollectQueryData(IntPtr query);
    [DllImport("pdh.dll",CharSet=CharSet.Unicode)]static extern uint PdhGetFormattedCounterArray(IntPtr counter,uint format,ref uint size,out uint count,IntPtr items);
    [DllImport("pdh.dll")]static extern uint PdhCloseQuery(IntPtr query);
    public string Note="Aguardando segunda amostra";
    public static string EngineKey(string name){if(String.IsNullOrEmpty(name))return "";int index=name.IndexOf("_luid_",StringComparison.Ordinal);return index>=0?name.Substring(index+1):name;}
    public double? Read(){
        try{
            if(query==IntPtr.Zero){if(PdhOpenQuery(null,IntPtr.Zero,out query)!=0||PdhAddEnglishCounter(query,@"\GPU Engine(*)\Utilization Percentage",IntPtr.Zero,out counter)!=0){Dispose();Note="Contadores de GPU indisponíveis neste Windows/driver";return null;}}
            if(PdhCollectQueryData(query)!=0){Note="O Windows não forneceu leitura de GPU";return null;}
            if(!primed){primed=true;Note="Aguardando segunda amostra";return null;}
            uint size=0,count;uint status=PdhGetFormattedCounterArray(counter,0x200|0x8000,ref size,out count,IntPtr.Zero);
            if(size==0||size>64000000){Note="Nenhum motor de GPU disponível";return null;}
            var buffer=Marshal.AllocHGlobal((int)size);
            try{
                status=PdhGetFormattedCounterArray(counter,0x200|0x8000,ref size,out count,buffer);
                if(status!=0){Note="Leitura de GPU mudou; aguardando próxima amostra";return null;}
                var groups=new Dictionary<string,double>();int step=Marshal.SizeOf(typeof(Item));
                for(uint i=0;i<count;i++){var item=(Item)Marshal.PtrToStructure(IntPtr.Add(buffer,(int)i*step),typeof(Item));if(item.Value.Status>1||Double.IsNaN(item.Value.Number)||Double.IsInfinity(item.Value.Number)||item.Value.Number<0)continue;string key=EngineKey(Marshal.PtrToStringUni(item.Name));double value;groups.TryGetValue(key,out value);groups[key]=value+item.Value.Number;}
                double maximum=0;foreach(double value in groups.Values)maximum=Math.Max(maximum,value);
                Note=groups.Count==0?"Aguardando amostra válida da GPU":"Motor de GPU mais ocupado (Windows · coleta agrupada)";return groups.Count==0?(double?)null:Math.Min(100,maximum);
            }finally{Marshal.FreeHGlobal(buffer);}
        }catch{Note="Contadores de GPU indisponíveis neste Windows/driver";Dispose();return null;}
    }
    public void Dispose(){if(query!=IntPtr.Zero)PdhCloseQuery(query);query=counter=IntPtr.Zero;primed=false;}
}
