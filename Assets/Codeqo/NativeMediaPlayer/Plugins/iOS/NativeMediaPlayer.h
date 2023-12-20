#import <AVFoundation/AVFoundation.h>
#import <Foundation/Foundation.h>
#import <MediaPlayer/MPMediaItem.h>
#import <MediaPlayer/MediaPlayer.h>
#import "AVLocalPlayer.h"
#import "AVRemotePlayer.h"

@interface NativeMediaPlayer : NSObject
{
	AVLocalPlayer*	localPlayer;    
    AVRemotePlayer* remotePlayer;
    NSString*       listener;
    int             currentMediaLocation;
    long            duration;
    long            position;
}

+ (NativeMediaPlayer*) sharedObject;

// core methods
- (void) prepare: (int)_id
    listenerName: (NSString*)_listener
   playWhenReady: (bool)_playWhenReady
   playlistJson : (NSString*)_playlistJson;
- (void) preparePlaylist: (int)_id
            playWhenReady: (bool)_playWhenReady
            playlistJson : (NSString*)_playlistJson;
- (void) prepareMediaItem: (int)_id
            playWhenReady: (bool)_playWhenReady;

// basic player methods
- (void) togglePlayPause;
- (void) reload;
- (void) play;
- (void) stop;
- (void) frelease;
- (void) pause;
- (void) previousTrack;
- (void) nextTrack;
- (void) seekBackward;
- (void) seekForward;
- (void) seekTo: (float)_position;

// core variables
- (bool) isPlaying;
- (bool) isLoading;
- (float) getVolume;
- (void) setVolume: (float)_volume;
- (int) getRepeatMode;
- (void) setRepeatMode: (int)_repeatMode;
- (bool) getShuffleModeEnabled;
- (void) setShuffleModeEnabled: (bool)_shuffleModeEnabled;

// interface variables
- (long) getDuration;
- (long) getCurrentPosition;
- (int) getCurrentMediaItemIndex;
- (int) getShuffleOrder: (int)_id;
- (bool) hasPreviousMediaItem;
- (bool) hasNextMediaItem;
- (MediaItem*) getCurrentMediaItem;
- (UnityListener*) getUnityListener;

// retrieving metadata
- (char*) retrieveAlbumTitle: (int)_id;
- (char*) retrieveAlbumArtist: (int)_id;
- (char*) retrieveTitle: (int)_id;
- (char*) retrieveArtist: (int)_id;
- (char*) retrieveGenre: (int)_id;
- (char*) retrieveReleaseDate: (int)_id;
- (char*) retrieveArtwork: (int)_id;

// playlist
- (void) addMediaItem: (int)_id json:(NSString*)_json;
- (void) removeMediaItem: (int)_id;

// extra
- (void) useRemoteCommands: (bool)_active;

@property(nonatomic, retain) AVLocalPlayer*  localPlayer;
@property(nonatomic, retain) AVRemotePlayer* remotePlayer;

@end
