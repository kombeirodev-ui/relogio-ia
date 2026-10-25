using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Control;

public class MusicState {
    public string Title="",Artist="",Album="",Playlist="",Source="",Error="";
    public bool Available,Playing,CanPrevious,CanToggle,CanNext;
    public bool CanShuffle,CanRepeat;public bool? Shuffle;public int? Repeat;public VolumeState Volume=new VolumeState();
    public double PositionSeconds,DurationSeconds;
    public DateTime At=DateTime.UtcNow;
    public string Description {get{return Available?(String.IsNullOrWhiteSpace(Artist)?Title:Title+" — "+Artist):"Abra o Spotify e escolha uma música";}}
}

// Uses Windows' media sessions, not account cookies or global media key presses.
// Playlist context is not part of SMTC metadata. Album is never called a playlist.
public sealed class SpotifyMedia : IDisposable {
    GlobalSystemMediaTransportControlsSessionManager manager;
    GlobalSystemMediaTransportControlsSession selected;
    bool subscribed;
    readonly SemaphoreSlim gate=new SemaphoreSlim(1,1);
    int dirty=1;bool disposed;
    string title="",artist="",album="";
    DateTime metadataAt=DateTime.MinValue;
    public static bool IsSpotify(string id){return !String.IsNullOrEmpty(id)&&(String.Equals(id,"Spotify.exe",StringComparison.OrdinalIgnoreCase)||id.StartsWith("SpotifyAB.SpotifyMusic_",StringComparison.OrdinalIgnoreCase));}
    public static int NextRepeat(int current){return current==0?2:current==2?1:0;}
    void Dirty(GlobalSystemMediaTransportControlsSession s,MediaPropertiesChangedEventArgs args){Interlocked.Exchange(ref dirty,1);}
    void Unselect(){if(selected!=null){if(subscribed){selected.MediaPropertiesChanged-=Dirty;subscribed=false;}selected=null;}title=artist=album="";metadataAt=DateTime.MinValue;}
    async Task<bool> SelectAsync(){
        if(manager==null)manager=await GlobalSystemMediaTransportControlsSessionManager.RequestAsync().AsTask().ConfigureAwait(false);
        if(disposed)return false;
        var next=manager.GetSessions().FirstOrDefault(s=>IsSpotify(s.SourceAppUserModelId));
        if(next==null){Unselect();return false;}
        if(selected==null||!Object.Equals(next,selected)){Unselect();selected=next;selected.MediaPropertiesChanged+=Dirty;subscribed=true;Interlocked.Exchange(ref dirty,1);}
        return true;
    }
    public async Task<MusicState> ReadAsync(){
        await gate.WaitAsync().ConfigureAwait(false);
        try{
            if(disposed)return new MusicState();
            if(!await SelectAsync().ConfigureAwait(false))return new MusicState();
            if(Interlocked.Exchange(ref dirty,0)!=0||(DateTime.UtcNow-metadataAt).TotalSeconds>=30){var data=await selected.TryGetMediaPropertiesAsync().AsTask().ConfigureAwait(false);title=data.Title??"";artist=data.Artist??"";album=data.AlbumTitle??"";metadataAt=DateTime.UtcNow;}
            var playback=selected.GetPlaybackInfo();var timeline=selected.GetTimelineProperties();var controls=playback.Controls;double position=Math.Max(0,(timeline.Position-timeline.StartTime).TotalSeconds),duration=Math.Max(0,(timeline.EndTime-timeline.StartTime).TotalSeconds);if(playback.PlaybackStatus==GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing){double elapsed=(DateTimeOffset.UtcNow-timeline.LastUpdatedTime).TotalSeconds;if(elapsed>=0&&elapsed<86400)position+=elapsed;}if(duration>0)position=Math.Min(position,duration);
            return new MusicState {Available=true,Title=title,Artist=artist,Album=album,Source=selected.SourceAppUserModelId,Playing=playback.PlaybackStatus==GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing,CanPrevious=controls.IsPreviousEnabled,CanNext=controls.IsNextEnabled,CanToggle=controls.IsPlayPauseToggleEnabled,CanShuffle=controls.IsShuffleEnabled,CanRepeat=controls.IsRepeatEnabled,Shuffle=playback.IsShuffleActive,Repeat=playback.AutoRepeatMode.HasValue?(int?)playback.AutoRepeatMode.Value:null,PositionSeconds=position,DurationSeconds=duration};
        }catch(Exception e){Unselect();manager=null;return new MusicState {Error=e is UnauthorizedAccessException?"O Windows não permitiu acessar a mídia":"Não foi possível ler o Spotify agora"};}
        finally{gate.Release();}
    }
    public async Task<bool> CommandAsync(string command){
        await gate.WaitAsync().ConfigureAwait(false);
        try{
            if(disposed||!await SelectAsync().ConfigureAwait(false))return false;
            bool result=false;
            if(command=="previous")result=await selected.TrySkipPreviousAsync().AsTask().ConfigureAwait(false);
            else if(command=="next")result=await selected.TrySkipNextAsync().AsTask().ConfigureAwait(false);
            else if(command=="toggle")result=await selected.TryTogglePlayPauseAsync().AsTask().ConfigureAwait(false);
            else if(command=="pause")result=await selected.TryPauseAsync().AsTask().ConfigureAwait(false);
            else if(command=="shuffle"){var p=selected.GetPlaybackInfo();if(!p.Controls.IsShuffleEnabled||!p.IsShuffleActive.HasValue)return false;result=await selected.TryChangeShuffleActiveAsync(!p.IsShuffleActive.Value).AsTask().ConfigureAwait(false);}
            else if(command=="repeat"){var p=selected.GetPlaybackInfo();if(!p.Controls.IsRepeatEnabled||!p.AutoRepeatMode.HasValue)return false;result=await selected.TryChangeAutoRepeatModeAsync((Windows.Media.MediaPlaybackAutoRepeatMode)NextRepeat((int)p.AutoRepeatMode.Value)).AsTask().ConfigureAwait(false);}
            Interlocked.Exchange(ref dirty,1);return result;
        }catch{return false;}finally{gate.Release();}
    }
    public void Dispose(){disposed=true;/* Any native request in flight releases its own gate. */if(gate.Wait(0)){try{Unselect();manager=null;}finally{gate.Release();}}}
}
