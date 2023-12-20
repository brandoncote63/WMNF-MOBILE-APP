//
//  AVRemotePlayer.m
//  Extented AVQueuePlayer for Unity (ExoPlayer Style)
//
//  Created by Yohan Song 4/15/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import "AVRemotePlayer.h"
#import <AudioToolbox/AudioToolbox.h>

@interface AVRemotePlayer()
- (void) prepare;
- (void) setReady;
- (void) resetPlaylist;
- (void) shufflePlaylist;
- (int) syncIndex:(int)current;
- (int) getMediaItemIndex:(int)_id;
- (bool) isLastMediaItem;
- (void) setIsLoading:(bool)value;
- (void) repeatModeAction:(bool)isNext;
- (void) saveAutoSave;
- (void) loadAutoSave;
@end

@implementation AVRemotePlayer // public methods

- (float) getVolume {
    return volume;
}

- (void) setVolume: (float)value {
    volume = value;
    [super setVolume:value];
}

- (void) setSeekIncrement: (int)value {
    seekIncrement = value;
}

- (int) getSeekIncrement {
    return seekIncrement;
}

- (void) setRepeatMode: (int)value {
    repeatMode = value;
}

- (int) getRepeatMode {
    return repeatMode;
}

- (void) setShuffleModeEnabled: (bool)value {
    shuffleModeEnabled = value;
    int current = [self getCurrentMediaItemIndex];
    if (value) [self shufflePlaylist];
    else [self resetPlaylist];
    currentMediaItemIndex = [self syncIndex:current];
}

- (bool) getShuffleModeEnabled {
    return shuffleModeEnabled;
}

- (void) resetPlaylist {
    playOrder = nil;
    playOrder = [[NSMutableArray alloc] init];
    for (int i = 0; i < mediaItems.count; ++i) {
        [playOrder addObject:[NSNumber numberWithInteger:i]];
    }
}

- (void) shufflePlaylist {
    for (NSUInteger i = 0; i < playOrder.count; ++i) {
        NSInteger remainingCount = playOrder.count - i;
        NSInteger exchangeIndex = i + arc4random_uniform((u_int32_t)remainingCount);
        [playOrder exchangeObjectAtIndex:i withObjectAtIndex:exchangeIndex];
    }
}

- (NSArray<NSNumber*>*) getShuffleOrder {
    return playOrder;
}

- (void) setPlayWhenReady: (bool)value {
    playWhenReady = value;
}

- (bool) getPlayWhenReady {
    return playWhenReady;
}

- (void) setPreBufferDuration: (int)value {
    preBufferDuration = value;
}

- (int) getPreBufferDuration {
    return preBufferDuration;
}

- (int) getMediaItemIndex:(int)_id {
    return (int)[[playOrder objectAtIndex:_id] integerValue];
}

- (int) getCurrentMediaItemIndex {
    return [self getMediaItemIndex:currentMediaItemIndex];
}

- (MediaItem*) getCurrentMediaItem {
    return mediaItems[[self getCurrentMediaItemIndex]];
}

- (MediaItem*) getMediaItem:(int)_id {
    return mediaItems[_id];
}

- (AVPlayerItem*) getCurrentAVPlayerItem {
    NSURL *mediaURL = [self getCurrentMediaItem].MediaUri;
    
    if (mediaURL == nil || ![mediaURL isKindOfClass:[NSURL class]] || ![mediaURL scheme]) {
        NSLog(@"Invalid URL");
        return nil;
    }
    
    AVPlayerItem *item = [[AVPlayerItem alloc] initWithURL:mediaURL];
    
    if (item.status == AVPlayerItemStatusFailed) {
        NSLog(@"Failed to initialize AVPlayerItem: %@", item.error.localizedDescription);
        return nil;
    }
    
    item.preferredForwardBufferDuration = preBufferDuration;
    item.canUseNetworkResourcesForLiveStreamingWhilePaused = true;
    
    return item;
}

- (UnityListener*) getUnityListener {
    return unityListener;
}

- (void) seekTo: (float)value {
    if (value > [self getDuration]) return;
    CMTime pos = CMTimeMakeWithSeconds(value, NSEC_PER_SEC);
    [super seekToTime:pos];
}

- (void) seekToPrevious {
    if ([self getCurrentPosition] > 1) {
        [super seekToTime:kCMTimeZero];
    }
    if (currentMediaItemIndex != 0) {
        currentMediaItemIndex--;
    } else {
        if (repeatMode != REPEAT_MODE_ALL) return;
        currentMediaItemIndex = (int)mediaItems.count - 1;
    } 
    [self prepare];
}

- (void) seekToNext {
    if (currentMediaItemIndex != mediaItems.count - 1) {
        currentMediaItemIndex++;
    } else {
        if (repeatMode != REPEAT_MODE_ALL) return;
        currentMediaItemIndex = 0;
    }
    [self prepare];
}

- (void) seekBack {
    float time = [self getCurrentPosition] - seekIncrement;
    if (time > 0) {
        [self seekToTime:CMTimeMakeWithSeconds(time, 1)];
    } else {
        [self repeatModeAction:/*isNext*/false];
    }
}

- (void) seekForward {
    float time = [self getCurrentPosition] + seekIncrement;
    if (time < [self getDuration]) {
        [self seekToTime:CMTimeMakeWithSeconds(time, 1)];
    } else {
        [self repeatModeAction:/*isNext*/true];
    }
}

/* @Override */
- (void) play {
    [super play];
    [unityListener setIsPlayingChanged:true];
}

/* @Override */
- (void) pause {
    [super pause];
    [unityListener setIsPlayingChanged:false];
}

- (void) stop {
    if ([self rate] != 0) {
        [self setRate:0];
        [unityListener setIsPlayingChanged:false];
    }
}

- (long) getDuration {
    if ([self currentItem].status == AVPlayerItemStatusReadyToPlay) {
        return (long)CMTimeGetSeconds([self currentItem].asset.duration);
    }
    return 0;
}

- (long) getCurrentPosition {
    if ([self currentItem].status == AVPlayerItemStatusReadyToPlay) {
        return (long)CMTimeGetSeconds([self currentItem].currentTime);
    }
    return 0;
}

- (id) initWithMediaItems:(NSArray<MediaItem*>*)_mediaItems
           startingItemId:(int)_id
        unityListenerName:(NSString*)_listener
            playWhenReady:(bool)_playWhenReady
{
    if (self = [super init]) {//[super initWithItems:[self convertMediaItemsToAVPlayerItems:_mediaItems]]) {
        mediaItems = [[NSMutableArray alloc] initWithArray:_mediaItems];
        unityListener = [[UnityListener alloc] initWithObjectName:_listener];
        playWhenReady = _playWhenReady;
        [self resetPlaylist];

        if (_id == -1) [self loadAutoSave];
        else {
            currentMediaItemIndex = _id;
            volume = [[SharedVariables sharedObject] loadVolume];
        }

        [self prepare];
        [unityListener setInit];
        [self setReady];
    }
    return self;
}

- (void) setReady {
    if (itemCheck > 30) {
        [unityListener setError:@"Something went wrong while loading media items"];
        itemCheck = 0;
        return;
    }
    
    for (int i = 0; i < mediaItems.count; ++i) {
        if (!mediaItems[i].isPrepared) {
            itemCheck++;
            [self performSelector:@selector(setReady) withObject:nil afterDelay:1];
            return;
        }
    }

    [unityListener setReady];
    itemCheck = 0;
}

- (void) prepareMediaItem:(int)_id playWhenReady:(bool)_playWhenReady
{
    currentMediaItemIndex = _id;
    playWhenReady = _playWhenReady;
    [self prepare];
}

/* @SomethingLike OnCompletionListener */
- (void) onCompletionEvent:(NSNotification*)notification {
    [self repeatModeAction:true];
}

- (bool) isPlaying {
    if (super.rate != 0 && super.error == nil) return true;
    else return false;
}

- (bool) isLoading {
    return isLoading;
}

- (void) setIsLoading: (bool)value {
    isLoading = value;
    [unityListener setIsLoadingChanged:value];
}

- (bool) hasPreviousMediaItem {
    if (currentMediaItemIndex == 0) return repeatMode == REPEAT_MODE_ALL;
    return true;
}

- (bool) hasNextMediaItem {
    if (currentMediaItemIndex == mediaItems.count - 1) return repeatMode == REPEAT_MODE_ALL;
    return true;
}

- (void) repeatModeAction: (bool)isNext {
    if (repeatMode == REPEAT_MODE_OFF) {
        if ([self isLastMediaItem]) {
            [self stop];
            [unityListener setComplete];
            currentMediaItemIndex = [self syncIndex:[self getMediaItemIndex:0]];
            forceNotPlay = true;
            [self prepare];
        } else {
            [self seekToNext];
        }
    } else if (repeatMode == REPEAT_MODE_ALL) {
        if ([self isPlaying]) internalPlayWhenReady = true;
        if (isNext) [self seekToNext]; else [self seekToPrevious];
    } else if (repeatMode == REPEAT_MODE_ONE) {
        [super seekToTime:kCMTimeZero];
        [self play];
    } else {
        NSLog(@"Error:Invalid repeatMode");
    }
}

- (bool) isLastMediaItem {
    return currentMediaItemIndex == playOrder.count - 1;
}

- (int) syncIndex: (int)current {
    int valueTo = 0;
    for (int i = 0; i < playOrder.count; ++i) {
        if (current == [self getMediaItemIndex:i]) {
            valueTo = i;
        }
    }
    return valueTo;
}

- (void) prepare {
    if ([self isPlaying]) {
        [self stop];
    }
    [self setIsLoading:true];
    [self saveAutoSave];
    
    if ([self getCurrentAVPlayerItem] == nil) {
        [unityListener setError:@"There was an error loading this media source"];
        NSLog(@"There was an error loading this media source");
        [self setIsLoading:false];
        return;
    }

    if (super.currentItem) {
        [super.currentItem removeObserver:self forKeyPath:@"status"];
        [super.currentItem removeObserver:self forKeyPath:@"playbackBufferEmpty"];
        [super.currentItem removeObserver:self forKeyPath:@"playbackLikelyToKeepUp"];
        [super.currentItem removeObserver:self forKeyPath:@"playbackBufferFull"];
        [[NSNotificationCenter defaultCenter] removeObserver:self name:AVPlayerItemDidPlayToEndTimeNotification object:super.currentItem];
    }

    [self replaceCurrentItemWithPlayerItem:[self getCurrentAVPlayerItem]];
    NSLog(@"setting vol to %f", volume);
    [super setVolume:volume];
    [self setSeekIncrement:[[SharedVariables sharedObject] loadSeekIncrement]];
    [self setPreBufferDuration:[[SharedVariables sharedObject] loadPreBufferDuration]];
    super.automaticallyWaitsToMinimizeStalling = false;
    
    NSKeyValueObservingOptions options = NSKeyValueObservingOptionOld | NSKeyValueObservingOptionNew;

    [super.currentItem addObserver:self
                        forKeyPath:@"status"
                           options:options
                           context:nil];
    
    [super.currentItem addObserver:self
                        forKeyPath:@"playbackBufferEmpty"
                           options:options
                           context:nil];
    
    [super.currentItem addObserver:self
                        forKeyPath:@"playbackLikelyToKeepUp"
                           options:options
                           context:nil];
    
    [super.currentItem addObserver:self
                        forKeyPath:@"playbackBufferFull"
                           options:options
                           context:nil];

    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(onCompletionEvent:)
                                                 name:AVPlayerItemDidPlayToEndTimeNotification
                                               object:super.currentItem];
}

/* @SomethingLike OnPreparedListener */
- (void) observeValueForKeyPath:(NSString*)keyPath ofObject:(id)object change:(NSDictionary<NSKeyValueChangeKey,id> *)change context:(void *)context {
    
    if ([keyPath isEqualToString:@"status"]) {
        AVPlayerItemStatus status = AVPlayerItemStatusUnknown;
        NSNumber *statusNumber = change[NSKeyValueChangeNewKey];
        if ([statusNumber isKindOfClass:[NSNumber class]]) {
            status = statusNumber.integerValue;
        }
        switch (status) {
            case AVPlayerItemStatusReadyToPlay:
                if(internalPlayWhenReady || playWhenReady) {
                    if(!forceNotPlay) {
                        [self play];
                    } else {
                        forceNotPlay = false;
                    }
                }
                [self performSelector:@selector(setPrepared) withObject:nil afterDelay:.3];
                NSLog(@"AVRemotePlayer Prepared");
                break;
            case AVPlayerItemStatusFailed:
                [unityListener setError:@"Error occurred: AVPlayerItemStatusFailed"];
                break;
            case AVPlayerItemStatusUnknown:
                [unityListener setError:@"Error occurred: AVPlayerItemStatusUnknown"];
                break;
        }
        [self setIsLoading:false];
    } else if ([keyPath isEqualToString:@"playbackBufferEmpty"]) {
        [unityListener setIsBufferingChanged:true];
    } else if ([keyPath isEqualToString:@"playbackLikelyToKeepUp"]) {
        [unityListener setIsBufferingChanged:false];
    } else if ([keyPath isEqualToString:@"playbackBufferFull"]) {
        [unityListener setIsBufferingChanged:false];
    }
}

- (void) setPrepared {
    [unityListener setPrepared];
}

- (void) loadAutoSave {
    if ([[SharedVariables sharedObject] loadAutoSave]) {
        [self setRepeatMode:[[SharedVariables sharedObject] loadRepeatMode]];
        [self setShuffleModeEnabled:[[SharedVariables sharedObject] loadShuffleModeEnabled]];
        volume = [[SharedVariables sharedObject] loadVolume];
        currentMediaItemIndex = [[SharedVariables sharedObject] loadCurrentMediaItemIndex];
    } else {
        [self setRepeatMode:REPEAT_MODE_OFF];
        [self setShuffleModeEnabled:false];
        volume = 1;
        currentMediaItemIndex = 0;
    }
}

- (void) saveAutoSave {
    if ([[SharedVariables sharedObject] loadAutoSave]) {
        [[SharedVariables sharedObject] saveCurrentMediaItemIndex:currentMediaItemIndex];
        [[SharedVariables sharedObject] saveVolume:volume];
        [[SharedVariables sharedObject] saveRepeatMode:repeatMode];
        [[SharedVariables sharedObject] saveShuffleModeEnabled:shuffleModeEnabled];
    }
}

- (void) addMediaItem:(int)_id json:(NSString*)_json {
    int count = (int)mediaItems.count + 1;
    [mediaItems insertObject:[[MediaItem alloc] initWithJson:_id count:count pathType:0 json:_json] atIndex:_id];
    [playOrder insertObject:[NSNumber numberWithInteger:_id] atIndex:_id];
}

- (void) removeMediaItem:(int)_id {
    [mediaItems removeObjectAtIndex:_id];
    [playOrder removeObjectAtIndex:_id];
}

@end







