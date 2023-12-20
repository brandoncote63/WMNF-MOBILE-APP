#import "NativeMediaPlayer.h"
#import "RemoteCommandCenter.h"
#import <AudioToolbox/AudioToolbox.h>
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

void _prepare(int _id, const char* _listener, bool _playWhenReady, const char* _playlistJson)  {
    [[NativeMediaPlayer sharedObject] prepare:(int)_id listenerName:GetStringParam(_listener) playWhenReady:(bool)_playWhenReady playlistJson:GetStringParam(_playlistJson)];
}

void _preparePlaylist(int _id, bool _playWhenReady, const char* _playlistJson)  {
    [[NativeMediaPlayer sharedObject] preparePlaylist:(int)_id playWhenReady:(bool)_playWhenReady playlistJson:GetStringParam(_playlistJson)];
}

void _prepareMediaItem(int _id, bool _playWhenReady)  {
    [[NativeMediaPlayer sharedObject] prepareMediaItem:(int)_id playWhenReady:(bool)_playWhenReady];
}

void _reload() {
    [[NativeMediaPlayer sharedObject] reload];
}

void _play() {
	[[NativeMediaPlayer sharedObject] play];
}

void _pause() {
    [[NativeMediaPlayer sharedObject] pause];
}

void _stop() {
    [[NativeMediaPlayer sharedObject] stop];
}

void _frelease() {
    [[NativeMediaPlayer sharedObject] frelease];
}

void _seekForward() {
    [[NativeMediaPlayer sharedObject] seekForward];
}

void _seekBackward() {
    [[NativeMediaPlayer sharedObject] seekBackward];
}

void _nextTrack() {
    [[NativeMediaPlayer sharedObject] nextTrack];
}

void _previousTrack() {
    [[NativeMediaPlayer sharedObject] previousTrack];
}

void _seekTo(float _position) {
    [[NativeMediaPlayer sharedObject] seekTo:(float)_position];
}

float _getVolume() {
    return [[NativeMediaPlayer sharedObject] getVolume];
}

void _setVolume(float _volume) {
    [[NativeMediaPlayer sharedObject] setVolume:(float)_volume];
}

int _getRepeatMode() {
    return [[NativeMediaPlayer sharedObject] getRepeatMode];
}

void _setRepeatMode(int _repeatMode) {
    [[NativeMediaPlayer sharedObject] setRepeatMode:(int)_repeatMode];
}

bool _getShuffleModeEnabled() {
    return [[NativeMediaPlayer sharedObject] getShuffleModeEnabled];
}

void _setShuffleModeEnabled(bool _shuffleModeEnabled) {
    [[NativeMediaPlayer sharedObject] setShuffleModeEnabled:(bool)_shuffleModeEnabled];
}

int _getShuffleOrder(int _id) {
    return [[NativeMediaPlayer sharedObject] getShuffleOrder:(int)_id];
}

float _getCurrentPosition() {
    return [[NativeMediaPlayer sharedObject] getCurrentPosition];
}

float _getDuration() {
    return [[NativeMediaPlayer sharedObject] getDuration];
}

int _getCurrentMediaItemIndex() {
    return [[NativeMediaPlayer sharedObject] getCurrentMediaItemIndex];
}

bool _isPlaying() {
	return [[NativeMediaPlayer sharedObject] isPlaying];
}

bool _isLoading() {
    return [[NativeMediaPlayer sharedObject] isLoading];
}

bool _hasPreviousMediaItem() {
    return [[NativeMediaPlayer sharedObject] hasPreviousMediaItem];
}

bool _hasNextMediaItem() {
    return [[NativeMediaPlayer sharedObject] hasNextMediaItem];
}

// retrieving metadata
char* _retrieveAlbumTitle(int _id) {
    return [[NativeMediaPlayer sharedObject] retrieveAlbumTitle:(int)_id];
}

char* _retrieveAlbumArtist(int _id) {
    return [[NativeMediaPlayer sharedObject] retrieveAlbumArtist:(int)_id];
}

char* _retrieveTitle(int _id) {
    return [[NativeMediaPlayer sharedObject] retrieveTitle:(int)_id];
}

char* _retrieveArtist(int _id) {
    return [[NativeMediaPlayer sharedObject] retrieveArtist:(int)_id];
}

char* _retrieveGenre(int _id) {
    return [[NativeMediaPlayer sharedObject] retrieveGenre:(int)_id];
}

char* _retrieveReleaseDate(int _id) {
    return [[NativeMediaPlayer sharedObject] retrieveReleaseDate:(int)_id];
}

char* _retrieveArtwork(int _id) {
    return [[NativeMediaPlayer sharedObject] retrieveArtwork:(int)_id];
}

void _addMediaItem(int _id, const char* _json) {
    [[NativeMediaPlayer sharedObject] addMediaItem:(int)_id json:GetStringParam(_json)];
}

void _removeMediaItem(int _id) {
    [[NativeMediaPlayer sharedObject] removeMediaItem:(int)_id];
}

// extra

void _useRemoteCommands(bool _active) {
	[[NativeMediaPlayer sharedObject] useRemoteCommands:(bool)_active];
}

@interface NativeMediaPlayer()
- (void) resetAudioSession;
- (bool) isLocal;
- (bool) isRemote;
- (char*) convertNSStringToUnityString:(NSString*)nsString;
- (MediaItem*) getMediaItem:(int)_id;
- (MediaItem*) getCurrentMediaItem;
@end

@implementation NativeMediaPlayer
@synthesize localPlayer;
@synthesize remotePlayer;

+ (NativeMediaPlayer*) sharedObject {
	static NativeMediaPlayer *sharedObject = nil;
	if (!sharedObject) {
		sharedObject = [[NativeMediaPlayer alloc] init];
    }
    return sharedObject;
}

- (id) init {
	if(self = [super init]) {
        currentMediaLocation = -1;
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(onAudioSessionEvent:)
                                                     name:AVAudioSessionInterruptionNotification
                                                   object:nil];
	}
	return self;
}

- (void) resetAudioSession {
	self.localPlayer = nil;
    self.remotePlayer = nil;
    
    [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayback
                                     withOptions:AVAudioSessionCategoryOptionAllowAirPlay
                                           error:nil];
    [[AVAudioSession sharedInstance] setMode:AVAudioSessionModeDefault error:nil];
	[[AVAudioSession sharedInstance] setActive:YES withOptions:0 error:nil];
}

- (void) reload {
    if ([self isLocal]) [self.localPlayer prepareMediaItem:self.localPlayer.getCurrentMediaItemIndex playWhenReady:self.localPlayer.getPlayWhenReady];
    else if ([self isRemote]) [self.remotePlayer prepareMediaItem:self.remotePlayer.getCurrentMediaItemIndex playWhenReady:self.remotePlayer.getPlayWhenReady];
}

- (void) prepare:(int)_id listenerName:(NSString*)_listener playWhenReady:(bool)_playWhenReady playlistJson:(NSString *)_playlistJson {
    
    [self frelease];
    
    if (_playlistJson == nil) {
        NSLog(@"Playlist is null");
        return;
    }
    
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        NSLog(@"%@", _playlistJson);
        NSData* jsonData = [_playlistJson dataUsingEncoding:NSUTF8StringEncoding];
        NSError *error = nil;
        NSDictionary *playlistDict = [NSJSONSerialization
                                      JSONObjectWithData:jsonData
                                      options:0
                                      error:&error];
    
        if(!error) {
            NSNumber* uriPathType = [playlistDict objectForKey:@"uriPathType"];
            self->currentMediaLocation = [uriPathType intValue];
            NSMutableArray *mediaItemJson = [playlistDict objectForKey:@"mediaMetadataJson"];
            if (mediaItemJson != nil) {
                NSMutableArray *_mediaItems = [[NSMutableArray<MediaItem*> alloc] init];
                for (int i = 0; i < mediaItemJson.count; ++i) {
                    [_mediaItems addObject:[[MediaItem alloc] initWithJson:i
                                                                     count:(int)mediaItemJson.count
                                                                  pathType:self->currentMediaLocation
                                                                      json:[mediaItemJson objectAtIndex:i]]];
                }
                
                if (self->listener != _listener) self->listener = _listener;

                if (self->currentMediaLocation == STREAMING_ASSET) {
                    self.localPlayer = [[AVLocalPlayer alloc] initWithMediaItems:_mediaItems
                                                             startingItemId:_id
                                                          unityListenerName:_listener
                                                              playWhenReady:_playWhenReady
                                                                 isDownload:false];
                    
                } else if (self->currentMediaLocation == REMOTE_URL) {
                    self.remotePlayer = [[AVRemotePlayer alloc] initWithMediaItems:_mediaItems
                                                               startingItemId:_id
                                                            unityListenerName:_listener
                                                                playWhenReady:_playWhenReady];
                                                                
                } else if (self->currentMediaLocation == DOWNLOAD_AND_PLAY) {
                    self.localPlayer = [[AVLocalPlayer alloc] initWithMediaItems:_mediaItems
															 startingItemId:_id
														  unityListenerName:_listener
															  playWhenReady:_playWhenReady
                                                                 isDownload:true];
                }
            }
        } else {
            NSLog(@"Error in parsing JSON");
        }
    });
}

- (void) preparePlaylist:(int)_id playWhenReady:(bool)_playWhenReady playlistJson:(NSString *)_playlistJson {
    NSLog(@"New playlist found. Reinitiating player...");
    [self prepare:_id listenerName:listener playWhenReady:_playWhenReady playlistJson:_playlistJson];
}

- (void) prepareMediaItem:(int)_id playWhenReady:(bool)_playWhenReady {
    if ([self isLocal]) [self.localPlayer prepareMediaItem:_id playWhenReady:_playWhenReady];
    else if ([self isRemote]) [self.remotePlayer prepareMediaItem:_id playWhenReady:_playWhenReady];
}

- (void) togglePlayPause {
    if ([self isPlaying]) {
        [self pause];
    } else {
        [self play];
    }
}

- (void) play {
    if ([self isLocal]) [self.localPlayer play];
    else if ([self isRemote]) [self.remotePlayer play];
}

- (void) pause {
    if ([self isLocal]) [self.localPlayer pause];
    else if ([self isRemote]) [self.remotePlayer pause];
}

- (void) stop {
    if ([self isLocal]) [self.localPlayer stop];
    else if ([self isRemote]) [self.remotePlayer stop];
}

- (void) frelease {
    if ([self isPlaying]) [self stop];
    
    [self useRemoteCommands:false];
    [self resetAudioSession];
    [self resetMediaPath];
    
    if (self.localPlayer != nil) self.localPlayer = nil;
    if (self.remotePlayer != nil) self.remotePlayer = nil;
}

- (void) previousTrack {
    if ([self isLocal]) [self.localPlayer seekToPrevious];
    else if ([self isRemote]) [self.remotePlayer seekToPrevious];
}

- (void) nextTrack {
    if ([self isLocal]) [self.localPlayer seekToNext];
    else if ([self isRemote]) [self.remotePlayer seekToNext];
}

- (void) seekBackward {
    if ([self isLocal]) [self.localPlayer seekBack];
    else if ([self isRemote]) [self.remotePlayer seekBack];
}

- (void) seekForward {
    if ([self isLocal]) [self.localPlayer seekForward];
    else if ([self isRemote]) [self.remotePlayer seekForward];
}

- (void) seekTo:(float)_position {
    if ([self isLocal]) [self.localPlayer seekTo:_position];
    else if ([self isRemote]) [self.remotePlayer seekTo:_position];
}

/* Get Set */

- (float) getVolume {
    if ([self isLocal]) return [self.localPlayer getVolume];
    else if ([self isRemote]) return [self.remotePlayer getVolume];
    return 0;
}

- (void) setVolume:(float)_volume {
    if ([self isLocal]) [self.localPlayer setVolume:_volume];
    else if ([self isRemote]) [self.remotePlayer setVolume:_volume];
}

- (int) getRepeatMode {
    if ([self isLocal]) return [self.localPlayer getRepeatMode];
    else if ([self isRemote]) return [self.remotePlayer getRepeatMode];
    return 0;
}

- (void) setRepeatMode:(int)_repeatMode{   
    if ([self isLocal]) [self.localPlayer setRepeatMode:_repeatMode];
    else if ([self isRemote]) [self.remotePlayer setRepeatMode:_repeatMode];
}

- (bool) getShuffleModeEnabled {
    if ([self isLocal]) return [self.localPlayer getShuffleModeEnabled];
    else if ([self isRemote]) return [self.remotePlayer getShuffleModeEnabled];
    return false;
}

- (void) setShuffleModeEnabled:(bool)_shuffleModeEnabled {
    if ([self isLocal]) [self.localPlayer setShuffleModeEnabled:_shuffleModeEnabled];
    else if ([self isRemote]) [self.remotePlayer setShuffleModeEnabled:_shuffleModeEnabled];
}

- (long) getCurrentPosition {
    if ([self isLocal]) position = [self.localPlayer getCurrentPosition];
    else if ([self isRemote]) position = [self.remotePlayer getCurrentPosition];
    return position;
}

- (long) getDuration {
    if ([self isLocal]) duration = [self.localPlayer getDuration];
    else if ([self isRemote]) duration = [self.remotePlayer getDuration];
    return duration;
}

- (int) getCurrentMediaItemIndex {
    if ([self isLocal]) return [self.localPlayer getCurrentMediaItemIndex];
    else if ([self isRemote]) return [self.remotePlayer getCurrentMediaItemIndex];
    else return 0;
}

- (int) getShuffleOrder:(int)_id {
    if ([self isLocal]) return (int)[[[self.localPlayer getShuffleOrder] objectAtIndex:_id] integerValue];
    else if ([self isRemote]) return (int)[[[self.remotePlayer getShuffleOrder] objectAtIndex:_id] integerValue];
    else return 0;
}

- (bool) isPlaying {
    if ([self isLocal]) return [self.localPlayer isPlaying];
    else if ([self isRemote]) return [self.remotePlayer isPlaying];
    return false;
}

- (bool) isLoading {
    if ([self isLocal]) return [self.localPlayer isLoading];
    else if ([self isRemote]) return [self.remotePlayer isLoading];
    return false;
}

- (bool) hasPreviousMediaItem {
    if ([self isLocal]) return [self.localPlayer hasPreviousMediaItem];
    else if ([self isRemote]) return [self.remotePlayer hasPreviousMediaItem];
    return false; 
}

- (bool) hasNextMediaItem {
    if ([self isLocal]) return [self.localPlayer hasNextMediaItem];
    else if ([self isRemote]) return [self.remotePlayer hasNextMediaItem];
    return false;
}

- (bool) isLocal {
    if ((currentMediaLocation == STREAMING_ASSET || currentMediaLocation == DOWNLOAD_AND_PLAY) && self.localPlayer != nil) {
        return true;
    } else if (self.localPlayer == nil && self.remotePlayer == nil) {
        //NSLog(@"Couldn't find an active player");
    }
    return false;
}

- (bool) isRemote {
    if (currentMediaLocation == REMOTE_URL && self.remotePlayer != nil) {
        return true;
    } else if (self.localPlayer == nil && self.remotePlayer == nil) {
        NSLog(@"Couldn't find an active player");
    }
    return false;
}

- (void) resetMediaPath {
    currentMediaLocation = [[PlayerPrefs sharedObject] getInt:@"URI_TYPE"];
    NSLog(@"MediaPath reset to : %d", currentMediaLocation);
}

- (MediaItem*) getCurrentMediaItem {
    if ([self isLocal]) return [self.localPlayer getCurrentMediaItem];
    else if ([self isRemote]) return [self.remotePlayer getCurrentMediaItem];
    return nil;
}

- (UnityListener*) getUnityListener {
    if (currentMediaLocation == STREAMING_ASSET || currentMediaLocation == DOWNLOAD_AND_PLAY) {
        return [self.localPlayer getUnityListener];
    } else {
        return [self.remotePlayer getUnityListener];
    }
}

/* Interruption Check (ex) Phone calls... */
- (void) onAudioSessionEvent:(NSNotification*)notification {
    if ([notification.name isEqualToString:AVAudioSessionInterruptionNotification]) {
        NSLog(@"NativeMediaPlayer:Interruption notification received!");
        //Check to see if it was a Begin interruption
        if ([[notification.userInfo valueForKey:AVAudioSessionInterruptionTypeKey] isEqualToNumber:[NSNumber numberWithInt:AVAudioSessionInterruptionTypeBegan]]) {
            NSLog(@"NativeMediaPlayer:Interruption began!");
            [self pause];
        } else {
            NSLog(@"NativeMediaPlayer:Interruption ended!");
            [self play];
        }
    }
}

/* Metadata Retrieving */
- (char*) retrieveAlbumTitle:(int)_id  {
    return [self convertNSStringToUnityString:[self getMediaItem:_id].AlbumTitle];
}

- (char*) retrieveAlbumArtist:(int)_id  {
    return [self convertNSStringToUnityString:[self getMediaItem:_id].AlbumArtist];
}

- (char*) retrieveTitle:(int)_id  {
    return [self convertNSStringToUnityString:[self getMediaItem:_id].Title];
}

- (char*) retrieveArtist:(int)_id  {
    return [self convertNSStringToUnityString:[self getMediaItem:_id].Artist];
}

- (char*) retrieveGenre:(int)_id {
    return [self convertNSStringToUnityString:[self getMediaItem:_id].Genre];
}

- (char*) retrieveReleaseDate:(int)_id  {
    return [self convertNSStringToUnityString:[self getMediaItem:_id].ReleaseDate];
}

- (char*) retrieveArtwork:(int)_id {
    if ([self getMediaItem:_id].ArtData == nil) return nil;
    NSString *encodedString = [NSString stringWithUTF8String:[[self getMediaItem:_id].ArtData bytes]];
    return [self convertNSStringToUnityString:encodedString];
}

- (MediaItem*) getMediaItem:(int)_id {
    if ([self isLocal]) return [self.localPlayer getMediaItem:_id];
    else if ([self isRemote]) return [self.remotePlayer getMediaItem:_id];
    return nil;
}

- (char*) convertNSStringToUnityString:(NSString*)nsString {
    if (nsString == nil) {
        return nil;
    }
    const char* nsStringUtf8 = [nsString UTF8String];
    //create a null terminated C string on the heap so that our string's memory isn't wiped out right after method's return
    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);
    return cString;
}

- (void) addMediaItem:(int)_id json:_json{
    if ([self isLocal]) [self.localPlayer addMediaItem:_id json:_json];
    else if ([self isRemote]) [self.remotePlayer addMediaItem:_id json:_json];
}

- (void) removeMediaItem:(int)_id {
    if ([self isLocal]) return [self.localPlayer removeMediaItem:_id];
    else if ([self isRemote]) [self.remotePlayer removeMediaItem:_id];
}

- (void) useRemoteCommands:(bool)_active {
    [[RemoteCommandCenter sharedObject] setRemoteCommandCenter:_active];
}

@end
