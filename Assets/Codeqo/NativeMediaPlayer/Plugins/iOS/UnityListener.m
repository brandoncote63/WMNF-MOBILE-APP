//
//  UnityListener.h
//  UnitySendMessage Func Extention for Unity
//
//  Created 4/15/22.
//  Updated 5/17/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import "UnityListener.h"
#import "RemoteCommandCenter.h"
#define UNITY_RECEIVER "UnityReceiveMessage"
#define UNITY_ERROR "UnityReceiveError"

@interface UnityListener()
@end

@implementation UnityListener // public methods
@synthesize objectName;

- (id) initWithObjectName: (NSString*) _objectName {
    if (self = [super init]){
        self.objectName = _objectName;
        NSLog(@"Attached Unity Listener Name : %@", self.objectName);
    }
    return self;
}

- (void) setInit; {
    if ([SharedVariables sharedObject].isInit) return;
    [SharedVariables sharedObject].isInit = true;
    UnitySendMessage([self.objectName UTF8String], UNITY_RECEIVER, "ON_INIT");
}

- (void) setReady {
    NSLog(@"Send State Ready");
    UnitySendMessage([self.objectName UTF8String], UNITY_RECEIVER, "ON_READY");
}

- (void) setPrepared {
    NSLog(@"Send State Ended");
    UnitySendMessage([self.objectName UTF8String], UNITY_RECEIVER, "ON_PREPARED");
    [[RemoteCommandCenter sharedObject] updateRemoteCommandCenter];
}

- (void) setComplete {
    NSLog(@"Send State Ended");
    UnitySendMessage([self.objectName UTF8String], UNITY_RECEIVER, "ON_COMPLETE");
}

- (void) setIsLoadingChanged: (bool)_isLoading {
    if (_isLoading) {
        UnitySendMessage([self.objectName UTF8String], UNITY_RECEIVER, "ON_IS_LOADING_CHANGED_TRUE");
    } else {
        UnitySendMessage([self.objectName UTF8String], UNITY_RECEIVER, "ON_IS_LOADING_CHANGED_FALSE");
    }
}

- (void) setIsPlayingChanged: (bool)_isPlaying {
    if (_isPlaying) {
        UnitySendMessage([self.objectName UTF8String], UNITY_RECEIVER, "ON_IS_PLAYING_CHANGED_TRUE");
    } else {
        UnitySendMessage([self.objectName UTF8String], UNITY_RECEIVER, "ON_IS_PLAYING_CHANGED_FALSE");
    }
    [[RemoteCommandCenter sharedObject] updateRemoteCommandCenter];
}

- (void) setIsBufferingChanged:(bool)_isBuffering {
    if (_isBuffering) {
        UnitySendMessage([self.objectName UTF8String], UNITY_RECEIVER, "ON_IS_BUFFERING_CHANGED_TRUE");
    } else {
        UnitySendMessage([self.objectName UTF8String], UNITY_RECEIVER, "ON_IS_BUFFERING_CHANGED_FALSE");
    }
}

- (void) setError: (NSString*)_error {
    const char *e = [_error UTF8String];
    UnitySendMessage([self.objectName UTF8String], UNITY_ERROR, e);
    NSLog(@"%s", e);
}

@end
