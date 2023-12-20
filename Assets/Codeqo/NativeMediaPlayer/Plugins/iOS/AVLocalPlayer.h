//
//  AVLocalPlayer.h
//  Extented AVAudioPlayer for Unity (ExoPlayer Style)
//
//  Created by Yohan Song 4/15/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import <AVFoundation/AVFoundation.h>
#import <Foundation/Foundation.h>
#import "MediaItem.h"
#import "UnityListener.h"
#import "SharedVariables.h"

@interface AVLocalPlayer: NSObject <AVAudioPlayerDelegate> {
    //AVAudioPlayer*              player;
    NSMutableArray<MediaItem*>* mediaItems;
    NSMutableArray<NSNumber*>*  playOrder;
    UnityListener*              unityListener;
    /* additional values */
    float   volume;
    int     currentMediaItemIndex;
    int     seekIncrement;
    int     repeatMode;
    bool    shuffleModeEnabled;
    bool    playWhenReady;
    bool    internalPlayWhenReady;
    bool    forceNotPlay;
    bool    isLoading;
    bool    isDownload;
    int     readyCheck;
    int     itemCheck;
}

// playbacks (only add new methods. ignore overriden methods)
- (id) initWithMediaItems:(NSArray<MediaItem*>*)_mediaItems 
           startingItemId:(int)_id
        unityListenerName:(NSString*)_listener 
            playWhenReady:(bool)_playWhenReady
               isDownload:(bool)_isDownload;
- (void) prepareMediaItem:(int)_id
            playWhenReady:(bool)_playWhenReady;
- (void) play;
- (void) pause;
- (void) stop;
- (void) seekBack;
- (void) seekForward;
- (void) seekToPrevious;
- (void) seekToNext;
- (void) seekTo:(float)position;

// getset
- (float) getVolume;
- (void) setVolume:(float)value;
- (int) getRepeatMode;
- (void) setRepeatMode:(int)value;
- (bool) getShuffleModeEnabled;
- (void) setShuffleModeEnabled:(bool)value;
- (bool) getPlayWhenReady;
- (void) setPlayWhenReady:(bool)value;
- (int) getSeekIncrement;
- (void) setSeekIncrement:(int)value;

// get
- (int) getCurrentMediaItemIndex;
- (NSArray<NSNumber*>*) getShuffleOrder;
- (long) getDuration;
- (long) getCurrentPosition;
- (MediaItem*) getCurrentMediaItem;
- (MediaItem*) getMediaItem:(int)_id;
- (UnityListener*) getUnityListener;

// readonly booleans
- (bool) isPlaying;
- (bool) isLoading;
- (bool) hasNextMediaItem;
- (bool) hasPreviousMediaItem;

// playlist
- (void) addMediaItem:(int)_id json:(NSString*)_json;
- (void) removeMediaItem:(int)_id;

@property(nonatomic, retain) AVAudioPlayer* player;

@end
