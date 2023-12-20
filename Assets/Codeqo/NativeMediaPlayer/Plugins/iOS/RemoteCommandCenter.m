//
//  NativeMediaPlayer+RemoteCommandCenter.m
//  RemoteCommandCenter Extention for NativeMediaPlayer for Unity
//
//  Created by Yohan Song 5/11/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import "RemoteCommandCenter.h"
#import "PlayerPrefs.h"
#import "UnityListener.h"

@interface RemoteCommandCenter()
- (NativeMediaPlayer*) player;
@end

@implementation RemoteCommandCenter

+ (RemoteCommandCenter*) sharedObject {
    static RemoteCommandCenter *sharedObject = nil;
    if (!sharedObject) {
        sharedObject = [[RemoteCommandCenter alloc] init];
    }
    return sharedObject;
}

- (id) init {
    if(self = [super init]) {
        
        [[UIApplication sharedApplication] beginReceivingRemoteControlEvents];
        MPRemoteCommandCenter *commandCenter = [MPRemoteCommandCenter sharedCommandCenter];
		isActive = true;
        
        [commandCenter.togglePlayPauseCommand addTarget:self action:@selector(onTogglePlayPause:)];
        commandCenter.togglePlayPauseCommand.enabled = YES;
        
        if ([[PlayerPrefs sharedObject] getInt:@"IOS_MEDIA_ACTION_SEEKBAR"] == 1) {
            [commandCenter.changePlaybackPositionCommand addTarget:self action:@selector(onSeekTo:)];
            commandCenter.changePlaybackPositionCommand.enabled = YES;
        }
        
        if ([[PlayerPrefs sharedObject] getInt:@"IOS_MEDIA_ACTION_STOP"] == 1) {
            [commandCenter.stopCommand addTarget:self action:@selector(onStop:)];
            commandCenter.stopCommand.enabled = YES;
        }

        if ([[PlayerPrefs sharedObject] getInt:@"IOS_MEDIA_ACTION_FASTFORWARD"] == 1) {
            [commandCenter.seekForwardCommand addTarget:self action:@selector(onSeekForward:)];
            commandCenter.seekForwardCommand.enabled = YES;
        }

        if ([[PlayerPrefs sharedObject] getInt:@"IOS_MEDIA_ACTION_REWIND"] == 1) {
            [commandCenter.seekBackwardCommand addTarget:self action:@selector(onSeekBackward:)];
            commandCenter.seekBackwardCommand.enabled = YES;
        }

        if ([[PlayerPrefs sharedObject] getInt:@"IOS_MEDIA_ACTION_SKIP_TO_NEXT"] == 1) {
            [commandCenter.nextTrackCommand addTarget:self action:@selector(onNextTrack:)];
            commandCenter.nextTrackCommand.enabled = YES;
        }

        if ([[PlayerPrefs sharedObject] getInt:@"IOS_MEDIA_ACTION_SKIP_TO_PREVIOUS"] == 1) {
            [commandCenter.previousTrackCommand addTarget:self action:@selector(onPreviousTrack:)];
            commandCenter.previousTrackCommand.enabled = YES;
        }
        
        [self updateRemoteCommandCenter];
    }
    return self;
}

- (NativeMediaPlayer*) player {
    return [NativeMediaPlayer sharedObject];
}



- (void) setRemoteCommandCenter:(bool)_active {
	if (isActive == _active) return;
	isActive = _active;
	
	if (isActive) {
		[[UIApplication sharedApplication] beginReceivingRemoteControlEvents];
		[self updateRemoteCommandCenter];	
    } else {
        [[UIApplication sharedApplication] endReceivingRemoteControlEvents];
	    MPNowPlayingInfoCenter *center = [MPNowPlayingInfoCenter defaultCenter];
	    [center setNowPlayingInfo:nil];	       
	} 
}

- (void) updateRemoteCommandCenter {
    MPNowPlayingInfoCenter *center = [MPNowPlayingInfoCenter defaultCenter];
    [center setNowPlayingInfo:[self getMediaItemDictionary]];
    NSLog(@"updateRemoteCommandCenter");
}

- (NSMutableDictionary*) getMediaItemDictionary {
    NSMutableDictionary *trackInfo = [[NSMutableDictionary alloc] init];
    MediaItem* _item = [[self player] getCurrentMediaItem];
        
    if (_item.AlbumTitle != nil) {
        [trackInfo setObject:_item.AlbumTitle forKey:MPMediaItemPropertyAlbumTitle];
    }
    if (_item.AlbumArtist != nil) {
        [trackInfo setObject:_item.AlbumArtist forKey:MPMediaItemPropertyAlbumArtist];
    }
    if (_item.Title != nil) {
        [trackInfo setObject:_item.Title forKey:MPMediaItemPropertyTitle];
    }
    if (_item.Artist != nil) {
        [trackInfo setObject:_item.Artist forKey:MPMediaItemPropertyArtist];
    }
    if (_item.Genre != nil) {
        [trackInfo setObject:_item.Genre forKey:MPMediaItemPropertyGenre];
    }
    if (_item.ReleaseDate != nil) {
        [trackInfo setObject:_item.ReleaseDate forKey:MPMediaItemPropertyReleaseDate];
    }
    if (_item.Artwork != nil) {
        [trackInfo setObject:_item.Artwork forKey:MPMediaItemPropertyArtwork];
    }
    if (_item.MediaUri != nil) {
        [trackInfo setObject:_item.MediaUri forKey:MPNowPlayingInfoPropertyAssetURL];
    }
    
    [trackInfo setObject:[NSNumber numberWithFloat:[[self player] getDuration]] forKey:MPMediaItemPropertyPlaybackDuration];
    [trackInfo setObject:[NSNumber numberWithFloat:positionZero ? 0 : [[self player] getCurrentPosition]] forKey:MPNowPlayingInfoPropertyElapsedPlaybackTime];
    
    bool disableAVAudioSession = [[PlayerPrefs sharedObject] getInt:@"IOS_MEDIA_SESSION_DISABLE_AV_AUDIO_SESSION_ON_PAUSE"] == 1;
    bool enableAVAudioSession = [[PlayerPrefs sharedObject] getInt:@"IOS_MEDIA_SESSION_ENABLE_MEDIA_AV_AUDIO_IMMEDIATELY_AFTER"] == 1;
    
    if ([[self player] isPlaying]) {
        if (disableAVAudioSession)[[AVAudioSession sharedInstance] setActive:YES withOptions:0 error:nil];
    } else {
        if (disableAVAudioSession){
            [[AVAudioSession sharedInstance] setActive:NO withOptions:0 error:nil];
            if (enableAVAudioSession) [[AVAudioSession sharedInstance] setActive:YES withOptions:0 error:nil];
        }
    }
    
    return trackInfo;
}

- (MPRemoteCommandHandlerStatus)onTogglePlayPause:(MPRemoteCommandEvent*)event {
    [[self player] togglePlayPause];
    positionZero = false;
    [self updateRemoteCommandCenter];
    return MPRemoteCommandHandlerStatusSuccess;
}

- (MPRemoteCommandHandlerStatus)onPlay:(MPRemoteCommandEvent*)event {
    [[self player] play];
    positionZero = false;
    [self updateRemoteCommandCenter];
    return MPRemoteCommandHandlerStatusSuccess;
}

- (MPRemoteCommandHandlerStatus)onPause:(MPRemoteCommandEvent*)event {
    [[self player] pause];
    positionZero = false;
    [self updateRemoteCommandCenter];
    return MPRemoteCommandHandlerStatusSuccess;
}

- (MPRemoteCommandHandlerStatus)onStop:(MPRemoteCommandEvent*)event {
    [[self player] stop];
    positionZero = true;
    [self updateRemoteCommandCenter];
    return MPRemoteCommandHandlerStatusSuccess;
}

- (MPRemoteCommandHandlerStatus)onPreviousTrack:(MPRemoteCommandEvent*)event {
    [[self player] previousTrack];
    positionZero = [[self player] hasPreviousMediaItem];
    [self updateRemoteCommandCenter];
    return MPRemoteCommandHandlerStatusSuccess;
}

- (MPRemoteCommandHandlerStatus)onNextTrack:(MPRemoteCommandEvent*)event {
    [[self player] nextTrack];
    positionZero = [[self player] hasNextMediaItem];
    [self updateRemoteCommandCenter];
    return MPRemoteCommandHandlerStatusSuccess;
}

- (MPRemoteCommandHandlerStatus)onSeekBackward:(MPRemoteCommandEvent*)event {
    [[self player] seekBackward];
    positionZero = false;
    [self updateRemoteCommandCenter];
    return MPRemoteCommandHandlerStatusSuccess;
}

- (MPRemoteCommandHandlerStatus)onSeekForward:(MPRemoteCommandEvent*)event {
    [[self player] seekForward];
    positionZero = false;
    [self updateRemoteCommandCenter];
    return MPRemoteCommandHandlerStatusSuccess;
}

- (MPRemoteCommandHandlerStatus)onSeekTo:(MPChangePlaybackPositionCommandEvent*)event {
    [[self player] seekTo:event.positionTime];
    positionZero = false;
    return MPRemoteCommandHandlerStatusSuccess;
}

@end
