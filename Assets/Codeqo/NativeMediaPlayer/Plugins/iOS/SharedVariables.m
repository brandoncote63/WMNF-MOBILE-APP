//
//  SharedVariables.m (Singleton)
//  Shared Constants for NativeMediaPlayer for Unity 
//
//  Created by Yohan Song 5/10/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import "SharedVariables.h"

@interface SharedVariables()
@end

@implementation SharedVariables

+ (SharedVariables*) sharedObject {
	static SharedVariables *sharedObject = nil;
	if (!sharedObject) {
		sharedObject = [[SharedVariables alloc] init];
    }
    return sharedObject;
}

- (bool) loadAutoSave {
    return [[PlayerPrefs sharedObject] getInt:@"AUTO_SAVE"] == 1; 
    // getInt returns 0 when value doesn't exist, which means this return false as default
}

- (float) loadVolume {
    if([[NSUserDefaults standardUserDefaults] objectForKey:@"VOLUME"])
        return [[PlayerPrefs sharedObject] getFloat:@"VOLUME"];
    return 1;
}

- (int) loadCurrentMediaItemIndex {
    return [[PlayerPrefs sharedObject] getInt:@"CURRENT_MEDIA_ITEM_INDEX"];
}

- (int) loadPreBufferDuration {
    if([[NSUserDefaults standardUserDefaults] objectForKey:@"PRE_BUFFER_DURATION"])
        return [[PlayerPrefs sharedObject] getInt:@"PRE_BUFFER_DURATION"];
    return 5;
}

- (int) loadSeekIncrement {
    if([[NSUserDefaults standardUserDefaults] objectForKey:@"SEEK_INCREMENT"])
        return [[PlayerPrefs sharedObject] getInt:@"SEEK_INCREMENT"];
    return 10;
}

- (int) loadRepeatMode {
    return [[PlayerPrefs sharedObject] getInt:@"REPEAT_MODE"];
}

- (bool) loadShuffleModeEnabled {
    return [[PlayerPrefs sharedObject] getInt:@"SHUFFLE_MODE"] == 1;
}

- (void) saveVolume:(float)value {
    [[PlayerPrefs sharedObject] setFloat:@"VOLUME" value:value];
}

- (void) saveCurrentMediaItemIndex:(int)value {
    [[PlayerPrefs sharedObject] setInt:@"CURRENT_MEDIA_ITEM_INDEX" value:value];
}

- (void) saveRepeatMode:(int)value {
    [[PlayerPrefs sharedObject] setInt:@"REPEAT_MODE" value:value];
}

- (void) saveShuffleModeEnabled:(bool)value {
    [[PlayerPrefs sharedObject] setInt:@"SHUFFLE_MODE" value:value? 1 : 0];
}

@end
