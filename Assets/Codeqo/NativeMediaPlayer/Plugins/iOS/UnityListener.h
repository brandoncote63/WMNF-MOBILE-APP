//
//  UnityListener.h
//  UnitySendMessage Func Extention for Unity
//
//  Created 4/15/22.
//  Updated 5/19/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import <Unity/UnityInterface.h>

@interface UnityListener : NSObject
{
    NSString* objectName;
}

- (id) initWithObjectName: (NSString*) _name;
- (void) setInit;
- (void) setReady;
- (void) setPrepared;
- (void) setComplete;
- (void) setIsLoadingChanged: (bool) _isLoading;
- (void) setIsPlayingChanged: (bool) _isPlaying;
- (void) setIsBufferingChanged: (bool) _isBuffering;
- (void) setError: (NSString*) _error;

@property(nonatomic, retain) NSString* objectName;

@end
