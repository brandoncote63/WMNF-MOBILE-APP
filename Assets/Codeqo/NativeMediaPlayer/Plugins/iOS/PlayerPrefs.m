//
//  PlayerPrefs.m
//  PlayerPrefs Emulator for Unity iOS Plugins (Singleton)
//
//  Created by Yohan Song 5/10/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import "PlayerPrefs.h"

@interface PlayerPrefs()
@end

@implementation PlayerPrefs

+ (PlayerPrefs*) sharedObject {
	static PlayerPrefs *sharedObject = nil;
	if (!sharedObject) {
		sharedObject = [[PlayerPrefs alloc] init];
    }
    return sharedObject;
}

- (NSString*) getString:(NSString*)_key
{
    if([[NSUserDefaults standardUserDefaults] objectForKey:_key]){
        NSString *tempStr = [[NSUserDefaults standardUserDefaults] objectForKey:_key];
        [tempStr stringByReplacingOccurrencesOfString:@"%20" withString:@" "];
        [tempStr stringByReplacingOccurrencesOfString:@"%2F" withString:@"/"];
        [tempStr stringByReplacingOccurrencesOfString:@"%2B" withString:@"+"];
        [tempStr stringByReplacingOccurrencesOfString:@"%3D" withString:@"="];
        [tempStr stringByReplacingOccurrencesOfString:@"%3A" withString:@":"];
        [tempStr stringByReplacingOccurrencesOfString:@"%23" withString:@"#"];
        return tempStr;
    } else {
        //NSLog(@"%@", [_key stringByAppendingString:@" doesn't exist!"]);
        return @"";
    }
}

- (NSString*) getString:(NSString*)_key id:(int)_id
{
    NSString *tempStr = [NSMutableString stringWithFormat:@"%d_%@", _id, _key];
    return [self getString:tempStr];
}

- (int) getInt:(NSString*)_key
{
    if([[NSUserDefaults standardUserDefaults] objectForKey:_key]){
        NSNumber* t = [[NSUserDefaults standardUserDefaults] objectForKey:_key];
        return t.intValue;
    } else {
        NSLog(@"%@", [_key stringByAppendingString:@" doesn't exist!"]);
        return 0;
    }
}

- (int) getInt:(NSString*)_key id:(int)_id
{
    NSString *tempStr = [NSMutableString stringWithFormat:@"%d_%@", _id, _key];
    return [self getInt:tempStr];
}

- (float) getFloat:(NSString*)_key
{
    if([[NSUserDefaults standardUserDefaults] objectForKey:_key]){
        return [[NSUserDefaults standardUserDefaults] floatForKey:_key];
    } else {
        NSLog(@"%@", [_key stringByAppendingString:@" doesn't exist!"]);
        return 0;
    }
}

- (float) getFloat:(NSString*)_key id:(int)_id
{
    NSString *tempStr = [NSMutableString stringWithFormat:@"%d_%@", _id, _key];
    return [self getFloat:tempStr];
}

- (void) setString:(NSString *)_key value:(NSString *)_value
{
    [[NSUserDefaults standardUserDefaults] setObject:_value forKey:_key];
    [[NSUserDefaults standardUserDefaults] synchronize];
}

- (void) setInt:(NSString *)_key value:(int)_value
{
    NSNumber *intValue = [NSNumber numberWithInt:_value];
    [[NSUserDefaults standardUserDefaults] setObject:intValue forKey:_key];
    [[NSUserDefaults standardUserDefaults] synchronize];
}

- (void) setFloat:(NSString *)_key value:(float)_value
{
    NSNumber *floatValue = [NSNumber numberWithFloat:_value];
    [[NSUserDefaults standardUserDefaults] setObject:floatValue forKey:_key];
    [[NSUserDefaults standardUserDefaults] synchronize];
}

- (void) deleteKey:(NSString *)_key
{
    [[NSUserDefaults standardUserDefaults] removeObjectForKey:_key];
    [[NSUserDefaults standardUserDefaults] synchronize];
}

@end
