//
//  AVLocalPlayer.m
//  Extented AVAudioPlayer for Unity (ExoPlayer Style)
//
//  Created by Yohan Song 4/15/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import "AVLocalPlayer.h"
#import <AudioToolbox/AudioToolbox.h>

@interface AVLocalPlayer()
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

@implementation AVLocalPlayer // public methods
@synthesize player;

- (float) getVolume {
    NSLog(@"getVolume %f", volume);
    return volume;
}

- (void) setVolume: (float)value {
    volume = value;
    [self.player setVolume:value];
    NSLog(@"volume changed %f", volume);
}

- (void) setSeekIncrement: (int)value {
    seekIncrement = value;
}

- (int) getSeekIncrement {
    return seekIncrement;
}

- (void) setRepeatMode: (int)value {
    repeatMode = value;
    if (repeatMode == REPEAT_MODE_ONE) {
        [player setNumberOfLoops:-1];
    } else {
        [player setNumberOfLoops:0];
    }
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

- (UnityListener*) getUnityListener {
    return unityListener;
}

- (void) seekTo: (float)value {
    if (value > [self getDuration]) return;
    self.player.currentTime = (NSTimeInterval)value;
}

- (void) seekToPrevious {
    if ([self getCurrentPosition] > 1) {
        self.player.currentTime = 0;
    }
    if (currentMediaItemIndex != 0) {
        currentMediaItemIndex--;
    } else {
        if (repeatMode != REPEAT_MODE_ALL) return;
        currentMediaItemIndex = (int)playOrder.count - 1;
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
        self.player.currentTime -= (NSTimeInterval)seekIncrement;
    } else {
        [self repeatModeAction:/*isNext*/false];
    }
}

- (void) seekForward {
    float time = [self getCurrentPosition] + seekIncrement;
    if (time < [self getDuration]) {
        self.player.currentTime += (NSTimeInterval)seekIncrement;
    } else {
        [self repeatModeAction:/*isNext*/true];
    }
}

- (void) play {
    [self.player play];
    [unityListener setIsPlayingChanged:true];
}

- (void) pause {
    [self.player pause];
    [unityListener setIsPlayingChanged:false];
}

- (void) stop {
    if ([self.player isPlaying]) {
        [self.player stop];
        [unityListener setIsPlayingChanged:false];
    }
}

- (long) getDuration {
    if ([self isLoading]) return 0;
    return (long) self.player.duration;
}

- (long) getCurrentPosition {
    if ([self isLoading]) return 0;
    return (long) self.player.currentTime;
}

- (id) initWithMediaItems:(NSArray<MediaItem*>*)_mediaItems
           startingItemId:(int)_id
        unityListenerName:(NSString*)_listener
            playWhenReady:(bool)_playWhenReady 
               isDownload:(bool)_isDownload
{
    if (self = [super init]) {
        mediaItems = [[NSMutableArray alloc] initWithArray:_mediaItems];
        unityListener = [[UnityListener alloc] initWithObjectName:_listener];
        playWhenReady = _playWhenReady;        
        isDownload = _isDownload;
        
        [self resetPlaylist];

        if (_id == -1) [self loadAutoSave];
        else {
            currentMediaItemIndex = _id;
            volume = [[SharedVariables sharedObject] loadVolume];
        }

        [self prepare];
        [unityListener setInit];
        [self setReady];
        NSLog(@"initWithMediaItems");
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

- (bool) isPlaying {
    return self.player.isPlaying;
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
    if (repeatMode == REPEAT_MODE_OFF && [self isLastMediaItem]) {
        [self.player stop];
        [unityListener setComplete];
        currentMediaItemIndex = [self syncIndex:[self getMediaItemIndex:0]];
        forceNotPlay = true;
        [self prepare];
    } else {
        if ([self.player isPlaying]) internalPlayWhenReady = true;
        if (isNext) [self seekToNext];
        else [self seekToPrevious];
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
        internalPlayWhenReady = true;
        [self stop];
    }
    [self setIsLoading:true];
    [self saveAutoSave];
    
    readyCheck = 0;
    NSError *error;
    
    self.player = nil;
    NSLog(@"Loading index: %d / uri: %@", [self getCurrentMediaItemIndex], [self getCurrentMediaItem].MediaUri);

    if (isDownload) {
        NSArray *documentDirectories = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
        NSString *persistantDataPath = [documentDirectories objectAtIndex:0];
        NSString *filename = [[self getCurrentMediaItem].MediaUri lastPathComponent];
        NSString *filePath = [persistantDataPath stringByAppendingPathComponent:filename];
        NSFileManager *fileManager = [NSFileManager defaultManager];
        bool fileExists = [fileManager fileExistsAtPath:filePath];
        
        if (fileExists) {
            NSURL *fileURL = [NSURL fileURLWithPath:filePath];
            NSError *error = nil;
            self.player = [[AVAudioPlayer alloc] initWithContentsOfURL:fileURL error:&error];
            if (error) {
                NSLog(@"AVAudioPlayer Error: %@", error.localizedDescription);
            }
            [self continueToPrepare];
        } else {            
            NSURLSession *session = [NSURLSession sharedSession];
            NSURLSessionDownloadTask *downloadTask = [session downloadTaskWithURL:[self getCurrentMediaItem].MediaUri completionHandler:^(NSURL *location, NSURLResponse *response, NSError *error) {
                if (error) {
                    [self->unityListener setError: [error localizedDescription]];
                    [self setIsLoading:false];
                } else {
                    NSData *audioData = [NSData dataWithContentsOfURL:location];
                    [audioData writeToFile:filePath atomically:YES];
                    NSURL *fileURL = [NSURL fileURLWithPath:filePath];
                    NSError *audioError = nil;
                    self.player = [[AVAudioPlayer alloc] initWithContentsOfURL:fileURL error:&audioError];
                    [self continueToPrepare];
                }
            }];
            [downloadTask resume];
            return;
        }

	} else {
        self.player = [[AVAudioPlayer alloc] initWithContentsOfURL:[self getCurrentMediaItem].MediaUri error:&error];
    }

    if (error) {
        [unityListener setError: [error localizedDescription]];
        [self setIsLoading:false];
        return;
    }

    [self continueToPrepare];
}

- (void) continueToPrepare {
    [self.player setVolume:volume];
    [self setSeekIncrement:[[SharedVariables sharedObject] loadSeekIncrement]];

    [self.player prepareToPlay];
    /* (important!) This enables */
    [self.player setDelegate:self];
    /* - audioPlayerDidFinishPlaying:successfully:
       - audioPlayerDecodeErrorDidOccur:error: */

    if (internalPlayWhenReady || playWhenReady) {
        if (!forceNotPlay) [self play];
        else forceNotPlay = false;
    }
    
    [self checkPlaybackStateReady];
}

/* Something like OnCompletionListener */
- (void) audioPlayerDidFinishPlaying:(AVAudioPlayer *)player successfully:(BOOL)flag {
    [self repeatModeAction:true];
}

/* Something like OnErrorListener */
- (void) audioPlayerDecodeErrorDidOccur:(AVAudioPlayer *)player error:(NSError *)error {
    [unityListener setError:[error localizedDescription]];
}

/* player-made OnPreparedListener */
- (void) checkPlaybackStateReady {
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        NSLog(@"Checking playback state... %d", self->readyCheck);
        self->readyCheck++;
        if (self->readyCheck > 30) {
            [self->unityListener setError:@"AVAudioPlayer failed to start"];
            self->readyCheck = 0;
            return;
        }
        if (![[self getCurrentMediaItem] isPrepared]) {
            NSLog(@"MediaItem is not prepared yet.");
            [self performSelector:@selector(checkPlaybackStateReady) withObject:nil afterDelay:.5];
            return;
        }
        NSLog(@"AVLocalPlayer Prepared");
        [self setIsLoading:false];
        [self->unityListener setPrepared];
    });
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







