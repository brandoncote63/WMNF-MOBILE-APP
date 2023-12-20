//
//  SharedVariables.h (Singleton)
//  Shared Constants for NativeMediaPlayer for Unity 
//
//  Created by Yohan Song 5/10/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import "PlayerPrefs.h"

typedef NS_ENUM(NSInteger, UriType) {
    STREAMING_ASSET,
    REMOTE_URL,
    DOWNLOAD_AND_PLAY
};

typedef NS_ENUM(NSInteger, RepeatMode) {
    REPEAT_MODE_OFF,
    REPEAT_MODE_ONE,
    REPEAT_MODE_ALL
};

@interface SharedVariables : NSObject
+ (SharedVariables*) sharedObject;
/* Gets */
- (bool) loadAutoSave;
- (float) loadVolume;
- (int) loadCurrentMediaItemIndex;
- (int) loadPreBufferDuration;
- (int) loadSeekIncrement;
- (int) loadRepeatMode;
- (bool) loadShuffleModeEnabled;
/* Sets (only available for variables that needs to be saved) */
- (void) saveVolume:(float)value;
- (void) saveCurrentMediaItemIndex:(int)value;
- (void) saveRepeatMode:(int)value;
- (void) saveShuffleModeEnabled:(bool)value;

@property(nonatomic) bool isInit;

@end
